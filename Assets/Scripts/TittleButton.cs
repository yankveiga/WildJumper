using System.Collections;
using System.Collections.Generic;
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

    // Tela das configurações
    public GameObject config_obj;
    private bool config_control = false;

    // Tela das instruições
    public GameObject instr_obj;
    private bool instr_control = false;

    // Start is called before the first frame update
    void Start()
    {
        mind = GameObject.FindWithTag("Mind").GetComponent<mind_wave>();
        name_player.onEndEdit.AddListener(OnEndEdit);
    }

    // Update is called once per frame
    void Update()
    {
        if(mind_control && !mind.control){
            mind_text.text = "Aperte o botão para se conectar ao Mind Wave!";
        }

        if(mind.control)
        {
            if(mind.conectado)
            {
                mind_text.text = "Conectado!!";
            }
            else if(!mind.conectado)
            {
                mind_text.text = "Não foi possível conectar-se ao Mind Wave\nTente novamente!";
            }
        }
    }

    public void next_scene()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void mind_open()
    {
        if(!mind_control)
        {
            mind_obj.SetActive(true);
            mind_control = true;
        }
        else
        {
            mind_obj.SetActive(false);
            mind_control = false;
        }
    }

    public void config_open()
    {
        if(!config_control)
        {
            config_obj.SetActive(true);
            config_control = true;
        }
        else
        {
            config_obj.SetActive(false);
            config_control = false;
        }
    }

    public void instr_open()
    {
        if(!instr_control)
        {
            instr_obj.SetActive(true);
            instr_control = true;
        }
        else
        {
            instr_obj.SetActive(false);
            instr_control = false;
        }
    }

    public void mind_connect()
    {
        if(mind_control)
        {
            mind.control = true;
        }
    }

    private void OnEndEdit(string text)
    {
        on_end_edit = true;
    }
}
