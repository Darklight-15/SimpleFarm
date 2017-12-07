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

    
    //States and Identity

    public bool Online; //If Online
    public bool Ready; //If all component are ready
    public bool ReadyInstantiate; // If ready instantiate
    public bool MoveActive; // If move is active

    //Mode Screens
    public string Mode;
    private GameObject ChGuide;
    private GameObject SingleView;

    //Navigation
    private ETVWindowManager WMScript;
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

    public ProgramScript SingleChannel;

    public string First;
    public string FirstOfPage;
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
                if (value)
                {
                    if (ActualSelected.name != value.name)
                    {
                        ActualSelected = value;
                        GameObject.Find("Canvas/ChannelsGuide/InfoChannelPanel/Info/ProgramName/Text").GetComponent<Text>().text = ActualSelected.GetComponent<ProgramScript>().Name;
                        GameObject.Find("Canvas/ChannelsGuide/InfoChannelPanel/Info/Description/Text").GetComponent<Text>().text = ActualSelected.GetComponent<ProgramScript>().Info;
                    }
                }else
                {
                    SetRC(0);
                }
            }
        }
    }

    public int CurrentPage, TotalPages, CurrentColumn, HorNavIndex, VerNavIndex, NumChannels, NumPrograms, ActualHour;

    private double JumpHor, JumpVer;

    //Remote Control

    public int OptionValue;
    public int OptionValueFromBD
    {
        get
        {
            return OptionValue;
        }

        set
        {
            if (OptionValue != value)
            {
                OptionValue = value;
            }
        }
    }

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

                    if (OptionValue == 0)
                        StartCoroutine(CR_CheckNextSelectable("down"));
                    else
                        StartCoroutine(CR_SkipPage(1));
                    break;

                case 2:
                    if (OptionValue == 0)
                        StartCoroutine(CR_CheckNextSelectable("up"));
                    else
                        StartCoroutine(CR_SkipPage(2));
                    break;

                case 3:
                    StartCoroutine(CR_CheckNextSelectable("right"));
                    break;

                case 4:
                    StartCoroutine(CR_CheckNextSelectable("left"));
                    break;

                case 5:

                    if (OptionValue == 0)
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
                        
                        GameObject channelsTemp = GameObject.Find(Macros.ChannelsContainer);
                        ProgramScript[] prog;
                        for (int i = 1; i <= channelsTemp.transform.childCount; i++)
                        {
                            if (i == (OptionValue - 1))
                            {
                                prog = channelsTemp.GetComponentsInChildren<ProgramScript>();
                                ActualHolder = ActualSelected;
                                ChangeToChannel(prog[Hour], OptionValue);
                            }
                        }
                    }
    
                    break;

                case 6:

                    BackToGuide();

                    break;

                case 7:

                    StartCoroutine(CR_FilterChannel(OptionValue));

                    break;
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
        
        if (Mode == "channel")
        {
            Clock2.text = DateTime.Now.ToString("HH:mm:ss");
            Date2.text = DateTime.Now.ToString("D");
        }
       
        if (EvSys)
        {
            if (!EvSys.currentSelectedGameObject)
            {
                if (ActualSelected)
                {
                    EvSys.SetSelectedGameObject(ActualSelected);
                    ActualSelected.GetComponent<Selectable>().Select();
                }
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

        WMScript = GameObject.Find(Macros.Controller).GetComponent<ETVWindowManager>();
        ServerScript = GameObject.Find(Macros.Controller).GetComponent<ETVServerManager>();

        ChGuide = GameObject.Find(Macros.ChannelsGuide);

        SingleView = GameObject.Find(Macros.SingleView);

        Clock = GameObject.Find("Clock/Text").GetComponent<Text>();
        Date = GameObject.Find("Date/Text").GetComponent<Text>();

        Clock2 = GameObject.Find("Hora/Text").GetComponent<Text>();
        Date2 = GameObject.Find("Fecha/Text").GetComponent<Text>();

        NumeroCanal = GameObject.Find(Macros.SingleView + "/NumCanal").GetComponent<Text>();
        NombreCanal = GameObject.Find(Macros.SingleView +  "NombreCanal").GetComponent<Text>();
        NombrePrograma = GameObject.Find(Macros.SingleView + "/NombrePrograma").GetComponent<Text>();

        Hour = 12;

        Mode = "guide";

        //Single View

        SVgranja = GameObject.Find(Macros.SingleView + "/Granja");
        SVgalpon = GameObject.Find(Macros.SingleView + "/Galpon");
        SVsilo = GameObject.Find(Macros.SingleView + "/Silo");

        SingleView.SetActive(false);

        //State & Identity

        Online = false;
        Ready = false;
        ReadyInstantiate = false;
        MoveActive = false;

        //Navigation

        Action = 0;
        OptionValue = 0;

        Canales = new List<Canal>();
        Programacion = new List<Programa>();
        
        EvSys = GameObject.Find(Macros.EvSystem).GetComponent<EventSystem>();
        ChVerScbr = GameObject.Find("ChVertical-scbr").GetComponent<Scrollbar>();
        ChHorScbr = GameObject.Find("ChHorizontal-scbr").GetComponent<Scrollbar>();

        HorNavIndex = 1;
        VerNavIndex = 1;
        CurrentColumn = 12;
        ActualHour = 12;
        JumpHor = 0.04760598f;
        JumpVer = 0.0f;

    }

    private IEnumerator CR_Instantiate()
    {
        int it1, it2;
        NumChannels = Canales.Count();

        for (it1 = 0; it1 < NumChannels; it1++)
        {
            GameObject prefabCanal = Resources.Load("Prefabs/ProgramationGuide/Channel") as GameObject;
            GameObject CopyCanal = Instantiate(prefabCanal, GameObject.Find("ChannelsContainerGrid").transform);
            CopyCanal.name = "Canal" + it1;
            CopyCanal.GetComponentInChildren<Text>().text = "CH-" + Canales[it1].id + " " + Canales[it1].nombre;

            CopyCanal.GetComponent<ChannelScript>().id = Canales[it1].id;
            CopyCanal.GetComponent<ChannelScript>().Name = Canales[it1].nombre;
            CopyCanal.GetComponent<ChannelScript>().type = Canales[it1].tipo;

            switch (Canales[it1].tipo)
            {
                case 1:
                    CopyCanal.GetComponentInChildren<Image>().color = AuxFunctions.ConvertColorRGBA(244, 173, 135, 1);
                    break;

                case 2:
                    CopyCanal.GetComponentInChildren<Image>().color = AuxFunctions.ConvertColorRGBA(9, 173, 135, 1);
                    break;
            }


            CopyCanal.transform.Find("Programs").GetComponent<ScrollRect>().horizontalScrollbar = ChHorScbr;

            if (Programacion.Count > 0)
                Programacion.Clear();

            ServerScript.GetProgramation(it1, "instantiate");

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

                if (it1 == 0 && it2 == 0)
                {
                    First = CopyProgram.name;
                }
            }
        }

        JumpVer = (double)1 / (NumChannels - 8);

        CurrentPage = 1;
        FirstOfPage = First;

        TotalPages = NumChannels / 8;

        if ((NumChannels % 8) > 0)
        {
            TotalPages++;
        }

        ReadyInstantiate = true;

        print("Ready instantiate");

        yield break;
    }

    //Navigation Functions

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

    public IEnumerator CR_FilterChannel(int category)
    {
        GameObject container = GameObject.Find(Macros.ChannelsContainer);
        string newfirst = "";
        bool band = false;
        int total = container.transform.childCount;

        WMScript.InstanceAndShowWindow("temporal", "BasicPopUpPrefab", "Canvas","Cargando", 2.0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        for (int j = 0; j< total; j++)
        {
            container.transform.GetChild(j).gameObject.SetActive(true);
        }

        for (int i = 0; i < total; i++)
        {
            if(category > 0 && category <= 3)
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
            }
            yield return null;
        }

        if (category > 0 && category <= 3) {
            Focus(GameObject.Find(newfirst + "/Programs/ProgramsGrid").transform.GetChild(0).name);
        }
        else
        {
            Focus(First);
        }
            

        SetProgramationToHour(Hour);

        SetRC(0);

        yield break;
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

                    if (CurrentColumn > ActualHour)
                    {
                        if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft())
                        {
                            Move("left");
                        }
                    }

                    break;

                case "up":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() || CurrentPage == TotalPages)
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

    public void Move(string direction)
    {
        if (!MoveActive)
            StartCoroutine(CR_Move(direction));
    }

    public IEnumerator CR_Move(string direction)
    {

        double aux;

        MoveActive = true;

        switch (direction)
        {
            case "right":

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

                break;

            case "left":

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

                break;

            case "down":

                VerNavIndex++;

                if (VerNavIndex <= 8)
                {
                    ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().name);
                    EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
                    EvSys.SetSelectedGameObject(ActualSelected);
                }

                if (VerNavIndex > 8)
                {
                    StartCoroutine(CR_SkipPage(1));
                    /*
                    aux = ChVerScbr.value - JumpVer;
                    if (ChVerScbr.value != 0.0f)
                    {
                        StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                    }
                    VerNavIndex = 8;
                    */
                }

                break;

            case "up":

                VerNavIndex--;

                if (VerNavIndex >= 1)
                {
                    ActualAncestor = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().name);
                    EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().Select();
                    EvSys.SetSelectedGameObject(ActualSelected);
                }

                if (VerNavIndex < 1)
                {
                    StartCoroutine(CR_SkipPage(2));
                    /*
                    aux = ChVerScbr.value + JumpVer;
                    if (ChVerScbr.value != 1.0f)
                    {
                        StartCoroutine(CR_LerpFloatTo((float)aux, 0.2f, direction));
                    }

                    VerNavIndex = 1;
                    */
                }

                break;
        }

        MoveActive = false;

        yield break;
    }

    public IEnumerator CR_SkipPage(int option)
    {
        GameObject container = GameObject.Find(Macros.ChannelsContainer);
        GameObject FOP = GameObject.Find(FirstOfPage);

        string newfirst = "";

        int total = container.transform.childCount, init = 0, fin = 0;

        init = FOP.transform.parent.parent.parent.GetSiblingIndex();

        if (option == 1)
        {

            fin = init + 8;

            CurrentPage++;

            for (int i = init; i < fin + 1; i++)
            {
                if (i < fin)
                    container.transform.GetChild(i).gameObject.SetActive(false);

                if (i == fin)
                {
                    yield return new WaitForSeconds(0.5f);
                    newfirst = container.transform.GetChild(i).name; //channel
                    FirstOfPage = GameObject.Find(newfirst + "/Programs/ProgramsGrid").transform.GetChild(0).name;
                    Focus(FirstOfPage);
                    SetProgramationToHour(Hour);
                }
            }

            VerNavIndex = 1;
        }
        else
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;

                fin = init - 8;

                print(init + " " + fin);

                for (int j = init; j >= fin; j--)
                {
                    container.transform.GetChild(j).gameObject.SetActive(true);

                    if(j == init - 1)
                    {
                        Focus(GameObject.Find(container.transform.GetChild(j).name + "/Programs/ProgramsGrid").transform.GetChild(0).name);
                    }

                    if (j == fin)
                    {
                        yield return new WaitForSeconds(0.5f);
                        FirstOfPage = GameObject.Find(container.transform.GetChild(j).name + "/Programs/ProgramsGrid").transform.GetChild(0).name;    
                    }
                }
                
                SetProgramationToHour(Hour);

                yield return new WaitForSeconds(0.5f);

                VerNavIndex = 8;
            }

        }

        
        HorNavIndex = 1;

        SetRC(0);

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

    //Server Talk

    public void SetRC(int accion)
    {
        StartCoroutine(ETVServerManager.CR_SendToServer("insertar-accion", accion, 0));
    }
}
