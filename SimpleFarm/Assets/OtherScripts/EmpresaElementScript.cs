using UnityEngine;
using UnityEngine.UI;

public class EmpresaElementScript : MonoBehaviour {

    public string empresaID;
    ControllerScript controlScript;

    private void Awake()
    {
        //controlScript = GameObject.Find("Controller").GetComponent<ControllerScript>();
        this.transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => this.SendEmpresaID());
    }

    public void SendEmpresaID()
    {
        controlScript.EmpresaID = empresaID;
        controlScript.JumpToComponent("granja");
    }
}
