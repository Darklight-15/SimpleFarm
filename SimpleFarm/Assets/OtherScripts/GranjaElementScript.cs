using UnityEngine;
using UnityEngine.UI;

public class GranjaElementScript : MonoBehaviour {

    public string granjaID;
    ControllerScript controlScript;

    private void Awake()
    {
        //controlScript = GameObject.Find("Controller").GetComponent<ControllerScript>();
        this.transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => this.SendGranjaID());
    }

    public void SendGranjaID()
    {
        controlScript.GranjaID = granjaID;
        controlScript.JumpToComponent("nucleo");
    }
}
