using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TittleButton : MonoBehaviour
{
    private mind_wave mind;

    public InputField name_player;
    public bool on_end_edit = false;

    // Tela do Mindwave
    public GameObject mind_obj;
    private bool mind_control = false;
    public Text mind_text;
    public GameObject mind_on;

    // Tela das configuracoes
    public GameObject config_obj;
    private bool config_control = false;

    // Tela das instrucoes
    public GameObject instr_obj;
    private bool instr_control = false;

    void Start()
    {
        GameObject mindObject = GameObject.FindWithTag("Mind");
        if (mindObject != null)
        {
            mind = mindObject.GetComponent<mind_wave>();
        }

        if (name_player != null)
        {
            name_player.onEndEdit.AddListener(OnEndEdit);
        }

        UpdateMindText("Aperte o botao para conectar ao MindWave.");
    }

    void Update()
    {
        if (!mind_control || mind == null)
        {
            return;
        }

        if (!mind.control)
        {
            UpdateMindText("Aperte o botao para conectar ao MindWave.");
            return;
        }

        UpdateMindText(mind.conectado ? "Conectado!" : "Nao foi possivel conectar ao MindWave. Tente novamente.");
    }

    public void next_scene()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void mind_open()
    {
        mind_control = TogglePanel(mind_obj, mind_control);
    }

    public void config_open()
    {
        config_control = TogglePanel(config_obj, config_control);
    }

    public void instr_open()
    {
        instr_control = TogglePanel(instr_obj, instr_control);
    }

    public void mind_connect()
    {
        if (mind_control && mind != null)
        {
            mind.control = true;
        }
        else
        {
            UpdateMindText("MindWave nao encontrado nesta cena.");
        }
    }

    private bool TogglePanel(GameObject panel, bool currentState)
    {
        bool nextState = !currentState;
        if (panel != null)
        {
            panel.SetActive(nextState);
        }

        return nextState;
    }

    private void UpdateMindText(string message)
    {
        if (mind_text != null)
        {
            mind_text.text = message;
        }
    }

    private void OnEndEdit(string text)
    {
        on_end_edit = true;
    }
}