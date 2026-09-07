using UnityEngine;
using UnityEngine.UI;

public class ui_controller : MonoBehaviour
{
    public static int LastDistance;
    public static int LastCoins;
    public static int BestDistance;

    public player_controller var;

    // Pontuacao
    public float distancia;
    public Text distancia_text;
    public int coin;
    public Text text_coin;
    public Text text_lives;
    public Text text_best_distance;

    // UI
    public GameObject hearth;
    public Canvas canvas;

    // Feedback
    public Text feedback_text;
    public float feedbackDuration = 1.2f;

    // Mindwave
    public Text attencion;
    public Text meditation;
    private mind_wave mw_var;
    private bool mind_on = false;
    private float feedbackTimer;

    void Start()
    {
        BestDistance = PlayerPrefs.GetInt("BestDistance", 0);
        RefreshScoreText();
        RefreshBestText();
    }

    void Update()
    {
        if (var == null)
        {
            return;
        }

        UpdateDistance();
        TryEnableMindwaveHud();
        UpdateLives();
        UpdateMindwaveHud();
        UpdateFeedback();
        SaveLastScore();
    }

    public void add_coin()
    {
        coin++;
        LastCoins = coin;
        RefreshScoreText();
        ShowFeedback("+1 moeda");
    }

    public void ShowFeedback(string message)
    {
        if (feedback_text == null)
        {
            return;
        }

        feedback_text.text = message;
        feedback_text.enabled = true;
        feedbackTimer = feedbackDuration;
    }

    private void UpdateDistance()
    {
        if (var.player_dead)
        {
            return;
        }

        distancia += Time.deltaTime * var.vel;
        LastDistance = Mathf.RoundToInt(distancia);

        if (LastDistance > BestDistance)
        {
            BestDistance = LastDistance;
            PlayerPrefs.SetInt("BestDistance", BestDistance);
            RefreshBestText();
        }

        RefreshScoreText();
    }

    private void RefreshScoreText()
    {
        if (distancia_text != null)
        {
            distancia_text.text = Mathf.Round(distancia).ToString() + "m";
        }

        if (text_coin != null)
        {
            text_coin.text = coin.ToString();
        }
    }

    private void RefreshBestText()
    {
        if (text_best_distance != null)
        {
            text_best_distance.text = "Recorde: " + BestDistance + "m";
        }
    }

    private void TryEnableMindwaveHud()
    {
        if (!Input.GetKeyDown(KeyCode.U))
        {
            return;
        }

        GameObject mindObject = GameObject.FindWithTag("Mind");
        if (mindObject == null)
        {
            mind_on = false;
            ShowFeedback("MindWave nao encontrado");
            return;
        }

        mw_var = mindObject.GetComponent<mind_wave>();
        mind_on = mw_var != null;
        ShowFeedback(mind_on ? "MindWave conectado ao HUD" : "MindWave nao encontrado");
    }

    private void UpdateLives()
    {
        if (var.vida_add)
        {
            var.vida++;
            var.vida_add = false;
            ShowFeedback("Vida extra!");
        }

        if (var.vida_remove)
        {
            var.vida_remove = false;
            var.vida--;
            ShowFeedback(var.vida > 0 ? "Cuidado!" : "Fim de jogo");
        }

        if (text_lives != null)
        {
            text_lives.text = var.vida.ToString();
        }
    }

    private void UpdateMindwaveHud()
    {
        if (!mind_on || mw_var == null)
        {
            return;
        }

        if (attencion != null)
        {
            attencion.text = mw_var.Attention.ToString();
        }

        if (meditation != null)
        {
            meditation.text = mw_var.Meditation.ToString();
        }
    }

    private void UpdateFeedback()
    {
        if (feedback_text == null || !feedback_text.enabled)
        {
            return;
        }

        feedbackTimer -= Time.deltaTime;
        if (feedbackTimer <= 0f)
        {
            feedback_text.enabled = false;
        }
    }

    private void SaveLastScore()
    {
        LastDistance = Mathf.RoundToInt(distancia);
        LastCoins = coin;
    }
}