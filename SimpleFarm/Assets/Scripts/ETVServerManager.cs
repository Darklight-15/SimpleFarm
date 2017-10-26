using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFarmNamespace;
using System.Linq;

public class ETVServerManager : MonoBehaviour
{

    public bool Ready;

    ChannelsGuide ChGuideScrpt;
    
    List<Canal> ListCanales;

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
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        Ready = false;

        Canales = "";
        NumChannels = 1;
        NumPrograms = 24;

        ChGuideScrpt = GameObject.Find("Controller").GetComponent<ChannelsGuide>();
        ListCanales = new List<Canal>();

        ReadyInsertProgramation = false;
        ReadyInsertCanal = false;
        ReadyReadCanales = false;

        StartCoroutine(ReadCanales());

        yield return new WaitUntil(() => ReadyReadCanales);

        Ready = true;

        yield return null;
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
        yield return new WaitUntil(() => ReadyInsertCanal);

        channelID = 1;
        for (int i = 0; i < 24; i++)
        {
            rowID = i + 1;
            programID = i + 1;
            programName = "Program " + (i + 1);
            programInfo = "Info " + (i + 1);
            programPosition = i;
            StartCoroutine(InsertProgramation());
            yield return new WaitUntil(()=>ReadyInsertProgramation);
        }
        
        yield break;
    }

    bool ReadyInsertCanal;
    private IEnumerator InsertCanal()
    {
        string aux;
        ReadyInsertCanal = false;
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
        ReadyInsertCanal = true;
        yield break;
    }

    bool ReadyInsertProgramation;
    public IEnumerator InsertProgramation()
    {
        ReadyInsertProgramation = false;
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

        ReadyInsertProgramation = true;
        yield break;
    }

    public void ParseCanales()
    {
        string[] items;
        items = CanalesFromBD.Split(';');

        for (int i = 0; i < items.Length - 1; i++)
        {
            ListCanales.Add(new Canal(int.Parse(AuxFunctions.GetDataValue(items[i], "id:")), AuxFunctions.GetDataValue(items[i], "nombre:")));
        }

        ChGuideScrpt.CanalesFromBD = ListCanales;
        /*
        List<Canal> aux, aux2;
        aux = new List<Canal>();
        aux2 = new List<Canal>();
        aux.AddRange(ListCanales);
        foreach (var item in ListCanales)
        {
            print(item.id + "/" + item.nombre);
        }
        aux.Add(new Canal(22, "pene"));
        aux2 = aux;
        print(aux == ListCanales);
        */
    }
    private string Canales;
    private string CanalesFromBD
    {
        get
        {
            return Canales;
        }

        set
        {
            if (AuxFunctions.ValueChanged("Canales", Canales, value))
            {
                Canales = value;
                ParseCanales();
            }
        }
    }
    bool ReadyReadCanales;
    private IEnumerator ReadCanales()
    {
        ReadyReadCanales = false;

        WWWForm form = new WWWForm();

        form.AddField("opcion", "canales");

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/RevisarProgramacion.php", form);

        yield return www;

        if(www.error == null)
            CanalesFromBD = www.text;

        //print(Canales);
        ReadyReadCanales = true;
        yield break;
    }
}
