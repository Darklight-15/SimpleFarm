using System;
using UnityEngine;
using System.Collections;
using SimpleFarmNamespace;

public class CentrosControllerScript : MonoBehaviour {

    private WindowManager basicScript;
    private GridClass gridScript;

    private void Awake()
    {
        StartCoroutine(InitializeControlScript());
    }

    private IEnumerator InitializeControlScript()
    {
        // Set Inherit Scripts
        gameObject.AddComponent<WindowManager>();
        gameObject.AddComponent<GridClass>();

        // Get Inherit Scripts
        basicScript = GetComponent<WindowManager>();
        gridScript = GetComponent<GridClass>();

        //START CONTROLLER IDENTITY
        gridScript.type = "centro";
        gridScript.itemsOnServerPath = "https://commendable-rigs.000webhostapp.com/initarjetas.php";
        //gridScript.serverPath = "http://localhost/indicadores/initarjetas.php";
        //Prefab MUST have children called Titulo
        gridScript.prefabLocalPath = "Prefabs/CentroPrefab/CentroPrefab";
        StartCoroutine(gridScript.Initialize());
        //END CONTROLLER IDENTITY

        basicScript.InstanceAndShowPopUp("temporal", "BasicPopUpPrefab", "CentrosViewCanvas", "Cargando", 8.0f, 0.5f);
        yield return new WaitUntil(() => gridScript.Ready);

        //Begin Editable Zone
        //ActivateDataChecking();
    }

}
