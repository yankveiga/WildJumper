using UnityEngine;
using UnityEngine.SceneManagement;

public class player_controller : MonoBehaviour
{
    [Header("Movimento")]
    public float jumpForce = 8.5f;
    public float gravity = 22f;
    public float vel = 5f;
    public float sideSpeed = 5.5f;
    public float sideAcceleration = 14f;
    public float maxSidePosition = 2.2f;
    public float speedSmooth = 2f;

    [Header("Pulo")]
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;
    public float fallGravityMultiplier = 1.35f;

    [Header("Colisoes")]
    public LayerMask layer;
    public LayerMask layer_coletavel;
    public float raio_colisao = 0.45f;
    public Vector3 obstacleCheckOffset = new Vector3(0f, 0.75f, 0.25f);
    public Vector3 coinCheckOffset = new Vector3(0f, 0.65f, 0.2f);
    public float hitInvincibilityTime = 1.0f;

    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip coinSound;
    public AudioClip gameOverSound;

    public Animator anim_player;
    public bool player_dead = false;
    public int vida;
    public bool vida_add = false;
    public bool vida_remove = false;
    public float tempo;
    public int media_attention;
    public int dificuldade = 1;
    public bool troca_dificuldade = false;

    private CharacterController controller;
    private ui_controller ui_control;
    private mind_wave mind;
    private Vector3 moveDirection = Vector3.zero;
    private float currentSideSpeed;
    private float targetForwardSpeed = 5f;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float hitInvincibilityCounter;
    private int coins = 0;
    private int media_meditation;
    private int timer_max = 20;
    private int timer = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim_player = GetComponent<Animator>();
        ui_control = FindObjectOfType<ui_controller>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        GameObject mindObject = GameObject.FindWithTag("Mind");
        if (mindObject != null)
        {
            mind = mindObject.GetComponent<mind_wave>();
        }

        vida = Mathf.Max(vida, 3);
        targetForwardSpeed = Mathf.Max(targetForwardSpeed, vel);
        InvokeRepeating(nameof(chance_difficult), 1f, 1f);
    }

    void Update()
    {
        if (controller == null || player_dead)
        {
            tempo = Time.time;
            return;
        }

        UpdateTimers();
        UpdateMovement();
        colisao();

        tempo = Time.time;
    }

    private void UpdateTimers()
    {
        if (controller.isGrounded)
        {
            coyoteCounter = coyoteTime;
            if (moveDirection.y < 0f)
            {
                moveDirection.y = -1f;
            }
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (hitInvincibilityCounter > 0f)
        {
            hitInvincibilityCounter -= Time.deltaTime;
        }
    }

    private void UpdateMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetSideSpeed = horizontalInput * sideSpeed;
        currentSideSpeed = Mathf.MoveTowards(currentSideSpeed, targetSideSpeed, sideAcceleration * Time.deltaTime);

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            moveDirection.y = jumpForce;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            SetAnimationBool("jump", true);
            PlaySound(jumpSound);
        }

        float gravityThisFrame = moveDirection.y < 0f ? gravity * fallGravityMultiplier : gravity;
        moveDirection.y -= gravityThisFrame * Time.deltaTime;
        moveDirection.x = currentSideSpeed;
        vel = Mathf.Lerp(vel, targetForwardSpeed, speedSmooth * Time.deltaTime);
        moveDirection.z = vel;

        controller.Move(moveDirection * Time.deltaTime);

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, -maxSidePosition, maxSidePosition);
        transform.position = position;

        if (controller.isGrounded && moveDirection.y <= 0f)
        {
            SetAnimationBool("jump", false);
        }
    }

    void colisao()
    {
        if (!player_dead && hitInvincibilityCounter <= 0f && TryFindObstacle(out RaycastHit hit))
        {
            TakeHit(hit.transform.gameObject);
        }

        if (TryFindCoin(out RaycastHit coin))
        {
            CollectCoin(coin.transform.gameObject);
        }
    }

    private bool TryFindObstacle(out RaycastHit hit)
    {
        Vector3 origin = transform.position + obstacleCheckOffset;
        return Physics.SphereCast(origin, raio_colisao, transform.forward, out hit, 0.85f, layer, QueryTriggerInteraction.Collide);
    }

    private bool TryFindCoin(out RaycastHit coin)
    {
        Vector3 origin = transform.position + coinCheckOffset;
        return Physics.SphereCast(origin, raio_colisao * 1.4f, transform.forward, out coin, 1.15f, layer_coletavel, QueryTriggerInteraction.Collide);
    }

    private void TakeHit(GameObject obstacle)
    {
        hitInvincibilityCounter = hitInvincibilityTime;
        vida_remove = true;
        PlaySound(hitSound);

        if (vida <= 1)
        {
            vel = 0f;
            targetForwardSpeed = 0f;
            currentSideSpeed = 0f;
            jumpForce = 0f;
            player_dead = true;
            SetAnimationBool("death", true);
            PlaySound(gameOverSound);
            Invoke(nameof(game_over), 2.5f);
        }
        else
        {
            Destroy(obstacle);
            SetAnimationBool("hit", true);
            Invoke(nameof(hit_end), 0.23f);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        if (ui_control != null)
        {
            ui_control.add_coin();
        }

        coins++;
        PlaySound(coinSound);

        if (coins >= 100)
        {
            vida_add = true;
            coins = 0;
        }

        Destroy(coin);
    }

    private void SetAnimationBool(string parameter, bool value)
    {
        if (anim_player != null)
        {
            anim_player.SetBool(parameter, value);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void hit_end()
    {
        SetAnimationBool("hit", false);
    }

    public void finish_line_player()
    {
        vel = 0f;
        targetForwardSpeed = 0f;
        currentSideSpeed = 0f;
        jumpForce = 0f;
        player_dead = true;
        SetAnimationBool("finish", true);
    }

    public void game_over()
    {
        SceneManager.LoadScene("Gameover");
    }

    void chance_difficult()
    {
        timer++;

        int attention = mind != null ? (int)mind.Attention : 35;
        int meditation = mind != null ? (int)mind.Meditation : 35;
        media_attention += attention;
        media_meditation += meditation;

        if (timer < timer_max || player_dead)
        {
            return;
        }

        int averageAttention = (media_attention / timer_max) / 10;
        int averageMeditation = (media_meditation / timer_max) / 10;
        int newDifficulty = Mathf.Clamp(Mathf.CeilToInt(((averageAttention + averageMeditation) / 2f) / 2.5f), 1, 4);

        targetForwardSpeed = 5f + averageAttention * 0.35f;
        sideSpeed = 4.8f + averageMeditation * 0.18f;

        if (dificuldade != newDifficulty)
        {
            dificuldade = newDifficulty;
            troca_dificuldade = true;
        }

        timer = 0;
        media_attention = 0;
        media_meditation = 0;
    }
}