using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFarmNamespace;

public class RemoteControl : MonoBehaviour {

    //Remote Control

    public bool ServerActive;
    public bool SendComplete;

    public string RC; //Remote control variable

    public int Action;

    public int Value;

    //Debugging Control

    private Button right;
    private Button left;
    private Button up;
    private Button down;
    private Button ok;
    private Button back;
    private Button ddown;
    private Button dup;

    private void Update()
    {
        /*
        print("RC ACTION: " + Action);

        if(Action == 0)
        {
            BlockMove(true);
        }else
        {
            BlockMove(false);
        }
        */
    }

    void Start () {
        StartCoroutine(Initialize());
    }

    public IEnumerator Initialize()
    {
        ServerActive = false;
        SendComplete = false;

        RC = "";

        StartCoroutine(CR_GetAction());

        yield return new WaitUntil(() => ServerActive);

        print("RC: Server respond");

        down = GameObject.Find("Down-btn").GetComponent<Button>();
        down.onClick.AddListener(() => StartCoroutine(WaitForInner(1, 0)));

        ddown = GameObject.Find("DDown-btn").GetComponent<Button>();
        ddown.onClick.AddListener(() => StartCoroutine(WaitForInner(1, 1)));

        up = GameObject.Find("Up-btn").GetComponent<Button>();
        up.onClick.AddListener(() => StartCoroutine(WaitForInner(2, 0)));

        dup = GameObject.Find("DUp-btn").GetComponent<Button>();
        dup.onClick.AddListener(() => StartCoroutine(WaitForInner(2, 1)));

        right = GameObject.Find("Right-btn").GetComponent<Button>();
        right.onClick.AddListener(() => StartCoroutine(WaitForInner(3, 0)));

        left = GameObject.Find("Left-btn").GetComponent<Button>();
        left.onClick.AddListener(() => StartCoroutine(WaitForInner(4, 0)));

        ok = GameObject.Find("Ok-btn").GetComponent<Button>();
        ok.onClick.AddListener(() => StartCoroutine(WaitForInner(5, 0)));

        back = GameObject.Find("Back-btn").GetComponent<Button>();
        back.onClick.AddListener(() => StartCoroutine(WaitForInner(6, 0)));

        yield break;
    }

    public IEnumerator WaitForInner(int dir, int val)
    {
        MoveEnabled(false);

        SendComplete = false;

        StartCoroutine(CR_SendToServer("insertar-accion", dir, val));

        yield return new WaitUntil(() => SendComplete);

        yield return new WaitUntil(() => (Action == 0));

        MoveEnabled(true);

        SendComplete = false;

        yield break;
    }

    public void MoveEnabled(bool state)
    {
        right.interactable = state;
        left.interactable = state;
        up.interactable = state;
        dup.interactable = state;
        down.interactable = state;
        ddown.interactable = state;
        ok.interactable = state;
        back.interactable = state;
    }

    public IEnumerator CR_SendToServer(string option, int action, int value)
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
            //print("RC: Successful Insert: (" + option + " : " + action + ")");
            SendComplete = true;    
        }
        else
        {
            print("RC: Error inserting row");
            SendComplete = false;
        }

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
            
            ActionQuery = new WWW(Macros.SitePath + "/ControlRemoto.php", form );
            //ActionQuery = new WWW("http://localhost/SF_Services/ControlRemoto.php");

            yield return ActionQuery;

            if (ActionQuery.error == null)
            {
                ServerActive = true;

                if(RC != ActionQuery.text)
                {
                    RC = ActionQuery.text;
                    //Parse
                    //print(ActionQuery.text);
                    RCdata = ActionQuery.text.ToString().Split(new string[] { "-" }, StringSplitOptions.None);
                    //print("RC: " + RCdata[0]);
                    Action = int.Parse(RCdata[0]);
                    Value = int.Parse(RCdata[1]);
                }
            }
            else
            {
                ServerActive = false;
                print("RC: Server Error (Remote Control): (" + ActionQuery.error + ")");
            }

            yield return null;
        }

    }

}
