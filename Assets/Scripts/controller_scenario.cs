using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class controller_scenario : MonoBehaviour
{
    // Váriaveis da plataforma
    public List<GameObject> platforms = new List<GameObject>();
    public GameObject finish_line;
    public List<Transform> atual_platforms = new List<Transform>();
    public float tam_rua = 32;

    // Váriaveis de ambiente
    public List<GameObject> scenario = new List<GameObject>();
    public List<Transform> atual_scenario = new List<Transform>();
    public int offset;
    public int offset_scenario;
    public float tam_sce = 150;
    private float ambient_x;
    private Transform player;
    private Transform point_plataforma_atual;
    private Transform point_finish_line;
    private Transform point_scenario_atual;
    private int index;
    private int index_scenario;
    private float debug = 0.0001f;
    public player_controller var;

    // Váriavel de Controle do ambiente
    public ui_controller ui;
    private string ambient = "florest";
    private bool troca_ambient = false;

    // Criando os obstáculos
    public List<GameObject> obstacles; // Array contendo os obstáculos pré-selecionados
    public List<float> spawn_points = new List<float> {-1.4f,0f,1,4f}; // Pontos de spawn para os obstáculos
    public float difficultyIncreaseRate = 10f; // Taxa de aumento da dificuldade
    public float initialSpawnDelay = 1f; // Tempo inicial entre o instanciamento de obstáculos
    public float spawnDelayReduction = 0.2f; // Redução do tempo entre o instanciamento de obstáculos ao aumentar a dificuldade
    private float currentSpawnDelay; // Tempo entre o instanciamento de obstáculos
    private int obstacles_z = 125;
    private int obstacles_number;
    private float obstacles_x;
    private GameObject selected_obstacle;

    // Criando as moedas
    public GameObject coin_prefab;
    public int[] spawn_coin;

    // Start is called before the first frame update
    void Start()
    {
        // Procurando a posição atual do player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        var = GameObject.FindGameObjectWithTag("Player").GetComponent<player_controller>();

        // Instanciando as plataformas
        for(int i = 0; i < platforms.Count; i++)
        {
            Transform p = Instantiate(platforms[i], new Vector3(0,0,i * tam_rua), transform.rotation).transform;
            atual_platforms.Add(p);
            offset += 32;
        }

        // Instanciando o ambiente
        for(int i = 0; i < 3; i++)
        {
            Transform ambient_ = Instantiate(scenario[i], new Vector3(0,debug,i * tam_sce), transform.rotation).transform;
            ambient_.transform.name = "florest"+i.ToString();
            atual_scenario.Add(ambient_);
            offset_scenario += 150;
        }

        // Pegando a posição da plataforma atual onde o player esta
        point_plataforma_atual = atual_platforms[index].GetComponent<platforms>().point;
        point_scenario_atual = atual_scenario[index_scenario].GetComponent<platforms>().point;

        Transform finish = Instantiate(finish_line, new Vector3(0,debug,10000), transform.rotation).transform;
        finish.transform.Rotate(new Vector3(0, 90, 0));
        point_finish_line = finish.GetComponent<platforms>().point;

        // Define o tempo inicial entre o instanciamento de obstáculos
        currentSpawnDelay = initialSpawnDelay;

        // Inicia a chamada do método para instanciar obstáculos
        InvokeRepeating("SpawnObstacle", initialSpawnDelay, currentSpawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        // Armazenando a diferen�a da posi��o entra player e plataforma atual **quando for zero a plataforma � destruida**
        float distance = player.position.z - point_plataforma_atual.position.z;
        float distance_finish = player.position.z - point_finish_line.position.z;
        float distance_scenario = player.position.z - point_scenario_atual.position.z;

        if (distance >= 5)
        {
            recycle(atual_platforms[index].gameObject,32);
            index++;

            if(index > atual_platforms.Count - 1)
            {
                index = 0;
            }

            point_plataforma_atual = atual_platforms[index].GetComponent<platforms>().point;
        }

        // Animação da linha de chegada
        if(distance_finish >= 0){
            var.finish_line_player();
        }

        // Teste da mudança de ambiente
        if(distance_scenario >= 25){
            if(!troca_ambient){
                recycle_scenario(atual_scenario[index_scenario].gameObject,145,ambient_x);
            }
            index_scenario++;

            if(index_scenario > atual_scenario.Count - 1)
            {
                index_scenario = 0;
            }
            point_scenario_atual = atual_scenario[index_scenario].GetComponent<platforms>().point;
        }

        if(index_scenario == 0 && troca_ambient)
        {
            troca_ambient = false;
        }

        switch(ambient)
        {
            case "florest":
                if(ui.distancia >= 1400 && distance_scenario >= 25)
                {
                    if(index_scenario == 2)
                    {   
                        troca_ambient = true;
                        ambient_x = 0;
                        for(int i = 3; i < 6; i++)
                        {
                            atual_scenario.RemoveAt(0);
                            Transform ambient_ = Instantiate(scenario[i], new Vector3(ambient_x,debug,offset_scenario-5), transform.rotation).transform;
                            ambient_.transform.name = "desert"+(i-3).ToString();
                            atual_scenario.Add(ambient_);
                            offset_scenario += 145;
                        }  
                        ambient = "desert";          
                    }
                }
            break;

            case "desert":
                if(ui.distancia >= 3400 && distance_scenario >= 25)
                {
                    if(index_scenario == 2)
                    {   
                        troca_ambient = true;
                        ambient_x = 0f;
                        for(int i = 6; i < 9; i++)
                        {
                            atual_scenario.RemoveAt(0);
                            Transform ambient_ = Instantiate(scenario[i], new Vector3(ambient_x,debug,offset_scenario-5), transform.rotation).transform;
                            ambient_.transform.name = "halloween"+(i-3).ToString();
                            atual_scenario.Add(ambient_);
                            offset_scenario += 145;
                        }  
                        ambient = "halloween";         
                    }
                }
            break;

            case "halloween":
                if(ui.distancia >= 5400 && distance_scenario >= 25)
                {
                    if(index_scenario == 2)
                    {   
                        troca_ambient = true;
                        ambient_x = 10.7f;
                        for(int i = 9; i < 12; i++)
                        {
                            atual_scenario.RemoveAt(0);
                            Transform ambient_ = Instantiate(scenario[i], new Vector3(ambient_x,debug,offset_scenario-5), transform.rotation).transform;
                            ambient_.transform.name = "city"+(i-3).ToString();
                            atual_scenario.Add(ambient_);
                            offset_scenario += 145;
                        }  
                        ambient = "city";         
                    }
                }
            break;

            case "city":
                if(ui.distancia >= 7400 && distance_scenario >= 25)
                {
                    if(index_scenario == 2)
                    {   
                        troca_ambient = true;
                        ambient_x = 0f;
                        for(int i = 12; i < 15; i++)
                        {
                            atual_scenario.RemoveAt(0);
                            Transform ambient_ = Instantiate(scenario[i], new Vector3(ambient_x,debug,offset_scenario-5), transform.rotation).transform;
                            ambient_.transform.name = "mountain"+(i-3).ToString();
                            atual_scenario.Add(ambient_);
                            offset_scenario += 145;
                        }  
                        ambient = "mountain";         
                    }
                }
            break;

            case "mountain":
                Debug.Log("aq");
            break;
        }

        // Quando a dificuldade do jogo for alterada, mude o tempo de spawn dos obstaculos
        if(var.troca_dificuldade)
        {
            if(var.dificuldade == 1)
            {
                CancelInvoke("SpawnObstacle");
                currentSpawnDelay = 1f;
                // Inicia novamente as chamadas repetidas com o novo tempo entre instanciamentos
                InvokeRepeating("SpawnObstacle", 0f, currentSpawnDelay);
            }
            else if(var.dificuldade == 2)
            {
                CancelInvoke("SpawnObstacle");
                currentSpawnDelay = 0.75f;
                // Inicia novamente as chamadas repetidas com o novo tempo entre instanciamentos
                InvokeRepeating("SpawnObstacle", 0f, currentSpawnDelay);
            }
            else
            {
                CancelInvoke("SpawnObstacle");
                currentSpawnDelay = 0.5f;
                // Inicia novamente as chamadas repetidas com o novo tempo entre instanciamentos
                InvokeRepeating("SpawnObstacle", 0f, currentSpawnDelay);
            }
            var.troca_dificuldade = false;
        }
    }

    // Função para reciclagem das plataformas
    public void recycle(GameObject plataforma, int range)
    {
        plataforma.transform.position = new Vector3(0, 0, offset);
        offset += range;
    }

    // Função para reciclagem das plataformas
    public void recycle_scenario(GameObject plataforma, int range, float x)
    {
        plataforma.transform.position = new Vector3(x, debug, offset_scenario);
        offset_scenario += range;
    }

    void SpawnObstacle() {
        // Lista das posições definidas para instanciar os obstaculos
        List<float> spawn_points_alt = new List<float>();
        spawn_points_alt.AddRange(spawn_points);
        select_point(spawn_points_alt);

        // Selecionando o obstaculo
        var random_obstacle = Random.Range(0,obstacles.Count);
        selected_obstacle = obstacles[random_obstacle];
        var selected_obstacle_name = selected_obstacle.name;
        Debug.Log(random_obstacle);
        Debug.Log(selected_obstacle.name);

        // Numero de obstaculos e Espaçamento dos obstáculos
        if(var.dificuldade == 1)
        {
            obstacles_number = 1;
            obstacles_z += 15;
        }
        else if(var.dificuldade == 2)
        {
            obstacles_number = Random.Range(1, var.dificuldade);
            obstacles_z += 12;
        }
        else if (var.dificuldade == 3)
        {
            obstacles_number = Random.Range(1, var.dificuldade);
            obstacles_z += 9;
        }
        else{
            obstacles_number = Random.Range(2, var.dificuldade-1);
            obstacles_z += 7;
        }
        
        // State Machine
        switch(selected_obstacle_name){
            // Obstaculos pulaveis
            case "traffic_pot":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "traffic"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "traffic":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "traffic pot"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_pot");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "pot_tree":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "bench"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "bench");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "trash":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "trashcan"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "trashcan");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "trashcan":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "trash"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "trash");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "wood_pot":
                for (int j = 0; j < obstacles_number; j++)
                {
                    Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                    Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "hydrant":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "traffic"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.16f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "wall":
                for (int j = 0; j < obstacles_number; j++)
                {
                    if ((var.dificuldade <= 2 && j < 1) || (var.dificuldade >= 3 && j < 2))
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x + 0.65f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else if (var.dificuldade <= 2)
                    {
                        // Cria um "traffic warning" no lugar do "desert temple" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        // Cria um "traffic warning" no lugar do "desert temple" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    select_point(spawn_points_alt);
                }
                break;

            case "bench":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "pot_tree"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "pot_tree");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "gravestone":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "pumpkin"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "pumpkin");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

             case "desert_column":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "ram sharp"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "ram_sharp");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

             case "desert_temple":
                for (int j = 0; j < obstacles_number; j++)
                {
                    if((var.dificuldade <= 2 && j < 1) || (var.dificuldade >= 3 && j < 2))
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else if (var.dificuldade <= 2)
                    {
                        // Cria um "traffic warning" no lugar do "desert temple" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        // Cria um "traffic warning" no lugar do "desert temple" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    select_point(spawn_points_alt);
                }
             break;

             case "pumpkin":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "gravestone"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "gravestone");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

             case "ram_sharp":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "desert column"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "desert column");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

             case "skull":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "zombie hand"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "zombie_hand");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

             case "zombie_hand":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "skull"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "skull");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "shopping_cart":
                for (int j = 0; j < obstacles_number; j++)
                {
                    // Se a dificuldade for expert trocar um objeto por "trash"
                    if (var.dificuldade == 4 && j > 1)
                    {
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "trash");
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    // Trocar o ponto de instanciar
                    select_point(spawn_points_alt);
                }
                break;

            case "street_seller":
                for (int j = 0; j < obstacles_number; j++)
                {
                    if ((var.dificuldade <= 2 && j < 1) || (var.dificuldade >= 3 && j < 2))
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x + 0.1f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else if (var.dificuldade <= 2)
                    {
                        // Cria um "traffic warning" no lugar do "street_seller" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    else
                    {
                        // Cria um "traffic warning" no lugar do "street_seller" independente da posição no array
                        GameObject selected_obstacle = obstacles.Find(objeto => objeto.name == "traffic_warning");  // Posição do "traffic warning" na lista de obstaculos
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x - 0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                    }
                    select_point(spawn_points_alt);
                }
                break;

            case "traffic_warning":
                // ALTERAR AQUI POSTERIORMENTE ------------------------------
                if (obstacles_number <= 3)
                {
                    for (int j = 0; j < obstacles_number; j++)
                    {
                        Transform selected_spawn_point = selected_obstacle.GetComponent<Transform>();
                        Instantiate(selected_obstacle, new Vector3(obstacles_x-0.9f, selected_spawn_point.position.y, obstacles_z), selected_spawn_point.rotation);
                        if (j < 2)
                        {
                            select_point(spawn_points_alt);
                        }
                    }
                }
            break;
        }

        // Criando as moedas
        int qntd = Random.Range(0,5);
        float coin_z = obstacles_z+2;
        int coin_x = Random.Range(spawn_coin[0],spawn_coin[2]);
        for(int i = 0; i <= qntd; i++)
        {
            // Seleciona um ponto de spawn aleatoriamente
            Transform selected_coin = coin_prefab.GetComponent<Transform>();

            // Instancia o obstáculo no ponto de spawn selecionado
            Instantiate(selected_coin, new Vector3(coin_x, selected_coin.position.y, coin_z), selected_coin.rotation);
            coin_z += 1.5f;
        }
    }

    void select_point(List<float> spawn_points_alt)
    {
        if(spawn_points_alt.Count != 0)
        {
            obstacles_x = spawn_points_alt[Random.Range(0, spawn_points_alt.Count)];
            spawn_points_alt.Remove(obstacles_x);
        }
    }
}