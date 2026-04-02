using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player_data : MonoBehaviour
{
    private TittleButton tittle;
    private new string name = "";

    // Acessando as variaveis do script do mind wave
    private mind_wave mind;
    public string cena_gameplay;
    public bool cena_certa = false;
    public string file_name = @"";
    private string diretorio = Application.dataPath;
    private string cena_atual;
    private string teste;
    private player_controller player;
    private ui_controller ui;


    // Start is called before the first frame update
    void Start()
    {
        tittle = GameObject.Find("TitleScreen").GetComponent<TittleButton>();
        mind = GameObject.FindWithTag("Mind").GetComponent<mind_wave>();
        cena_atual = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (tittle.on_end_edit)
        {
            name = tittle.name_player.text;
            tittle.on_end_edit = false;
        }

        if (cena_atual == cena_gameplay && !cena_certa)
        {
            StartCoroutine(MakeData(Create_File()));
            cena_certa = true;
        }
    }

    // Transformar em objeto persistente
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Configurando o diretorio e o nome do arquivo
    public string Create_File()
    {
        // Se a variável name estiver vazia renomeie ela
        if(name == "")
        {
            name = "sem_nome";
        }
        file_name += diretorio + @"\Data\" + name.ToString() + ".csv";

        // Se tiver um arquivo com o mesmo nome na pasta não sobreescrever
        string pasta = diretorio + @"\Data\";
        if (Directory.Exists(pasta))
        {
            string[] arquivos = Directory.GetFiles(pasta);
            foreach(string arquivo in arquivos)
            {
                if(file_name == arquivo)
                {
                    name += "_";
                    file_name = diretorio + @"\Data\" + name.ToString() + ".csv";
                }
            }
        }
        string file = Headers(file_name);
        return file;
    }
    public string Headers(string FileName)
    {
        string file = FileName;
        string[] HeaderText = {"Atencion Level, Meditation Level, Blink, Life, Distance, Speed, Time played, Difficulty"};
        File.WriteAllLines(file, HeaderText);
        return file;
    }

    IEnumerator MakeData(string file)
    {
        while(cena_atual == cena_gameplay)
        {
            yield return new WaitForSeconds(1);
            if (Time.timeScale != 0)
            {
                float attention = mind_wave.sAttention;
                float meditation = mind_wave.sMeditation;
                int blink = mind_wave.blink;
                float life = player.vida;
                float distance = ui.distancia;
                float speed = player.vel;
                float time = player.tempo;
                int difficulty = player.dificuldade;
                DataRecord(file, attention, meditation, blink, life, distance, speed, time, difficulty);
            }
        }
    }
    public void DataRecord(string file, float AT, float ML, int BK, float LF, float DS, float SP, float TM, int DF)
    {
        DS = Mathf.FloorToInt(DS);
        TM = Mathf.FloorToInt(TM);
        string DC = "easy";
        
        if(DF == 1)
        {
            DC = "easy";
        }
        else if(DF == 2)
        {
            DC = "normal";
        }
        else if(DF == 3)
        {
            DC = "hard";
        }
        else
        {
            DC = "expert";
        }

        string DataForWrite = Convert.ToString(AT) + "," + Convert.ToString(ML) + "," + Convert.ToString(BK) + "," + Convert.ToString(LF) + 
                        "," + Convert.ToString(DS) + "," + Convert.ToString(SP) + "," + Convert.ToString(TM) + "," + DC + "\n";
        File.AppendAllText(file, DataForWrite);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        cena_atual = scene.name;
        if(cena_atual == cena_gameplay && !cena_certa)
        {
            player = GameObject.Find("Player").GetComponent<player_controller>();
            ui = GameObject.Find("ui_controller").GetComponent<ui_controller>();
        }
    }
}
