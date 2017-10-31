using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using SimpleFarmNamespace;
using System.Linq;

//Guia de Canales

public class ChannelsGuide : MonoBehaviour {
    
    //Scripts

    ETVServerManager ServerScrpt;

    //Lists

    private List<Canal> Canales;
    public List<Canal> CanalesFromBD{
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

    //Navigation
    string First;
    private EventSystem EvSys;
    public GameObject ActualGameObjectSelected, FirstSelected;
    public int HorNavIndex, VerNavIndex, NumChannels, NumPrograms, CurrentColumn, ActualHour;
    public double JumpHor, JumpVer;
    private Scrollbar ChHorScbr;
    private Scrollbar ChVerScbr;
    public bool ReadyInstantiate;

    //Clock

    private Text Clock;
    private Text Date;

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
                //SetProgramationToHour(value);
            }
        }
    }
    private IEnumerator CO_CheckHour()
    {
        while (true)
        {
            HourC = DateTime.Now.Hour;
            yield return new WaitForSeconds(1.0f);
        }
    }

    //Debugging Control

    private Button right;
    private Button left;
    private Button up;
    private Button down;
    
    private void Update()
    {       
        Clock.text = DateTime.Now.ToString("HH:mm:ss");
        Date.text = DateTime.Now.ToString("D");
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    void InitializeValues() {

        ReadyInstantiate = false;

        //Debugging
        right = GameObject.Find("Right-btn").GetComponent<Button>();
        left = GameObject.Find("Left-btn").GetComponent<Button>();
        up = GameObject.Find("Up-btn").GetComponent<Button>();
        down = GameObject.Find("Down-btn").GetComponent<Button>();

        //Navigation
        ServerScrpt = GameObject.Find("Controller").GetComponent<ETVServerManager>();
        Canales = new List<Canal>();
        Programacion = new List<Programa>();
        EvSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        ChVerScbr = GameObject.Find("ChVertical-scbr").GetComponent<Scrollbar>();
        ChHorScbr = GameObject.Find("ChHorizontal-scbr").GetComponent<Scrollbar>();
        right.onClick.AddListener(() => Move("right"));
        left.onClick.AddListener(() => Move("left"));
        up.onClick.AddListener(() => Move("up"));
        down.onClick.AddListener(() => Move("down"));

        HorNavIndex = 1;
        VerNavIndex = 1;
        CurrentColumn = 12;
        ActualHour = 12;
        JumpHor = 0.04760598f;
        JumpVer = 0.0f;

        //Clock
        Hour = 12;
        Clock = GameObject.Find("Clock/Text").GetComponent<Text>();
        Date = GameObject.Find("Date/Text").GetComponent<Text>(); 
    }

    private IEnumerator Initialize()
    {
        
        InitializeValues();

        yield return new WaitUntil(()=> ServerScrpt.Ready);

        StartCoroutine(Instantiate());

        yield return new WaitUntil(()=>(ReadyInstantiate == true));

        FirstSelected = GameObject.Find(First);
        ActualGameObjectSelected = FirstSelected;
        EvSys.firstSelectedGameObject = FirstSelected;
        EvSys.SetSelectedGameObject(FirstSelected);

        yield return new WaitUntil(() => EvSys.alreadySelecting == true);

        HourC = DateTime.Now.Hour;

        CheckNextSelectable("horizontal");
        CheckNextSelectable("vertical");

        SetProgramationToHour(Hour);

        StartCoroutine(CO_CheckHour());

        print("Loading Complete");
    }

    private IEnumerator Instantiate()
    {
        int it1, it2;
        NumChannels = Canales.Count();

        for(it1 = 0; it1 < NumChannels; it1++)
        //for (it1 = 0; it1 < 1; it1++)
        {
            GameObject prefabCanal = Resources.Load("Prefabs/ProgramationGuide/Channel") as GameObject;
            GameObject CopyCanal = Instantiate(prefabCanal, GameObject.Find("ChannelsContainerGrid").transform);
            CopyCanal.name = "Canal" + it1;
            CopyCanal.GetComponentInChildren<Text>().text = Canales[it1].nombre;
            CopyCanal.transform.Find("Programs").GetComponent<ScrollRect>().horizontalScrollbar = ChHorScbr;

            Programacion.Clear();

            ServerScrpt.GetProgramas(it1);

            //print(Programacion.Count());

            yield return new WaitWhile(() => Programacion.Count() == 0);

            NumPrograms = Programacion.Count();

            for (it2 = 0; it2 < NumPrograms; it2++)
            //for (it2 = 0; it2 < 24; it2++)
            {
                GameObject prefabProgram = Resources.Load("Prefabs/ProgramationGuide/Program") as GameObject;
                GameObject CopyProgram = Instantiate(prefabProgram, GameObject.Find("Canvas/Controller/ProgramationPanel/ChannelsContainer/ChannelsContainerGrid/" + CopyCanal.name + "/Programs/ProgramsGrid").transform);
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
        print("Ready instantiate");
        ReadyInstantiate = true;
        yield break;
    }

    public void CheckNextSelectable(string direction)
    {
        if (EvSys.currentSelectedGameObject != null)
        {
            if (direction == "horizontal")
            {
                if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
                {
                    right.interactable = false;
                }
                else { right.interactable = true; }

                if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft())
                {
                    left.interactable = false;
                }
                else
                {
                    left.interactable = true;
                }
            }
            if (direction == "vertical")
            {
                if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown())
                {
                    down.interactable = false;
                }
                else { down.interactable = true; }

                if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp())
                {
                    up.interactable = false;
                }
                else { up.interactable = true; }
            }
        }
        if (CurrentColumn == ActualHour)
        {
            left.interactable = false;
        }
    }

    public void CheckGuideScrollbarFinish()
    {
        if (ChHorScbr.value > 0.99f)
        {
            ChHorScbr.value = 1.0f;
        }

        if (ChHorScbr.value < 0.011f)
        {
            ChHorScbr.value = 0.0f;
        }
    }

    public void Move(string direction)
    {
        if(!MoveActive)
            StartCoroutine(CO_Move(direction));
    }

    private bool MoveActive;
	public IEnumerator CO_Move(string direction) {

        MoveActive = true;
        double aux;

        if (ActualGameObjectSelected != null)
        {
            EvSys.SetSelectedGameObject(ActualGameObjectSelected);

            if(direction == "right" || direction == "left")
            {
                CheckNextSelectable("horizontal");
            }
            if(direction == "up" || direction == "down")
            {
                CheckNextSelectable("vertical");
            }

            switch (direction)
            {
                case "right":
                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
                    {
                        HorNavIndex++;
                        CurrentColumn++;
                        ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().Select();
                        EvSys.SetSelectedGameObject(ActualGameObjectSelected);

                        if (HorNavIndex > 3)
                        {
                            aux = ChHorScbr.value + JumpHor;

                            if (ChHorScbr.value != 1.0f)
                            {
                                StartCoroutine(LerpFloatTo((float)aux, 0.2f, direction));
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
                        ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft().Select();
                        EvSys.SetSelectedGameObject(ActualGameObjectSelected);

                        if (HorNavIndex < 1)
                        {
                            aux = ChHorScbr.value - JumpHor;
                            if (ChHorScbr.value != 0.0f)
                            {
                                StartCoroutine(LerpFloatTo((float)aux, 0.2f, direction));
                            }

                            HorNavIndex = 1;
                        }
                    }
                        
                    break;

                case "down":
                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown())
                    {
                        VerNavIndex++;
                        ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
                        EvSys.SetSelectedGameObject(ActualGameObjectSelected);
                        print(ActualGameObjectSelected.name);

                        if (VerNavIndex > 8)
                        {
                            aux = ChVerScbr.value - JumpVer;
                            if (ChVerScbr.value != 0.0f)
                            {
                                StartCoroutine(LerpFloatTo((float)aux, 0.2f, direction));
                            }
                            VerNavIndex = 8;
                        }

                    }

                    break;

                case "up":

                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp())
                    {
                        VerNavIndex--;
                        ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp().Select();
                        EvSys.SetSelectedGameObject(ActualGameObjectSelected);


                        if (VerNavIndex < 1)
                        {
                            aux = ChVerScbr.value + JumpVer;
                            if (ChVerScbr.value != 1.0f)
                            {
                                StartCoroutine(LerpFloatTo((float)aux, 0.2f, direction));
                            }

                            VerNavIndex = 1;
                        }
                    }
                    break;
            }

            yield return new WaitForSeconds(0.3f);

            if (direction == "right" || direction == "left")
            {
                CheckNextSelectable("horizontal");
            }

            if (direction == "up" || direction == "down")
            {
                CheckNextSelectable("vertical");
            }
        }

        MoveActive = false;

        yield break;
    }

    public IEnumerator LerpFloatTo(float end, float atime, string direction) // Moves scrollbar value to previous or next page
    {
        if (direction == "right" || direction == "left")
        {
            left.interactable = false;
            right.interactable = false;
        }

        if (direction == "up" || direction == "down")
        {
            up.interactable = false;
            down.interactable = false;
        }
        
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

        CheckGuideScrollbarFinish();

        if (direction == "right" || direction == "left")
        {
            CheckNextSelectable("horizontal");
            left.interactable = true;
            right.interactable = true;
        }

        if (direction == "up" || direction == "down")
        {
            CheckNextSelectable("vertical");
            up.interactable = true;
            down.interactable = true;
        }

        yield break;
    }

    public void SetProgramationToHour(int hour)
    {
        double aux;

        CurrentColumn = hour;
        ActualHour = hour;
        aux = JumpHor * hour;
        ChHorScbr.value = (float) aux;

        for (int i = 1; i<= hour; i++)
        {
            ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().name);
            EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().Select();
            EvSys.SetSelectedGameObject(ActualGameObjectSelected);
        }
    }

    public void Stop()
    {
        ReadyInstantiate = false;
    }
}
