using UnityEngine;
using UnityEngine.UI;

public class NucleoElementScript : MonoBehaviour
{

    public string nucleoID;
    ControllerScript controlScript;

    private void Awake()
    {
        //controlScript = GameObject.Find("Controller").GetComponent<ControllerScript>();
        this.transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => this.SendNucleoID());
    }

    public void SendNucleoID()
    {
        controlScript.NucleoID = nucleoID;
        controlScript.JumpToComponent("galpon");
    }


}
