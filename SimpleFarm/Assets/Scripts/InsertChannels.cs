using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertChannels : MonoBehaviour {

    public int NumChannels;
    public int NumPrograms;

    public string opcion;
    
    //Channel List
    public int ChannelID;
    public string ChannelName;

    //Program Info
    public int rowID;
    public int channelID;

    public int programID;
    public string programName;
    public string programInfo;
    public int programPosition;
    public string[] a;

    public void Start()
    {
        ReadyProgramation = false;
        ReadyCanal = false;
        NumChannels = 1;
        NumPrograms = 24;

        StartCoroutine(Insert());
    }

    private IEnumerator Insert()
    {
        
        channelID = 0;
        List<string> L;
        L = new List<string>();
        for (int i = 0; i < 12; i++)
        {
            ChannelID = (i + 1);
            ChannelName = "Canal" + (i + 1);
            L.Add("Canal" + (i + 1));
        }
        a = L.ToArray();

        StartCoroutine(InsertCanal());
        yield return new WaitUntil(() => ReadyCanal);

        channelID = 1;
        for (int i = 0; i < 24; i++)
        {
            rowID = i + 1;
            programID = i + 1;
            programName = "Program " + (i + 1);
            programInfo = "Info " + (i + 1);
            programPosition = i;
            StartCoroutine(InsertProgramation());
            yield return new WaitUntil(()=>ReadyProgramation);
        }
        
        yield break;
    }

    bool ReadyCanal;
    private IEnumerator InsertCanal()
    {
        string aux;
        ReadyCanal = false;
        WWWForm form = new WWWForm();
        form.AddField("opcion", "canales");
        for (int i = 0; i < a.Length; i++)
        {
            form.AddField("canalData[]", a[i]);
        }

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarProgramacion.php", form);
        yield return www;
        aux = www.text;
        print(aux);
        ReadyCanal = true;
        yield break;
    }

    bool ReadyProgramation;
    public IEnumerator InsertProgramation()
    {
        ReadyProgramation = false;
        string aux;

        WWWForm form = new WWWForm();

        form.AddField("opcion", "programas");

        form.AddField("rowID", rowID);
        form.AddField("channelID", channelID);

        form.AddField("programID", programID);
        form.AddField("programName", programName);
        form.AddField("programInfo", programInfo);
        form.AddField("programPosition", programPosition);

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarProgramacion.php", form);

        yield return www;
        aux = www.text;
        print(aux);
        ReadyProgramation = true;
        yield break;
    }

}
