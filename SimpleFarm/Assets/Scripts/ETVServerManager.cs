using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFarmNamespace;

public class ETVServerManager : MonoBehaviour
{
    //State & Identity

    public bool Online;
    public bool OnlineFromBD
    {
        get
        {
            return Online;
        }

        set
        {
            if (Online != value)//Change ui online status
            {
                Online = value;

                ChGhideScript.Online = value;

                if (value)
                {
                    OnlineUI.text = "Online";
                    AuxFunctions.ChangeColor("Canvas/Online", AuxFunctions.ConvertColorRGBA(103, 221, 127, 1), 1f);
                }
                else
                {
                    OnlineUI.text = "Offline";
                    AuxFunctions.ChangeColor("Canvas/Online", AuxFunctions.ConvertColorRGBA(247, 125, 115, 1), 1f);
                }
            }
        }
    }

    public bool Ready;
    public bool ReadyCanales;
    public bool ReadyProgramation;

    public int NumChannels;
    public int NumPrograms;
    
    //Scripts

    ChannelsGuide ChGhideScript;

    //Remote Control

    List<Canal> ListCanales;
    public string Canales;
    public string CanalesFromBD
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

    public string ProgramacionFromBD;
    List<Programa> ListProgramas;

    public int Action;
    public int ActionFromBD
    {
        get
        {
            return Action;
        }

        set
        {
            if (Action != value)
            {
                Action = value;
                ChGhideScript.ActionFromBD = Action;
            }
        }
    }

    public int NumCanal;
    public int NumCanalFromBD
    {
        get
        {
            return NumCanal;
        }

        set
        {
            if(NumCanal != value)
            {
                NumCanal = value;
                ChGhideScript.OptionValueFromBD = NumCanal;
            }
        }
    }
    
    //UI

    private Text OnlineUI;

    //Start APP

    private void Update()
    {
        //print("Server Action: " + Action);
    }

    public void Play()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        InitializeValues();

        StartCoroutine(CR_GetOnline());

        print("Waiting for Online");
        yield return new WaitUntil(() => Online == true);

        StartCoroutine(CR_GetChannels());

        yield return new WaitUntil(() => ReadyCanales);

        //StartCoroutine(Insert());        
        //yield return new WaitUntil(() => ReadyInsertProgramation);

        StartCoroutine(CR_GetAction());

        Ready = true;

        print("Ready Server");

        yield break;
    }

    public void InitializeValues()
    {
        Online = false;
        Ready = false;
        ReadyCanales = false;
        ReadyProgramation = false;

        ChGhideScript = GameObject.Find("Controller").GetComponent<ChannelsGuide>();

        StartCoroutine(CR_SendToServer("insertar-accion", 0, 0));

        Canales = "";
        NumChannels = 12;
        NumPrograms = 24;
        
        Action = 0;
        NumCanal = 0;

        ListCanales = new List<Canal>();
        ListProgramas = new List<Programa>();

        //UI Values

        OnlineUI = GameObject.Find("Online/Text").GetComponent<Text>();
        OnlineUI.text = "Offline";

        //ChannelIDInsert = 0;
        //ReadyInsertProgramation = false;
        //ReadyInsertCanal = false;
    }
    
    public void ParseCanales()
    {
        string[] items;

        items = CanalesFromBD.Split(';');

        for (int i = 0; i < items.Length - 1; i++)
        {
            ListCanales.Add(new Canal(int.Parse(AuxFunctions.GetDataValue(items[i], "id:")), AuxFunctions.GetDataValue(items[i], "nombre:"), int.Parse(AuxFunctions.GetDataValue(items[i], "tipo:") ) ) );
        }

        ChGhideScript.CanalesFromBD = ListCanales;
    }

    public void ParseProgramacion(string programacion, string type)
    {
        string[] items;

        items = programacion.Split(';');

        if(ListProgramas.Count > 0)
            ListProgramas.Clear();

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

        if (type == "instantiate")
        {
            ChGhideScript.Programacion = ListProgramas;
        }
            
        if(type == "channel")
        {
            ChGhideScript.Progch = ListProgramas;
        }
    }

    //Server Talk
    
    private IEnumerator CR_GetOnline()
    {
        WWW OnlineStatus;

        while (true)
        {
            OnlineStatus = new WWW(Macros.SitePath);

            yield return OnlineStatus;

            if (OnlineStatus.error == null)
            {
                OnlineFromBD = true;
            }
            else
            {
                OnlineFromBD = false;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator CR_GetChannels()
    {
        WWWForm form = new WWWForm();

        form.AddField("opcion", "leer-canales");

        WWW www = new WWW(Macros.SitePath + "/ControlRemoto.php", form);

        yield return www;

        if (www.error == null)
            CanalesFromBD = www.text;

        ReadyCanales = true;

        yield break;
    }

    public void GetProgramation(int CanalID, string type)
    {
        ReadyProgramation = false;
        StartCoroutine(CR_GetProgramation(CanalID, type));
    }

    private IEnumerator CR_GetProgramation(int CanalID, string type)
    {
        WWWForm form = new WWWForm();

        form.AddField("opcion", "leer-programacion");
        form.AddField("canalProgramacion", CanalID);

        WWW www = new WWW(Macros.SitePath + "/ControlRemoto.php", form);

        yield return www;

        if (www.error == null)
        {
            ParseProgramacion(www.text, type);
        }
        else
            print("Instantiate programation error");

        yield return new WaitForSeconds(0.1f);

        ReadyProgramation = true;

        yield break;
    }

    private IEnumerator CR_GetAction()
    {
        WWWForm form;
        WWW ActionQuery;
        string[] DatosControl;

        form = new WWWForm();
        form.AddField("opcion", "leer-accion");

        while (true)
        {
            ActionQuery = new WWW(Macros.SitePath + "/ControlRemoto.php", form);

            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return ActionQuery;

            if (ActionQuery.error == null)
            {
                DatosControl = ActionQuery.text.ToString().Split(new string[] { "-" }, StringSplitOptions.None);
                NumCanalFromBD = int.Parse(DatosControl[1]);
                ActionFromBD = int.Parse(DatosControl[0]);
            }
            else
            {
                print("Server Error (Remote Control): (" + ActionQuery.error + ")");
            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    static public IEnumerator CR_SendToServer(string option, int action, int value)
    {
        //Send to server: accion(0=inner / 1=down / 2=up / 3=right / 4=left / 5=ok)
        //flag(1=true / 0=false)
        //leer-accion
        WWW www;
        WWWForm form;

        form = new WWWForm();
        form.AddField("opcion", option);
        form.AddField("value", action);
        form.AddField("numerocanal", value);

        www = new WWW(Macros.SitePath + "/ControlRemoto.php", form);

        yield return www;

        if (www.error == null)
        {
            //print("Successful Insert: (" + option + " : " + action + ")");
        }
        else
        {
            print("Error inserting row");
        }

        yield break; 
    }

    /*
    public int ChannelID;
    public string ChannelName;
    public int rowID;
    public int programID;
    public string programName;
    public string programInfo;
    public int programDuration;
    public int programPosition;
    public int ChannelIDInsert;
    bool ReadyInsertCanal;
    public string[] stringArray;
    private IEnumerator InsertCanal()
    {
        string aux;
        ReadyInsertCanal = false;
        WWWForm form = new WWWForm();

        form.AddField("opcion", "canales");

        for (int i = 0; i < stringArray.Length; i++)
        {
            form.AddField("canalData[]", stringArray[i]);
        }

        WWW www = new WWW("https://bhutan-slave.000webhostapp.com/insertarProgramacion.php", form);
        yield return www;

        aux = www.text;

        ReadyInsertCanal = true;

        yield break;
    }
    private IEnumerator Insert()
    {
        //Canales
        List<string> L;
        L = new List<string>();
        for (int i = 0; i < 12; i++)
        {
            ChannelID = (i + 1);
            ChannelName = "Canal" + (i + 1);
            L.Add("Canal" + (i + 1));
        }
        stringArray = L.ToArray();

        StartCoroutine(InsertCanal());
        yield return new WaitUntil(() => ReadyInsertCanal);

        //Programacion
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
        WWW www = new WWW("https://bhutan-slave.000webhostapp.com/insertarProgramacion.php", form);

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
    */
}
