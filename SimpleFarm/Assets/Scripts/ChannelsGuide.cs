using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleFarmNamespace;

//Guia de Canales

public class ChannelsGuide : MonoBehaviour {

    //State and Identity

    public bool Online; //If Online
    public bool Ready; //If all component are ready
    public bool ReadyInstantiate; // If ready instantiate
    public bool MoveActive; // If move is active
    public string ProgramsContainer;

    //Screens
    public string Mode;
    private GameObject ChGuide;
    private GameObject SingleView;

    //Navigation

    private ETVServerManager ServerScript;

    public List<Canal> Canales;
    public List<Canal> CanalesFromBD
    {
        get
        {
            return Canales;
        }

        set
        {
            if (!(Canales == value))
            {
                Canales = value;
            }
        }
    }

    public List<Programa> Programacion;
    public List<Programa> Progch;
    public List<GameObject> ProgAct;

    public ProgramScript SingleChannel;

    public string First;
    public GameObject FirstSelected;

    public GameObject ActualHolder;
    public GameObject ActualSelected;
    public GameObject ActualAncestor
    {
        get
        {
            return ActualSelected;
        }

        set
        {
            if (Mode == "guide")
            {
                if (ActualSelected.name != value.name)
                {
                    ActualSelected = value;
                    GameObject.Find("Canvas/ChannelsGuide/InfoChannelPanel/Info/ProgramName/Text").GetComponent<Text>().text = ActualSelected.GetComponent<ProgramScript>().Name;
                    GameObject.Find("Canvas/ChannelsGuide/InfoChannelPanel/Info/Description/Text").GetComponent<Text>().text = ActualSelected.GetComponent<ProgramScript>().Info;
                }
            }
        }
    }

    public int CurrentColumn, HorNavIndex, VerNavIndex, NumChannels, NumPrograms, ActualHour;

    private double JumpHor, JumpVer;

    //Remote Control

    public int Action;
    public int ActionFromBD
    {
        get
        {
            return Action;
        }

        set
        {
            Action = value;

            switch (Action)
            {
                case 1:
                    StartCoroutine(CR_CheckNextSelectable("down"));
                    break;

                case 2:
                    StartCoroutine(CR_CheckNextSelectable("up"));
                    break;

                case 3:
                    StartCoroutine(CR_CheckNextSelectable("right"));
                    break;

                case 4:
                    StartCoroutine(CR_CheckNextSelectable("left"));
                    break;

                case 5:

                    if (NumCanal == 0)
                    {
                        if (ActualSelected != null)
                        {
                            if (ActualHour == CurrentColumn)
                            {
                                ActualHolder = ActualSelected;
                                ChangeToSingleView();
                            }
                            else
                                SetRC(0);
                        }
                    }
                    else
                    {
                        
                        GameObject channelsTemp = GameObject.Find(ProgramsContainer);
                        ProgramScript[] prog;
                        for (int i = 1; i <= channelsTemp.transform.childCount; i++)
                        {
                            if (i == (NumCanal - 1))
                            {
                                prog = channelsTemp.GetComponentsInChildren<ProgramScript>();
                                ActualHolder = ActualSelected;
                                ChangeToChannel(prog[Hour], NumCanal);
                            }
                        }
                    }
    
                    break;

                case 6:

                    BackToGuide();

                    break;

                case 7:

                    FilterChannels(NumCanal);

                    break;
            }


        }
    }

    public int NumCanal;
    public int NumCanalFromBD
    {
        get {
            return NumCanal;
        }

        set
        {
            if(NumCanal != value)
            {
                NumCanal = value;
            }
        }
    }
    
    //Clock

    public int Hour;
    public int HourC
    {
        get
        {
            return Hour;
        }

        set
        {
            if (AuxFunctions.ValueChanged("hour", Hour, value))
            {
                Hour = value;
            }
        }
    }

    //UI
    private EventSystem EvSys;

    private Text Clock;
    private Text Date;

    private Text Clock2;
    private Text Date2;

    private GameObject SVgranja;
    private GameObject SVgalpon;
    private GameObject SVsilo;

    private Text NumeroCanal;
    private Text NombreCanal;
    private Text NombrePrograma;

    private Scrollbar ChHorScbr;
    private Scrollbar ChVerScbr;

    //Start APP

    private void Update()
    {  
        if(Mode == "guide")
        {
            Clock.text = DateTime.Now.ToString("HH:mm:ss");
            Date.text = DateTime.Now.ToString("D");
        }     
        
        if(Mode == "channel")
        {
            Clock2.text = DateTime.Now.ToString("HH:mm:ss");
            Date2.text = DateTime.Now.ToString("D");
        }

        if (!EvSys.currentSelectedGameObject)
        {
            if (ActualSelected)
            {
                EvSys.SetSelectedGameObject(ActualSelected);
                ActualSelected.GetComponent<Selectable>().Select();
            }
        }
    }

    private void Start()
    {
        Play();
    }

    public void Play() {
        StartCoroutine(CR_Initialize());
    }

    public void Stop()
    {
        ReadyInstantiate = false;
        Ready = false;
    }

    private IEnumerator CR_Initialize()
    {

        InitializeValues();

        ServerScript.Play();

        yield return new WaitUntil(() => ServerScript.Ready);

        StartCoroutine(CR_Instantiate());

        yield return new WaitUntil(() => ReadyInstantiate);

        Focus(First);

        yield return new WaitForSeconds(0.5f);

        HourC = DateTime.Now.Hour;

        yield return new WaitForSeconds(0.2f);

        SetProgramationToHour(Hour);

        yield return new WaitForSeconds(0.2f);

        Ready = true;

        print("Loading Complete");
    }

    void InitializeValues() {

        //Scripts and Editor Elements

        ProgramsContainer = "ChannelsContainerGrid";
        ServerScript = GameObject.Find("Controller").GetComponent<ETVServerManager>();

        ChGuide = GameObject.Find("ChannelsGuide");

        SingleView = GameObject.Find("SingleView");

        Clock = GameObject.Find("Clock/Text").GetComponent<Text>();
        Date = GameObject.Find("Date/Text").GetComponent<Text>();

        Clock2 = GameObject.Find("Hora/Text").GetComponent<Text>();
        Date2 = GameObject.Find("Fecha/Text").GetComponent<Text>();

        NumeroCanal = GameObject.Find("SingleView/NumCanal").GetComponent<Text>();
        NombreCanal = GameObject.Find("SingleView/NombreCanal").GetComponent<Text>();
        NombrePrograma = GameObject.Find("SingleView/NombrePrograma").GetComponent<Text>();

        

        Hour = 12;

        Mode = "guide";

        //Single View

        SVgranja = GameObject.Find("SingleView/Granja");
        SVgalpon = GameObject.Find("SingleView/Galpon");
        SVsilo = GameObject.Find("SingleView/Silo");

        SingleView.SetActive(false);

        //State & Identity

        Online = false;
        Ready = false;
        ReadyInstantiate = false;
        MoveActive = false;

        //Navigation

        Action = 0;
        NumCanal = 0;

        Canales = new List<Canal>();
        Programacion = new List<Programa>();
        ProgAct = new List<GameObject>();
        
        EvSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        ChVerScbr = GameObject.Find("ChVertical-scbr").GetComponent<Scrollbar>();
        ChHorScbr = GameObject.Find("ChHorizontal-scbr").GetComponent<Scrollbar>();

        HorNavIndex = 1;
        VerNavIndex = 1;
        CurrentColumn = 12;
        ActualHour = 12;
        JumpHor = 0.04760598f;
        JumpVer = 0.0f;

    }

    public void Focus(string objName)
    {
        ActualSelected = GameObject.Find(objName);
        if (EvSys.currentSelectedGameObject)
        {
            EvSys.SetSelectedGameObject(null);
        }
        EvSys.SetSelectedGameObject(ActualSelected);
        ActualSelected.GetComponent<Selectable>().Select(); 
    }

    public void ActivateUISingleView(bool state)
    {
        SVgranja.SetActive(state);
        SVgalpon.SetActive(state);
        SVsilo.SetActive(state);
    }

    public void ChangeView(int type)
    {
        switch (type)
        {
            case 1:

                SVgranja.SetActive(true);
                SVgalpon.SetActive(false);
                SVsilo.SetActive(false);

                NumeroCanal.color = AuxFunctions.ConvertColorRGBA(52, 52, 52, 1);
                NombreCanal.color = AuxFunctions.ConvertColorRGBA(52, 52, 52, 1);
                NombrePrograma.color = AuxFunctions.ConvertColorRGBA(52, 52, 52, 1);

                break;

            case 2:

                SVgranja.SetActive(false);
                SVgalpon.SetActive(true);
                SVsilo.SetActive(false);

                NumeroCanal.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);
                NombreCanal.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);
                NombrePrograma.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);

                break;

            case 3:

                SVgranja.SetActive(false);
                SVgalpon.SetActive(false);
                SVsilo.SetActive(true);

                NumeroCanal.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);
                NombreCanal.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);
                NombrePrograma.color = AuxFunctions.ConvertColorRGBA(255, 255, 255, 1);

                break;

        }
    }

    public void BackToGuide()
    {
        ActivateUISingleView(true);

        SingleView.SetActive(false);

        ChGuide.SetActive(true);

        Mode = "guide";

        Focus(ActualHolder.transform.parent.gameObject.transform.GetChild(0).name);
        SetProgramationToHour(Hour);

        SetRC(0);
    }

    public void ChangeToSingleView()
    {
        int type;
        ChGuide.SetActive(false);
        SingleView.SetActive(true);
        
        Mode = "channel";
        type = ActualSelected.transform.parent.parent.parent.GetComponent<ChannelScript>().type;
        ChangeView(type);

        NumeroCanal.text = ActualSelected.GetComponent<ProgramScript>().ProgramID.ToString();
        NombreCanal.text = ActualSelected.transform.parent.parent.parent.GetComponent<ChannelScript>().Name;
        NombrePrograma.text = ActualSelected.GetComponent<ProgramScript>().Name;

        SetRC(0);
    }

    public void ChangeToChannel(ProgramScript info, int NumCh)
    {
        ChGuide.SetActive(false);
        SingleView.SetActive(true);

        NumeroCanal = GameObject.Find("SingleView/NumCanal").GetComponent<Text>();
        NombreCanal = GameObject.Find("SingleView/NombreCanal").GetComponent<Text>();
        NombrePrograma = GameObject.Find("SingleView/NombrePrograma").GetComponent<Text>();

        ChangeView(Canales[NumCh - 1].tipo);

        NumeroCanal.text = NumCh.ToString();
        NombreCanal.text = Canales[NumCh - 1].nombre;
        NombrePrograma.text = info.Name;

        Mode = "channel";

        SetRC(0);
    }

    public void FilterChannels(int category)
    {
        GameObject container = GameObject.Find("ChannelsContainerGrid");
        string newfirst = "";
        bool band = false;
        int total = container.transform.childCount;

        Mode = "filter";

        for (int j = 0; j< total; j++)
        {
            container.transform.GetChild(j).gameObject.SetActive(true);
        }

        for (int i = 0; i < total; i++)
        {
            if(category != 0)
            {
                if (container.transform.GetChild(i).gameObject.GetComponent<ChannelScript>().type != category)
                {
                    container.transform.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    if (!band)
                    {
                        newfirst = container.transform.GetChild(i).name;
                        band = true;
                    }
                }
            }else
            {
                if(container.transform.GetChild(i).gameObject.activeSelf == false)
                {
                    container.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        if (category != 0)
            Focus(GameObject.Find(newfirst + "/Programs/ProgramsGrid").transform.GetChild(0).name);
        else
            Focus(First);

        SetProgramationToHour(Hour);

        SetRC(0);
    }

    public void Move(string direction)
    {
        if (!MoveActive)
            StartCoroutine(CR_Move(direction));
    }  

    private IEnumerator CR_Instantiate()
    {
        int it1, it2;
        NumChannels = Canales.Count();

        for(it1 = 0; it1 < NumChannels; it1++)
        {
            GameObject prefabCanal = Resources.Load("Prefabs/ProgramationGuide/Channel") as GameObject;
            GameObject CopyCanal = Instantiate(prefabCanal, GameObject.Find("ChannelsContainerGrid").transform);
            CopyCanal.name = "Canal" + it1;
            CopyCanal.GetComponentInChildren<Text>().text = Canales[it1].nombre;
            
            CopyCanal.GetComponent<ChannelScript>().id = Canales[it1].id;
            CopyCanal.GetComponent<ChannelScript>().Name = Canales[it1].nombre;
            CopyCanal.GetComponent<ChannelScript>().type = Canales[it1].tipo;

            switch (Canales[it1].tipo)
            {
                case 1:
                    CopyCanal.GetComponentInChildren<Image>().color = AuxFunctions.ConvertColorRGBA(244, 173, 135, 1);
                    //CopyCanal.GetComponentInChildren<Text>().color = AuxFunctions.ConvertColorRGBA(78, 78, 78, 1);
                    break;

                case 2:
                    CopyCanal.GetComponentInChildren<Image>().color = AuxFunctions.ConvertColorRGBA(9, 173, 135, 1);
                    break;
            }


            CopyCanal.transform.Find("Programs").GetComponent<ScrollRect>().horizontalScrollbar = ChHorScbr;

            Programacion.Clear();

            ServerScript.GetProgramation(it1,"instantiate");

            yield return new WaitWhile(() => Programacion.Count() == 0);

            NumPrograms = Programacion.Count();

            for (it2 = 0; it2 < NumPrograms; it2++)
            {
                GameObject prefabProgram = Resources.Load("Prefabs/ProgramationGuide/Program") as GameObject;
                GameObject CopyProgram = Instantiate(prefabProgram, GameObject.Find("Canvas/ChannelsGuide/ProgramationPanel/ChannelsContainer/ChannelsContainerGrid/" + CopyCanal.name + "/Programs/ProgramsGrid").transform);
                CopyProgram.name = "Canal-" + it1 + "-Program-" + it2;
                CopyProgram.GetComponentInChildren<ProgramScript>().Position = it2;
                CopyProgram.GetComponentInChildren<ProgramScript>().ProgramID = it2;
                CopyProgram.GetComponentInChildren<ProgramScript>().Name = "Programa" + it2;
                CopyProgram.GetComponentInChildren<ProgramScript>().Info = "Info" + it2 + ":" + "Esta es una descripcion del programa";
                CopyProgram.GetComponentInChildren<Text>().text = Programacion[it2].nombre;

                if ( it1 == 0 && it2 == 0)
                {
                    First = CopyProgram.name;
                }
            }
        }

        JumpVer = (double)1 / (NumChannels - 8);
        
        ReadyInstantiate = true;

        print("Ready instantiate");

        yield break;
    }

    public void SetProgramationToHour(int hour)
    {
        double aux;

        CurrentColumn = hour;
        ActualHour = hour;
        aux = JumpHor * hour;
        ChHorScbr.value = (float)aux;

        for (int i = 1; i <= hour; i++)
        {
            ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().name);
            EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().Select();
            EvSys.SetSelectedGameObject(ActualSelected);
        }
    }

    public IEnumerator CR_CheckNextSelectable(string direction)
    {
        if (EvSys.currentSelectedGameObject != null)
        {
            switch (direction)
            {
                case "right":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
                    {
                        Move("right");
                    }

                    break;

                case "left":

                    if (CurrentColumn > ActualHour) {
                        if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft())
                        {
                            Move("left");
                        }
                    }

                    break;

                case "up":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp())
                    {
                        Move("up");
                    }

                    break;

                case "down":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown())
                    {
                        Move("down");
                    }

                    break;
            }

            yield return new WaitWhile(() => MoveActive == true);
        }

        SetRC(0);

        yield break;
    }

	public IEnumerator CR_Move(string direction) {

        double aux;
        MoveActive = true;

        if (ActualSelected != null)
        {
            EvSys.SetSelectedGameObject(ActualSelected);

            switch (direction)
            {
                case "right":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
                    {
                        HorNavIndex++;
                        CurrentColumn++;
                        ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().Select();
                        EvSys.SetSelectedGameObject(ActualSelected);

                        if (HorNavIndex > 3)
                        {
                            aux = ChHorScbr.value + JumpHor;

                            if (ChHorScbr.value != 1.0f)
                            {
                                StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                            }
                            HorNavIndex = 3;
                        }
                    }
                    
                    break;

                case "left":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft())
                    {
                        HorNavIndex--;
                        CurrentColumn--;
                        ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().Select();
                        EvSys.SetSelectedGameObject(ActualSelected);

                        if (HorNavIndex < 1)
                        {
                            aux = ChHorScbr.value - JumpHor;
                            if (ChHorScbr.value != 0.0f)
                            {
                                StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                            }

                            HorNavIndex = 1;
                        }
                    }
                        
                    break;

                case "down":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown())
                    {
                        VerNavIndex++;
                        ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
                        EvSys.SetSelectedGameObject(ActualSelected);

                        if (VerNavIndex > 8)
                        {
                            aux = ChVerScbr.value - JumpVer;
                            if (ChVerScbr.value != 0.0f)
                            {
                                StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                            }
                            VerNavIndex = 8;
                        }

                    }

                    break;

                case "up":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp())
                    {
                        VerNavIndex--;
                        ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().Select();
                        EvSys.SetSelectedGameObject(ActualSelected);


                        if (VerNavIndex < 1)
                        {
                            aux = ChVerScbr.value + JumpVer;
                            if (ChVerScbr.value != 1.0f)
                            {
                                StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                            }

                            VerNavIndex = 1;
                        }
                    }
                    break;

            }
        }

        MoveActive = false;

        yield break;
    }

    public IEnumerator CR_LerpFloatTo(float end, float atime, string direction) // Moves scrollbar value to previous or next page
    {
        
        for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / atime)
        {
            if (direction == "right" || direction == "left")
            {
                ChHorScbr.value = Mathf.Lerp(ChHorScbr.value, end, t);
            }

            if (direction == "up" || direction == "down")
            {
                ChVerScbr.value = Mathf.Lerp(ChVerScbr.value, end, t);
            }

            yield return null;
        }

        if (ChHorScbr.value > 0.99f)
        {
            ChHorScbr.value = 1.0f;
        }

        if (ChHorScbr.value < 0.011f)
        {
            ChHorScbr.value = 0.0f;
        }

        yield break;
    }

    //Server Talk

    public void SetRC(int accion)
    {
        StartCoroutine(ETVServerManager.CR_SendToServer("insertar-accion", accion, 0));
    }
}
