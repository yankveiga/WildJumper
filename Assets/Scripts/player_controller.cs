using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.Port;

public class player_controller : MonoBehaviour {

    // Variáveis publicas
    private float vel_move = 3.5f;
    public float jumpForce = 8f;
    public float gravity = 20f;
    //padrão 5f
    public float vel = 5f;

    // Referência ao CharacterController
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;

    // Variáveis de animação
    public Animator anim_player;
    public bool player_dead = false;
    public LayerMask layer;
    public LayerMask layer_coletavel;
    public float raio_colisao;

    // Variáveis de controle do player
    private bool exec_jump = false;
    public int vida;
    public bool vida_add = false;
    public bool vida_remove = false;
    public float tempo;
    private int coins = 0;
    public int media_attention;
    private int media_meditation;
    private float tempo_media;

    // Controle da Interface de Usuário
    private ui_controller ui_control;
    private mind_wave mind;
    
    // Variáveis de controle de tempo e dificuldade
    private float intervalo = 10f;
    private float time_atual = 0f;
    public int dificuldade = 1;
    public bool troca_dificuldade = false;
    private float vel_nova = 5f;

    private int timer_max = 20;
    private int timer = 0;

    void Start () {
        controller = GetComponent<CharacterController>();
        anim_player = GetComponent<Animator>();
        ui_control = FindObjectOfType<ui_controller>();
        mind = GameObject.FindWithTag("Mind").GetComponent<mind_wave>();
        vida = 3;
        InvokeRepeating("chance_difficult", 1f, 1f);
    }

    void Update()
    {
        // Verifica se o jogador está no chão
        if (controller.isGrounded)
        {
            // Obtém o input do movimento horizontal (esquerda/direita)
            float horizontalInput = Input.GetAxis("Horizontal");

            // Cria um vetor de movimento com base no input
            moveDirection = new Vector3(horizontalInput, 0, 0);

            // Multiplica pelo valor da velocidade
            moveDirection *= vel_move;

            // Verifica se o jogador pressionou o botão de pulo
            if (Input.GetKeyDown(KeyCode.Space) && !exec_jump)
            {
                // Adiciona uma força vertical ao vetor de movimento
                moveDirection.y = jumpForce;
                anim_player.SetBool("jump", true);
                exec_jump = true;
            }
            else if (exec_jump)
            {
                exec_jump = false;
                anim_player.SetBool("jump", false);
            }
        }

        /*
        // Controle da velocidade horizontal e vertical de acordo com os dados do mind wave
        if (time_atual >= intervalo && !player_dead)
        {
            // Atualizando a velocidade vertical de acordo com a media dos valores do mind wave pelo intervalo de 10s
            media_attention = media_attention / (int)Mathf.Round(intervalo);
            vel_nova = 5f + media_attention * 0.6f;

            // Atualizando a velocidade vertical de acordo com a media dos valores do mind wave pelo intervalo de 10s
            media_meditation = media_meditation / (int)Mathf.Round(intervalo);
            vel_move = 3.5f + media_meditation * 0.25f;

            // Alterando a dificuldade do jogo de acordo com os valores do mind wave
            int soma = Mathf.CeilToInt(((media_attention + media_meditation) / 25) / 2);
            if (soma != 0 && dificuldade != soma)
            {
                dificuldade = soma;
                troca_dificuldade = true;
            }

            // Resetar o tempo para entrar na condicional novamente
            media_attention = 0;
            media_meditation = 0;
            time_atual = 0f;
        }
        else if (tempo_media >= 1)
        {
            int attention = Mathf.FloorToInt(mind.Attention);
            int meditation = Mathf.FloorToInt(mind.Meditation);
            media_attention += attention;
            media_meditation += meditation;
            tempo_media = 0;
        }
        */

        // Aplica a gravidade ao vetor de movimento
        moveDirection.y -= gravity * Time.deltaTime;
        moveDirection.z = vel;

        // Move o jogador com base no vetor de movimento
        controller.Move(moveDirection * Time.deltaTime);
        vel = Mathf.Lerp(vel, vel_nova, 0.1f);
        tempo = Time.time;
        time_atual += Time.deltaTime;
        tempo_media += Time.deltaTime;
        colisao();

    }

    // Desenhar as esferas de colisão do RaycastHit
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(Vector3.forward + new Vector3(0, 0.8f, -0.6f)), raio_colisao);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(Vector3.forward + new Vector3(0, 0.6f, -1f)), raio_colisao*2);
        */
    }

    void colisao()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, raio_colisao, transform.TransformDirection(Vector3.forward + new Vector3(0, 0.8f, -0.6f)), out hit, raio_colisao * 2, layer) && !player_dead && vida <= 1)
        {
            vel = 0;
            vel_move = 0;
            jumpForce = 0;
            player_dead = true;
            vida_remove = true;
            anim_player.SetBool("death", true);
            Invoke("game_over", 5.0f);
        }
        else if (Physics.SphereCast(transform.position, raio_colisao, transform.TransformDirection(Vector3.forward + new Vector3(0, 0.8f, -0.6f)), out hit, raio_colisao * 2, layer) && !player_dead)
        {
            vida_remove = true;
            Destroy(hit.transform.gameObject);
            anim_player.SetBool("hit", true);
            Invoke("hit_end", .23f);
        }
        
        RaycastHit coin;

        if (Physics.SphereCast(transform.position, raio_colisao*2, transform.TransformDirection(Vector3.forward + new Vector3(0, 0.6f, -1f)), out coin, raio_colisao * 2, layer_coletavel))
        {
            ui_control.add_coin();
            coins++;
            if(coins >= 100)
            {

                vida_add = true;
                coins = 0;
            }
            Destroy(coin.transform.gameObject);
        }
        else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward + new Vector3(0, 0.6f, 0.3f)), out coin, raio_colisao * 2, layer_coletavel))
        {
            ui_control.add_coin();
            coins++;
            if (coins >= 100)
            {

                vida_add = true;
                coins = 0;
            }
            Destroy(coin.transform.gameObject);
        }
    }

    // Animação de quando o player leva hit
    void hit_end()
    {
        anim_player.SetBool("hit",false);
    }


    // Animação de final do jogo
    public void finish_line_player()
    {
        vel = 0;
        vel_move = 0;
        jumpForce = 0;
        player_dead = true;
        anim_player.SetBool("finish",true);
    }

    public void game_over(){
        SceneManager.LoadScene("Gameover");
    }
    
    void chance_difficult()
    {
        timer++;
        int attention = (int)mind.Attention;
        int meditation = (int)mind.Meditation;
        media_attention += attention;
        media_meditation += meditation;
        if(timer >= timer_max && !player_dead)
        {
            media_attention = (media_attention / timer_max) / 10;
            vel_nova = 5f + media_attention * 0.6f;
            media_meditation = (media_meditation / timer_max) / 10;
            vel_move = 3.5f + media_meditation * 0.25f;
            int soma = Mathf.CeilToInt(((media_attention + media_meditation) / 2) / 2.5f);
            timer = 0;
            media_attention = 0;
            media_meditation = 0;
            dificuldade = soma;
            troca_dificuldade = true;
        }
    }
}