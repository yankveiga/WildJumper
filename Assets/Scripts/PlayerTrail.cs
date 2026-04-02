using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public GameObject trailPrefab; // Prefab do rastro
    public float trailInterval = 0.1f; // Intervalo entre a criação de cada clone do jogador
    public float trailLifetime = 1.5f; // Tempo de vida de cada clone do jogador
    public int maxTrailCount = 20; // Número máximo de clones do jogador que podem existir simultaneamente

    private float trailTimer = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Atualiza o temporizador
        trailTimer += Time.deltaTime;

        // Se o temporizador exceder o intervalo desejado, crie um novo clone do jogador
        if (trailTimer >= trailInterval)
        {
            trailTimer = 0f;

            // Cria um novo clone do jogador
            GameObject trailClone = Instantiate(trailPrefab, transform.position, transform.rotation);

            // Define a posição e a escala do clone do jogador
            trailClone.transform.position = transform.position;
            trailClone.transform.localScale = transform.localScale;

            // Inicia a contagem regressiva para destruir o clone do jogador
            Destroy(trailClone, trailLifetime);

            // Limita o número total de clones do jogador na cena
            if (GameObject.FindGameObjectsWithTag("PlayerTrail").Length > maxTrailCount)
            {
                Destroy(GameObject.FindGameObjectsWithTag("PlayerTrail")[0]);
            }
        }
    }
}
