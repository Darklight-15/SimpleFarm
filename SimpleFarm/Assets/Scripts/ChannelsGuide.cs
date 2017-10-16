using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using SimpleFarmNamespace;

//Guia de Canales

public class ChannelsGuide : MonoBehaviour {

    //Navigation
    private EventSystem EvSys;
    public ProgramScript Program;
    public GameObject ActualGameObjectSelected, FirstSelected;
    public int HorNavIndex, VerNavIndex, NumCol, NumRows, Current, ActualHour;
    public double JumpHor, JumpVer;
    private Scrollbar ChHorScbr;
    private Scrollbar ChVerScbr;
    bool ReadyInstantiate;

    //Clock
    private Text Clock;
    private Text Date;
    
    //Debugging Control

    private Button right;
    private Button left;
    private Button up;
    private Button down;
    
    private void Update()
    {
        //print("Scrollbar Value: " + ChVGuideScrollbar.value);
        Clock.text = DateTime.Now.ToString("HH:mm:ss");
        Date.text = DateTime.Now.ToString("D");
        //Clock.text = System.DateTime.UtcNow.ToString();
        if (Current == ActualHour)
        {
            left.interactable = false;
        }
        //Change
        
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
        EvSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        ChVerScbr = GameObject.Find("ChVertical-scbr").GetComponent<Scrollbar>();
        ChHorScbr = GameObject.Find("ChHorizontal-scbr").GetComponent<Scrollbar>();
        right.onClick.AddListener(() => MoveHorizontal("right"));
        left.onClick.AddListener(() => MoveHorizontal("left"));
        up.onClick.AddListener(() => MoveVertical("up"));
        down.onClick.AddListener(() => MoveVertical("down"));

        HorNavIndex = 1;
        VerNavIndex = 1;
        Current = 12;
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
        
        int it1;

        yield return new WaitForSeconds(0.5f);

        InstantiatePrefab("Prefabs/ProgramationGuide/Channel", "Canvas/Controller/ProgramationPanel/ChannelsContainer/ChannelsContainerGrid", 12, "Canal", "canal");

        NumCol = 12;
        NumRows = 24;

        for (it1=0; it1 < NumCol; it1++)
        {
            InstantiatePrefab("Prefabs/ProgramationGuide/Program", "Canvas/Controller/ProgramationPanel/ChannelsContainer/ChannelsContainerGrid/"+ "Canal" + it1 +"/Programs/ProgramsGrid", 24, "Reporte"+ it1 + "-", "programa");
        }

        JumpVer = (double)1 / (NumCol - 8);

        yield return new WaitForSeconds(0.5f);

        FirstSelected = GameObject.Find("Reporte0-0");
        EvSys.firstSelectedGameObject = FirstSelected;
        EvSys.SetSelectedGameObject(FirstSelected);
        ActualGameObjectSelected = FirstSelected;

        CheckNextSelectableHor();
        CheckNextSelectableVer();

        HourC = DateTime.Now.Hour;
        SetProgramationToHour(Hour);

        StartCoroutine(CheckHour());
       
    }

    public void CheckNextSelectableHor()
    {
        if(EvSys.currentSelectedGameObject != null)
        {
            if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
            {
                right.interactable = false;
            }
            else { right.interactable = true; }

            if (!EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnLeft() || Current == ActualHour)
            {
                left.interactable = false;
            }
            else { left.interactable = true; }
        }
    }

    public void CheckNextSelectableVer()
    {
        if (EvSys.currentSelectedGameObject != null)
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

	public void MoveHorizontal(string direction) {

        double aux;

        if (ActualGameObjectSelected != null)
        {
            EvSys.SetSelectedGameObject(ActualGameObjectSelected);

            CheckNextSelectableHor();

            switch (direction)
            {
                case "right":
                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight())
                    {
                        HorNavIndex++;
                        Current++;
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
                        Current--;
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
            }

            CheckNextSelectableHor();
        }
    }

    public void MoveVertical(string direction)
    {
        double aux;

        if (ActualGameObjectSelected != null)
        {
            EvSys.SetSelectedGameObject(ActualGameObjectSelected);

            CheckNextSelectableVer();

            switch (direction)
            {
                case "down":
                    if (EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown())
                    {
                        VerNavIndex++;
                        ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().name);
                        EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
                        EvSys.SetSelectedGameObject(ActualGameObjectSelected);

                        
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

            CheckNextSelectableVer();
        }
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
            CheckNextSelectableHor();
            left.interactable = true;
            right.interactable = true;
        }

        if (direction == "up" || direction == "down")
        {
            CheckNextSelectableVer();
            up.interactable = true;
            down.interactable = true;
        }
        yield break;
    }

    public void SetProgramationToHour(int hour)
    {
        double aux;
        EvSys.firstSelectedGameObject = FirstSelected;
        EvSys.SetSelectedGameObject(FirstSelected);

        Current = hour;
        ActualHour = hour;
        aux = JumpHor * hour;
        ChHorScbr.value = (float) aux;

        for(int i = 1; i<= hour; i++)
        {
            ActualGameObjectSelected = GameObject.Find(EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().name);
            EvSys.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnRight().Select();
            EvSys.SetSelectedGameObject(ActualGameObjectSelected);
        }
    }

    public void InstantiatePrefab(string prefabPath, string parentPath, int numItems, string name, string type)
    {
        GameObject prefabInstance = Resources.Load(prefabPath) as GameObject;

        for (int i = 0; i < numItems; i++)
        {
            GameObject Copy = Instantiate(prefabInstance, GameObject.Find(parentPath).transform) as GameObject;
            Copy.name = name + i;

            if (i == 0)
            {
                FirstSelected = Copy;
            }

            switch (type)
            {
                case "canal":
                    Copy.transform.Find("Programs").GetComponent<ScrollRect>().horizontalScrollbar = ChHorScbr;
                    break;

                case "programa":
                    Copy.GetComponentInChildren<ProgramScript>().Position = i;
                    Copy.GetComponentInChildren<ProgramScript>().ProgramID = i;
                    Copy.GetComponentInChildren<ProgramScript>().Name = "Programa" + i;
                    Copy.GetComponentInChildren<Text>().text = Copy.GetComponentInChildren<ProgramScript>().name;
                    break;
            }
        }

    }

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
                SetProgramationToHour(value);
            }
        }
    }

    private IEnumerator CheckHour()
    {
        while (true)
        {
            HourC = DateTime.Now.Hour;
            //print(HourC);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
