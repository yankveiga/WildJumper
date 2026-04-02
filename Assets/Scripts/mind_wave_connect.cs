using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mind_wave_connect : MonoBehaviour
{
    private MindwaveDataModel m_MindwaveData;
    private MindwaveDataModel _Data;
    public Text TMPText;
    public bool Control;

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("r"))
        {
            Reconnect();
            Control = false;
        }

    }
    IEnumerator Started()
    {
        yield return new WaitForSeconds(5);
        if (m_MindwaveData.eegPower.delta > 0)
        {
            TMPText.text = "Connected";
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(3);
            SceneManager.UnloadSceneAsync(2);
        }
        if (m_MindwaveData.eegPower.delta == 0)
        {
            TMPText.text = "Can't connect\nAperte 'R' para tentar novamente";
        }
    }
    public void OnUpdateMindwaveData(MindwaveDataModel _Data)
    {
        m_MindwaveData = _Data;
    }
    public void Reconnect()
    {  
        MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
        TMPText.text = "Tentando novamente conectar com o MindWave";
        StartCoroutine(Started());
    }
}