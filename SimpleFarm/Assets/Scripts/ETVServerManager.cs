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
    List<Programa> ListProgramas;

    public int NumChannels;
    public int NumPrograms;

    public string opcion;
    
    //Channel List
    public int ChannelID;
    public string ChannelName;

    //Program Info
    public int rowID;
    public int ChannelIDInsert;

    public int programID;
    public string programName;
    public string programInfo;
    public int programDuration;
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
        ChannelIDInsert = 0;

        ChGuideScrpt = GameObject.Find("Controller").GetComponent<ChannelsGuide>();
        
        ListCanales = new List<Canal>();
        ListProgramas = new List<Programa>();

        ReadyInsertProgramation = false;
        ReadyInsertCanal = false;
        ReadyCanales = false;
        ReadyProgramation = false;

        StartCoroutine(ReadCanales());
        
        yield return new WaitUntil(() => ReadyCanales);

        //StartCoroutine(Insert());

        //yield return new WaitUntil(() => ReadyInsertProgramation);

        Ready = true;
        print("Ready Server");

        yield return null;
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

    private IEnumerator Insert()
    {
        /*
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
        */
        int Lenght = ListCanales.Count();
        rowID = 0;
        programID = 0;
        for (int i1 = 0; i1 < Lenght; i1++)
        {
            for (int i2 = 0; i2 < 24; i2++)
            {
                ChannelIDInsert = i1;

                rowID++;

                programID++;

                programName = "Channel-" + i1 + "-Program " + (i2 + 1);
                programInfo = "Info " + (i2 + 1);
                programDuration = 2;
                programPosition = i2;

                StartCoroutine(InsertProgramation());
                yield return new WaitUntil(() => ReadyInsertProgramation);
                ReadyInsertProgramation = false;
            }
        }

        yield break;
    }
    bool ReadyInsertProgramation;
    public IEnumerator InsertProgramation()
    {
        ReadyInsertProgramation = false;

        WWWForm form = new WWWForm();

        form.AddField("opcion", "programas");

        form.AddField("rowID", rowID);
        form.AddField("channelID", ChannelIDInsert);

        form.AddField("programID", programID);
        form.AddField("programName", programName);
        form.AddField("programInfo", programInfo);
        form.AddField("programDuration", programDuration);
        form.AddField("programPosition", programPosition);

        print("Row: " + rowID + "ChannelID: " + ChannelIDInsert +"/"+ programID);
        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarProgramacion.php", form);

        yield return www;

        if(www.error == null)
        {
            print(www.text);
        }
        yield return new WaitForSeconds(0.1f);
        ReadyInsertProgramation = true;
        print("Insert");
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
    bool ReadyCanales;
    private IEnumerator ReadCanales()
    {
        ReadyCanales = false;

        WWWForm form = new WWWForm();

        form.AddField("opcion", "canales");

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/RevisarProgramacion.php", form);

        yield return www;

        if(www.error == null)
            CanalesFromBD = www.text;

        //print(Canales);
        ReadyCanales = true;
        print("Ready Reading Canales");
        yield break;
    }

    public bool ReadyProgramation;
    public string ProgramacionFromBD;
    public void ParseProgramacion(string programacion)
    {
        string[] items;
        items = programacion.Split(';');

        for (int i = 0; i < items.Length - 1; i++)
        {
            ListProgramas.Add(
                new Programa(
                    int.Parse(AuxFunctions.GetDataValue(items[i], "id:")), 
                    AuxFunctions.GetDataValue(items[i],"nombre:"),
                    AuxFunctions.GetDataValue(items[i], "info:"),
                    int.Parse(AuxFunctions.GetDataValue(items[i], "duracion:")),
                    int.Parse(AuxFunctions.GetDataValue(items[i], "posicion:"))
                )
            );
                
        }

        ChGuideScrpt.Programacion = ListProgramas;

    }
    public void GetProgramas(int CanalID)
    {
        ReadyProgramation = false;
        StartCoroutine(CO_ReadProgramation(CanalID));
    }
    private IEnumerator CO_ReadProgramation(int CanalID)
    {
        ReadyProgramation = false;

        WWWForm form = new WWWForm();

        form.AddField("opcion", "programacion");
        form.AddField("canalProgramacion", CanalID);

        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/RevisarProgramacion.php", form);

        yield return www;

        if (www.error == null)
        {
            ParseProgramacion(www.text);
        }
        else
            print("Error");

        yield return new WaitForSeconds(0.1f);

        //print("Leida programacion de Canal=" + CanalID);

        ReadyProgramation = true;
        
        yield break;
    }


    public bool Up, Down, Right, Left;
    private string RemoteControl;
    private string RemoteControlFromBD
    {
        get
        {
            return RemoteControl;
        }

        set
        {
            if (AuxFunctions.ValueChanged("remote", RemoteControl, value))
            {
                RemoteControl = value;
                ParseRemoteControl();
            }
        }
    }
    private void ParseRemoteControl()
    {
        string[] items;

        items = RemoteControl.Split(';');

        Down = bool.Parse(AuxFunctions.GetDataValue(items[0], "down:"));
        Up = bool.Parse(AuxFunctions.GetDataValue(items[0], "up:"));
        Left = bool.Parse(AuxFunctions.GetDataValue(items[0], "left:"));
        Right = bool.Parse(AuxFunctions.GetDataValue(items[0], "right:"));
    }
    private IEnumerator CR_GetRemoteControl()
    {
        WWW RemoteControlQuery;

        while (true)
        {
            RemoteControlQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarControlRemoto.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return RemoteControlQuery;

            if (RemoteControlQuery.error == null)
            {
                RemoteControlFromBD = RemoteControlQuery.text;
            }
            else
            {
                print("Server Error (Remote Control): (" + RemoteControlQuery.error + ")");
            }

            yield return new WaitForSeconds(1.0f);
        }

    }













}
