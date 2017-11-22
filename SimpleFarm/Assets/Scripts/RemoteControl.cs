using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFarmNamespace;

public class RemoteControl : MonoBehaviour {

    //Remote Control

    private int Action;

    private string RC;

    public int NumCanal;
    public int NumCanalFromBD
    {
        get
        {
            return NumCanal;
        }

        set
        {
            if (NumCanal != value)
            {
                NumCanal = value;
            }
        }
    }

    //Debugging Control

    private Button right;
    private Button left;
    private Button up;
    private Button down;
    private Button ok;
    private Button back;

    private void Update()
    {
        print("RC ACTION: " + Action);

        if(Action == 0)
        {
            BlockMove(true);
        }else
        {
            BlockMove(false);
        }
    }

    void Start () {
        Initialize();
    }

    public void Initialize()
    {
        RC = "";
        StartCoroutine(CR_GetAction());

        down = GameObject.Find("Down-btn").GetComponent<Button>();
        down.onClick.AddListener(() => StartCoroutine(WaitForInner(1)));

        up = GameObject.Find("Up-btn").GetComponent<Button>();
        up.onClick.AddListener(() => StartCoroutine(WaitForInner(2)));

        right = GameObject.Find("Right-btn").GetComponent<Button>();
        right.onClick.AddListener(() => StartCoroutine(WaitForInner(3)));

        left = GameObject.Find("Left-btn").GetComponent<Button>();
        left.onClick.AddListener(() => StartCoroutine(WaitForInner(4)));

        ok = GameObject.Find("Ok-btn").GetComponent<Button>();
        ok.onClick.AddListener(() => StartCoroutine(WaitForInner(5)));

        back = GameObject.Find("Back-btn").GetComponent<Button>();
        back.onClick.AddListener(() => StartCoroutine(WaitForInner(6)));
    }

    public void BlockMove(bool state)
    {
        right.interactable = state;
        left.interactable = state;
        up.interactable = state;
        down.interactable = state;
        ok.interactable = state;
        back.interactable = state;
    }

    public void SetRC(int accion)
    {
        StartCoroutine(ETVServerManager.CR_SendToServer("insertar-accion", accion, 0));
    }

    public IEnumerator WaitForInner(int accion)
    {
        /*
        BlockMove(false);
        */

        SetRC(accion);

        yield return new WaitUntil(()=> Action != 0);

        /*
        yield return new WaitUntil(()=> Action == 0);
        
        BlockMove(true);
        */
        yield break;
    }

    private IEnumerator CR_GetAction()
    {
        string[] RCdata;

        WWWForm form;
        WWW ActionQuery;

        form = new WWWForm();
        form.AddField("opcion", "leer-accion");

        while (true)
        {
            
            ActionQuery = new WWW("https://commendable-rigs.000webhostapp.com/ControlRemoto.php", form);
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return ActionQuery;

            if (ActionQuery.error == null)
            {
                if(RC != ActionQuery.text)
                {
                    RC = ActionQuery.text;
                    //Parse
                    RCdata = ActionQuery.text.ToString().Split(new string[] { "-" }, StringSplitOptions.None);
                    Action = int.Parse(RCdata[0]);
                    NumCanalFromBD = int.Parse(RCdata[1]);
                }
            }
            else
            {
                print("Server Error (Remote Control): (" + ActionQuery.error + ")");
            }
            yield return null;
        }

    }
}
