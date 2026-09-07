using System.Collections.Generic;
using UnityEngine;

public class controller_scenario : MonoBehaviour
{
    [System.Serializable]
    public struct EnvironmentStep
    {
        public string name;
        public float distanceToStart;
        public int firstScenarioIndex;
        public float xPosition;
    }

    // Plataformas
    public List<GameObject> platforms = new List<GameObject>();
    public GameObject finish_line;
    public List<Transform> atual_platforms = new List<Transform>();
    public float tam_rua = 32f;

    // Ambientes
    public List<GameObject> scenario = new List<GameObject>();
    public List<Transform> atual_scenario = new List<Transform>();
    public int offset;
    public int offset_scenario;
    public float tam_sce = 150f;
    public player_controller var;
    public ui_controller ui;

    // Obstaculos
    public List<GameObject> obstacles = new List<GameObject>();
    public List<float> spawn_points = new List<float> { -1.4f, 0f, 1.4f };
    public float difficultyIncreaseRate = 10f;
    public float initialSpawnDelay = 1f;
    public float spawnDelayReduction = 0.2f;
    public float minimumObstacleSpacing = 11f;
    public int maxBlockedLanes = 2;
    public float normalDistance = 900f;
    public float hardDistance = 2200f;
    public float expertDistance = 4200f;
    public List<EnvironmentStep> environmentSteps = new List<EnvironmentStep>()
    {
        new EnvironmentStep { name = "desert", distanceToStart = 1400f, firstScenarioIndex = 3, xPosition = 0f },
        new EnvironmentStep { name = "halloween", distanceToStart = 3400f, firstScenarioIndex = 6, xPosition = 0f },
        new EnvironmentStep { name = "city", distanceToStart = 5400f, firstScenarioIndex = 9, xPosition = 10.7f },
        new EnvironmentStep { name = "mountain", distanceToStart = 7400f, firstScenarioIndex = 12, xPosition = 0f }
    };

    // Moedas
    public GameObject coin_prefab;
    public int[] spawn_coin;

    private const int ScenarioGroupSize = 3;
    private const float PlatformRecycleDistance = 5f;
    private const float ScenarioRecycleDistance = 25f;
    private const float ScenarioRecycleSize = 145f;
    private const float GroundDebugOffset = 0.0001f;
    private const float FinishLineDistance = 10000f;

    private readonly Dictionary<string, string> expertObstacleSwaps = new Dictionary<string, string>()
    {
        { "traffic_pot", "traffic" },
        { "traffic", "traffic_pot" },
        { "pot_tree", "bench" },
        { "bench", "pot_tree" },
        { "trash", "trashcan" },
        { "trashcan", "trash" },
        { "hydrant", "traffic" },
        { "gravestone", "pumpkin" },
        { "pumpkin", "gravestone" },
        { "desert_column", "ram_sharp" },
        { "ram_sharp", "desert_column" },
        { "skull", "zombie_hand" },
        { "zombie_hand", "skull" },
        { "shopping_cart", "trash" }
    };

    private Transform player;
    private Transform currentPlatformPoint;
    private Transform finishLinePoint;
    private Transform currentScenarioPoint;
    private int platformIndex;
    private int scenarioIndex;
    private int nextEnvironmentIndex;
    private bool changingEnvironment;
    private float currentEnvironmentX;
    private float currentSpawnDelay;
    private int obstacles_z = 125;
    private int lastDistanceDifficulty = 1;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("Player com tag 'Player' nao encontrado.");
            enabled = false;
            return;
        }

        player = playerObject.transform;
        var = playerObject.GetComponent<player_controller>();

        SpawnInitialPlatforms();
        SpawnInitialScenario();
        SpawnFinishLine();

        currentSpawnDelay = initialSpawnDelay;
        InvokeRepeating(nameof(SpawnObstacle), initialSpawnDelay, currentSpawnDelay);
    }

    void Update()
    {
        if (player == null || var == null || ui == null)
        {
            return;
        }

        float platformDistance = player.position.z - currentPlatformPoint.position.z;
        float finishDistance = player.position.z - finishLinePoint.position.z;
        float scenarioDistance = player.position.z - currentScenarioPoint.position.z;

        if (platformDistance >= PlatformRecycleDistance)
        {
            RecyclePlatform();
        }

        if (finishDistance >= 0f)
        {
            var.finish_line_player();
        }

        if (scenarioDistance >= ScenarioRecycleDistance)
        {
            RecycleScenario();
        }

        TryChangeEnvironment(scenarioDistance);
        UpdateDistanceDifficulty();
        UpdateObstacleSpawnDelay();
    }

    public void recycle(GameObject plataforma, int range)
    {
        plataforma.transform.position = new Vector3(0f, 0f, offset);
        offset += range;
    }

    public void recycle_scenario(GameObject plataforma, int range, float x)
    {
        plataforma.transform.position = new Vector3(x, GroundDebugOffset, offset_scenario);
        offset_scenario += range;
    }

    private void SpawnInitialPlatforms()
    {
        atual_platforms.Clear();

        for (int i = 0; i < platforms.Count; i++)
        {
            Transform platform = Instantiate(platforms[i], new Vector3(0f, 0f, i * tam_rua), transform.rotation).transform;
            atual_platforms.Add(platform);
            offset += Mathf.RoundToInt(tam_rua);
        }

        currentPlatformPoint = GetPlatformPoint(atual_platforms, platformIndex, "plataforma");
    }

    private void SpawnInitialScenario()
    {
        atual_scenario.Clear();

        int scenarioCount = Mathf.Min(ScenarioGroupSize, scenario.Count);
        for (int i = 0; i < scenarioCount; i++)
        {
            Transform environment = Instantiate(scenario[i], new Vector3(0f, GroundDebugOffset, i * tam_sce), transform.rotation).transform;
            environment.name = "florest" + i;
            atual_scenario.Add(environment);
            offset_scenario += Mathf.RoundToInt(tam_sce);
        }

        currentScenarioPoint = GetPlatformPoint(atual_scenario, scenarioIndex, "cenario");
    }

    private void SpawnFinishLine()
    {
        Transform finish = Instantiate(finish_line, new Vector3(0f, GroundDebugOffset, FinishLineDistance), transform.rotation).transform;
        finish.Rotate(new Vector3(0f, 90f, 0f));
        finishLinePoint = finish.GetComponent<platforms>().point;
    }

    private Transform GetPlatformPoint(List<Transform> list, int index, string label)
    {
        if (list.Count == 0)
        {
            Debug.LogError("Nenhum item de " + label + " configurado.");
            enabled = false;
            return null;
        }

        platforms platformData = list[index].GetComponent<platforms>();
        if (platformData == null || platformData.point == null)
        {
            Debug.LogError("O prefab de " + label + " precisa do script platforms com o campo point preenchido.");
            enabled = false;
            return null;
        }

        return platformData.point;
    }

    private void RecyclePlatform()
    {
        recycle(atual_platforms[platformIndex].gameObject, Mathf.RoundToInt(tam_rua));
        platformIndex = NextIndex(platformIndex, atual_platforms.Count);
        currentPlatformPoint = GetPlatformPoint(atual_platforms, platformIndex, "plataforma");
    }

    private void RecycleScenario()
    {
        if (!changingEnvironment)
        {
            recycle_scenario(atual_scenario[scenarioIndex].gameObject, Mathf.RoundToInt(ScenarioRecycleSize), currentEnvironmentX);
        }

        scenarioIndex = NextIndex(scenarioIndex, atual_scenario.Count);
        currentScenarioPoint = GetPlatformPoint(atual_scenario, scenarioIndex, "cenario");

        if (scenarioIndex == 0 && changingEnvironment)
        {
            changingEnvironment = false;
        }
    }

    private int NextIndex(int currentIndex, int count)
    {
        currentIndex++;
        return currentIndex > count - 1 ? 0 : currentIndex;
    }

    private void TryChangeEnvironment(float scenarioDistance)
    {
        if (nextEnvironmentIndex >= environmentSteps.Count || scenarioDistance < ScenarioRecycleDistance || scenarioIndex != ScenarioGroupSize - 1)
        {
            return;
        }

        EnvironmentStep nextEnvironment = environmentSteps[nextEnvironmentIndex];
        if (ui.distancia < nextEnvironment.distanceToStart)
        {
            return;
        }

        ChangeEnvironment(nextEnvironment);
        nextEnvironmentIndex++;
    }

    private void ChangeEnvironment(EnvironmentStep environment)
    {
        if (!HasScenarioGroup(environment.firstScenarioIndex))
        {
            Debug.LogWarning("Grupo de cenario '" + environment.name + "' incompleto na lista Scenario.");
            return;
        }

        changingEnvironment = true;
        currentEnvironmentX = environment.xPosition;

        for (int i = 0; i < ScenarioGroupSize; i++)
        {
            atual_scenario.RemoveAt(0);
            int prefabIndex = environment.firstScenarioIndex + i;
            Transform newScenario = Instantiate(scenario[prefabIndex], new Vector3(currentEnvironmentX, GroundDebugOffset, offset_scenario - 10f), transform.rotation).transform;
            newScenario.name = environment.name + i;
            atual_scenario.Add(newScenario);
            offset_scenario += Mathf.RoundToInt(ScenarioRecycleSize);
        }
    }

    private bool HasScenarioGroup(int firstIndex)
    {
        return scenario.Count >= firstIndex + ScenarioGroupSize;
    }

    private void UpdateObstacleSpawnDelay()
    {
        if (!var.troca_dificuldade)
        {
            return;
        }

        CancelInvoke(nameof(SpawnObstacle));
        currentSpawnDelay = GetSpawnDelayForDifficulty(var.dificuldade);
        InvokeRepeating(nameof(SpawnObstacle), 0f, currentSpawnDelay);
        var.troca_dificuldade = false;
    }

    private void UpdateDistanceDifficulty()
    {
        int distanceDifficulty = GetDifficultyForDistance(ui.distancia);
        if (distanceDifficulty <= lastDistanceDifficulty || distanceDifficulty <= var.dificuldade)
        {
            return;
        }

        lastDistanceDifficulty = distanceDifficulty;
        var.dificuldade = distanceDifficulty;
        var.troca_dificuldade = true;
    }

    private int GetDifficultyForDistance(float distance)
    {
        if (distance >= expertDistance)
        {
            return 4;
        }

        if (distance >= hardDistance)
        {
            return 3;
        }

        if (distance >= normalDistance)
        {
            return 2;
        }

        return 1;
    }

    private float GetSpawnDelayForDifficulty(int difficulty)
    {
        if (difficulty <= 1)
        {
            return 1f;
        }

        if (difficulty == 2)
        {
            return 0.75f;
        }

        return 0.5f;
    }

    private void SpawnObstacle()
    {
        if (obstacles.Count == 0 || spawn_points.Count == 0 || var == null)
        {
            return;
        }

        List<float> availableSpawnPoints = new List<float>(spawn_points);
        GameObject selectedObstacle = obstacles[Random.Range(0, obstacles.Count)];
        int obstacleCount = GetObstacleCountAndAdvanceZ(var.dificuldade);

        SpawnObstacleGroup(selectedObstacle, obstacleCount, availableSpawnPoints);
        SpawnCoins();
    }

    private int GetObstacleCountAndAdvanceZ(int difficulty)
    {
        if (difficulty <= 1)
        {
            obstacles_z += Mathf.RoundToInt(Mathf.Max(15f, minimumObstacleSpacing));
            return 1;
        }

        if (difficulty == 2)
        {
            obstacles_z += Mathf.RoundToInt(Mathf.Max(13f, minimumObstacleSpacing));
            return 1;
        }

        if (difficulty == 3)
        {
            obstacles_z += Mathf.RoundToInt(Mathf.Max(11f, minimumObstacleSpacing));
            return Random.Range(1, Mathf.Min(3, MaxAllowedObstacleCount()) + 1);
        }

        obstacles_z += Mathf.RoundToInt(Mathf.Max(9f, minimumObstacleSpacing));
        return Random.Range(1, MaxAllowedObstacleCount() + 1);
    }

    private int MaxAllowedObstacleCount()
    {
        int lanes = Mathf.Max(1, spawn_points.Count);
        return Mathf.Clamp(maxBlockedLanes, 1, Mathf.Max(1, lanes - 1));
    }

    private void SpawnObstacleGroup(GameObject selectedObstacle, int obstacleCount, List<float> availableSpawnPoints)
    {
        string obstacleName = selectedObstacle.name;

        for (int i = 0; i < obstacleCount; i++)
        {
            if (!TrySelectSpawnPoint(availableSpawnPoints, out float spawnX))
            {
                return;
            }

            GameObject obstacleToSpawn = GetObstacleForSlot(selectedObstacle, obstacleName, i);
            if (obstacleToSpawn == null)
            {
                continue;
            }

            float xOffset = GetObstacleXOffset(obstacleToSpawn.name, i);
            Transform obstacleTransform = obstacleToSpawn.transform;
            Instantiate(obstacleToSpawn, new Vector3(spawnX + xOffset, obstacleTransform.position.y, obstacles_z), obstacleTransform.rotation);
        }
    }

    private GameObject GetObstacleForSlot(GameObject selectedObstacle, string selectedObstacleName, int slotIndex)
    {
        if (ShouldUseWarningObstacle(selectedObstacleName, slotIndex))
        {
            return FindObstacle("traffic_warning");
        }

        if (var.dificuldade == 4 && slotIndex > 1 && expertObstacleSwaps.TryGetValue(selectedObstacleName, out string swapName))
        {
            return FindObstacle(swapName);
        }

        return selectedObstacle;
    }

    private bool ShouldUseWarningObstacle(string obstacleName, int slotIndex)
    {
        bool wideObstacle = obstacleName == "wall" || obstacleName == "desert_temple" || obstacleName == "street_seller";
        if (!wideObstacle)
        {
            return false;
        }

        int allowedWideObstacles = var.dificuldade <= 2 ? 1 : 2;
        return slotIndex >= allowedWideObstacles;
    }

    private float GetObstacleXOffset(string obstacleName, int slotIndex)
    {
        switch (obstacleName)
        {
            case "traffic_pot":
            case "trashcan":
            case "wood_pot":
            case "hydrant":
                return -0.16f;
            case "wall":
                return 0.65f;
            case "street_seller":
                return 0.1f;
            case "traffic_warning":
                return -0.9f;
            default:
                return 0f;
        }
    }

    private GameObject FindObstacle(string obstacleName)
    {
        GameObject obstacle = obstacles.Find(item => item.name == obstacleName);
        if (obstacle == null)
        {
            Debug.LogWarning("Obstaculo '" + obstacleName + "' nao encontrado na lista Obstacles.");
        }

        return obstacle;
    }

    private bool TrySelectSpawnPoint(List<float> availableSpawnPoints, out float spawnX)
    {
        spawnX = 0f;
        if (availableSpawnPoints.Count == 0)
        {
            return false;
        }

        int pointIndex = Random.Range(0, availableSpawnPoints.Count);
        spawnX = availableSpawnPoints[pointIndex];
        availableSpawnPoints.RemoveAt(pointIndex);
        return true;
    }

    private void SpawnCoins()
    {
        if (coin_prefab == null || spawn_coin == null || spawn_coin.Length < 3)
        {
            return;
        }

        int coinAmount = Random.Range(0, 5);
        float coinZ = obstacles_z + 2f;
        int coinX = Random.Range(spawn_coin[0], spawn_coin[2]);
        Transform coinTransform = coin_prefab.transform;

        for (int i = 0; i <= coinAmount; i++)
        {
            Instantiate(coin_prefab, new Vector3(coinX, coinTransform.position.y, coinZ), coinTransform.rotation);
            coinZ += 1.5f;
        }
    }
}
