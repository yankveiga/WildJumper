using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameover_screen : MonoBehaviour
{
    public Text distance_text;
    public Text coins_text;
    public Text best_distance_text;

    void Start()
    {
        if (distance_text != null)
        {
            distance_text.text = "Distancia: " + ui_controller.LastDistance + "m";
        }

        if (coins_text != null)
        {
            coins_text.text = "Moedas: " + ui_controller.LastCoins;
        }

        if (best_distance_text != null)
        {
            int bestDistance = Mathf.Max(ui_controller.BestDistance, PlayerPrefs.GetInt("BestDistance", 0));
            best_distance_text.text = "Recorde: " + bestDistance + "m";
        }
    }

    public void return_title()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Titlescreen");
    }

    public void restart_game()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
}