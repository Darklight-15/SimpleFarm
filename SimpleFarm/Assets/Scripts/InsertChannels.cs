using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertChannels : MonoBehaviour {

    public int NumChannels;
    public int NumPrograms;

    public int rowID;
    public int channelID;
    public string channelName;

    public int programID;
    public string programName;
    public string programInfo;
    public int programPosition;



    private IEnumerator Insert()
    {
        NumChannels = 1;
        NumPrograms = 24;

        channelID = 0;
        channelName = "Granja A";
        for (int i = 0; i < 24; i++)
        {
            rowID = i + 1;
            programID = i + 1;
            programName = "Canal " + (i + 1);
            programInfo = "Info " + (i + 1);
            programPosition = i;
            InsertChannel();
            yield return new WaitForSeconds(1.0f);
        }
        
    }
    public void Start () {
        StartCoroutine(Insert());
	}

    public void InsertChannel()
    {
        WWWForm form = new WWWForm();

        form.AddField("rowID", rowID);
        form.AddField("channelID", channelID);
        form.AddField("channelName", channelName);

        form.AddField("programID", programID);
        form.AddField("programName", programName);
        form.AddField("programInfo", programInfo);
        form.AddField("programPosition", programPosition);

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarProgramacion.php", form);
    }

}
