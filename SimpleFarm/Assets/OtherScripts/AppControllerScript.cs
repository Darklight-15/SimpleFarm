using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFarmNamespace;
using System;

public class AppControllerScript : MonoBehaviour{

    static bool Ready;
    static int NumberCanales;
    
    //Scripts

    private GridClass GridScript;
    private ServerClass ServerScript;
    private ThemeClass ThemeScript;
    private WindowManager WinManagerScript;

    //Grid Info

    public string GridName;
    private GameObject GridLocation;
    private GameObject GridCanales;

    // Remote Control & Navigation Values

    public int Canal;
    public int CanalFromBD{
        get
        {
            return Canal;
        }

        set
        {
            if (AuxFunctions.ValueChanged("canalAppController", Canal, value))
            {
                Canal = value;
                if(Canal == 1)
                {
                    GoToChannel("indicadores");
                }
                if(Canal == 5)
                {
                    GoToChannel("favorites");
                }
            }                
        }
    }

    public int Nivel;
    public int NivelFromBD
    {
        get
        {
            return Nivel;
        }

        set
        {
            if (AuxFunctions.ValueChanged("nivelAppController", Nivel, value))
            {
                if (value < Nivel && value != 0)
                {
                    JumpToComponent("back");
                }

                Nivel = value;

                if (value == 0)
                {
                    JumpToComponent("menu");
                }
            }    
        }
    }

    public int EmpresaID;
    public int EmpresaIdFromBD
    {
        get
        {
            return EmpresaID;
        }

        set
        {
            if (AuxFunctions.ValueChanged("empresa", EmpresaID, value))
            {
                EmpresaID = value;
                if (value != -1)
                {
                    JumpToComponent("granja");
                }
            }
                
        }
    }

    public int GranjaID;
    public int GranjaIdFromBD
    {
        get
        {
            return GranjaID;
        }

        set
        {
            if (AuxFunctions.ValueChanged("granjaid", GranjaID, value))
            {
                GranjaID = value;
                if (value != -1)
                {

                    JumpToComponent("nucleo");
                }
                
            }
        }
    }

    public int NucleoID;
    public int NucleoIdFromBD
    {
        get
        {
            return NucleoID;
        }

        set
        {
            if (AuxFunctions.ValueChanged("nucleo", NucleoID, value))
            {
                NucleoID = value;
                JumpToComponent("galpon");
            }

        }
    }
    
    public int GalponID;
    public int GalponIdFromBD
    {
        get
        {
            return GalponID;
        }

        set
        {
            if (AuxFunctions.ValueChanged("galpon", GalponID, value))
            {
                GalponID = value;
            }
        }
    }
    
    public int SiloID;
    public int SiloIdFromBD
    {
        get
        {
            return SiloID;
        }

        set
        {
            if (AuxFunctions.ValueChanged("silo", SiloID, value))
            {
                SiloID = value;
            }
        }
    }

    //Debugging and Navigation Buttons

    private Button backBtn;
    Button empresaBtn;
    Button granjaBtn;
    Button nucleoBtn;
    Button galponBtn;
    Image empresaImg;
    Image granjaImg;
    Image nucleoImg;
    Image galponImg;
    Color enabledColor;
    Color disabledColor;

    public void Start()
    {
        Play();
    }

    public void Play()
    {
        StartCoroutine(Initialize());
    }

    public void Stop()
    {
        StopCoroutine("Initialize");
    }

    public IEnumerator Initialize()
    {
        InitializeLocalValues();

        WinManagerScript.InstanceAndShowPopUp("temporal", "BasicPopUpPrefab", "SimpleFarmCanvas", "Cargando", 1.0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        ServerScript.ResetBD();

        yield return new WaitForSeconds(0.5f);

        ServerScript.Play();
        
        yield return new WaitUntil(() => ServerScript.Online && ServerScript.Ready);

        print("App Controller Ready");
    }

    public void InitializeLocalValues()
    {
        //Navigation Values

        Canal = 0;
        Nivel = 0;
        EmpresaID = -1;
        GranjaID = -1;
        NucleoID = -1;
        GalponID = -1;
        SiloID = -1;

        //Get Scripts

        ServerScript = gameObject.GetComponent<ServerClass>();
        GridScript = gameObject.GetComponent<GridClass>();
        ThemeScript = gameObject.GetComponent<ThemeClass>();
        WinManagerScript = gameObject.GetComponent<WindowManager>();

        //Initialize Grids

        GridName = "Grid";
        GridLocation = GameObject.Find("Grid");
        GridCanales = GameObject.Find("GridCanales");
        GridLocation.SetActive(false);

        //Channels Buttons

        GameObject.Find("Temas").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => GoToChannel("temas"));
        GameObject.Find("Indicadores").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => GoToChannel("indicadores"));
        GameObject.Find("Resumen").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => GoToChannel("resumen"));
        GameObject.Find("Temperatura").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => GoToChannel("temperatura"));

        //Debugging and navigation

        backBtn = GameObject.Find("back-btn").GetComponent<Button>();
        backBtn.onClick.AddListener(() => JumpToComponent("menu"));
        empresaBtn = GameObject.Find("empresas-btn").GetComponent<Button>();
        granjaBtn = GameObject.Find("granjas-btn").GetComponent<Button>();
        nucleoBtn = GameObject.Find("nucleos-btn").GetComponent<Button>();
        galponBtn = GameObject.Find("galpones-btn").GetComponent<Button>();
        empresaImg = GameObject.Find("empresas-btn").GetComponent<Image>();
        granjaImg = GameObject.Find("granjas-btn").GetComponent<Image>();
        nucleoImg = GameObject.Find("nucleos-btn").GetComponent<Image>();
        galponImg = GameObject.Find("galpones-btn").GetComponent<Image>();
        enabledColor = AuxFunctions.ConvertColorRGBA(107.0f, 171.0f, 129.0f, 1.0f);
        disabledColor = AuxFunctions.ConvertColorRGBA(143.0f, 168.0f, 185.0f, 1.0f);
        empresaImg.color = disabledColor;
        granjaImg.color = disabledColor;
        nucleoImg.color = disabledColor;
        galponImg.color = disabledColor;
        empresaBtn.onClick.AddListener(() => JumpToComponent("empresa"));
        granjaBtn.onClick.AddListener(() => JumpToComponent("granja"));
        nucleoBtn.onClick.AddListener(() => JumpToComponent("nucleo"));
    }

    private void SetGridCanales(bool state) //Changes between Main Screen and Grid Screen
    {
        GridCanales.SetActive(state);
        if (state)
        {
            GridScript.CleanGrid();
            GridLocation.SetActive(false);
        }
        else
        {
            GridLocation.SetActive(true);
        }
    }

    public void GoToChannel(string channel)
    {
        switch (channel)
        {
            case "indicadores":
                Canal = 1;
                Nivel = 1;
                SetGridCanales(false);
                StartCoroutine(Instantiate("empresa", 0));
                empresaImg.color = enabledColor;
                break;

            case "favorites":
                Canal = 5;
                SetGridCanales(false);
                StartCoroutine(Instantiate("favorites", 0));
                break;

            case "temas":
                break;

            case "resumen":
                break;

            case "temperatura":
                break;
        }
    }

    public void JumpToComponent(string type)
    {
        GridScript.CleanGrid();

        switch (type)
        {

            case "back":
                /*
                if (Nivel == 1)
                {
                    ServerScript.ResetBD();
                    SetGridCanales(true);

                    empresaBtn.interactable = false;
                    empresaImg.color = disabledColor;
                   
                    ServerManager.SendFlagToServer(1);
                    print("back from empresa");
                }
                */
                if (Nivel == 2)
                {
                    JumpToComponent("empresa");
                    print("back from granja");
                }

                if (Nivel == 3)
                {
                    JumpToComponent("granja");
                    print("back from nucleo");
                }

                if (Nivel == 4)
                {
                    JumpToComponent("nucleo");
                     print("back from galpon");
                }

                break;


            case "menu":

                WinManagerScript.InstanceAndShowPopUp("temporal", "BasicPopUpPrefab", "SimpleFarmCanvas", "Cargando", 1.0f, 0.5f);
                ServerScript.ResetBD();
                SetGridCanales(true);

                empresaBtn.interactable = false;
                empresaImg.color = disabledColor;

                EmpresaID = -1;
                GranjaID = -1;
                NucleoID = -1;
                GalponID = -1;

                empresaBtn.interactable = false;
                granjaBtn.interactable = false;
                nucleoBtn.interactable = false;
                galponBtn.interactable = false;

                empresaImg.color = enabledColor;
                granjaImg.color = disabledColor;
                nucleoImg.color = disabledColor;
                galponImg.color = disabledColor;

                ServerClass.SendFlagToServer(1);

                break;

            case "empresa":

                StartCoroutine(Instantiate("empresa", 0));

                EmpresaID = -1;
                GranjaID = -1;
                NucleoID = -1;
                GalponID = -1;

                empresaBtn.interactable = false;
                granjaBtn.interactable = false;
                nucleoBtn.interactable = false;
                galponBtn.interactable = false;

                empresaImg.color = enabledColor;
                granjaImg.color = disabledColor;
                nucleoImg.color = disabledColor;
                galponImg.color = disabledColor;

                break;

            case "granja":

                StartCoroutine(Instantiate("granja", EmpresaID));

                GranjaID = -1;
                NucleoID = -1;
                GalponID = -1;

                empresaBtn.interactable = true;
                granjaBtn.interactable = false;
                nucleoBtn.interactable = false;
                galponBtn.interactable = false;

                empresaImg.color = disabledColor;
                granjaImg.color = enabledColor;
                nucleoImg.color = disabledColor;
                galponImg.color = disabledColor;

                break;

            case "nucleo":

                StartCoroutine(Instantiate("nucleo", GranjaID));

                NucleoID = -1;
                GalponID = -1;

                empresaBtn.interactable = true;
                granjaBtn.interactable = true;
                nucleoBtn.interactable = false;
                galponBtn.interactable = false;

                empresaImg.color = disabledColor;
                granjaImg.color = disabledColor;
                nucleoImg.color = enabledColor;
                galponImg.color = disabledColor;

                break;

            case "galpon":

                StartCoroutine(Instantiate("galpon", NucleoID));

                GalponID = -1;

                empresaBtn.interactable = true;
                granjaBtn.interactable = true;
                nucleoBtn.interactable = true;
                galponBtn.interactable = false;

                empresaImg.color = disabledColor;
                granjaImg.color = disabledColor;
                nucleoImg.color = disabledColor;
                galponImg.color = enabledColor;

                break;
        }
    }

    private IEnumerator Instantiate(string type, int ID)
    {
        string prefabPath = "", instanceServerPath = "";
        switch (type)
        {

            case "empresa":
                instanceServerPath = "https://ir7b164b04a7.us1.hana.ondemand.com/SimpleFarm/services/avicola.xsodata/OSPARTNERSHIP?$format=json";
                prefabPath = "Prefabs/EmpresaPrefab/EmpresaPrefab";

                break;

            case "granja":
                instanceServerPath = "https://ir7b164b04a7.us1.hana.ondemand.com/SimpleFarm/services/avicola.xsodata/OSBROILERSFARM?$format=json&$filter=OSPARTNERSHIP.PARTNERSHIPID eq '" + ID + "'";
                prefabPath = "Prefabs/GranjaPrefab/GranjaPrefab";
                break;

            case "nucleo":
                instanceServerPath = "https://ir7b164b04a7.us1.hana.ondemand.com/SimpleFarm/services/avicola.xsodata/OSCENTER?$format=json&$filter=OSBROILERSFARM.BROILERSFARMID eq '" + ID + "'";
                prefabPath = "Prefabs/NucleoPrefab/NucleoPrefab";
                break;

            case "galpon":
                instanceServerPath = "https://ir7b164b04a7.us1.hana.ondemand.com/SimpleFarm/services/avicola.xsodata/OSSHED?$format=json&$filter=OSCENTER.CENTERID eq '" + ID + "'";
                prefabPath = "Prefabs/GalponPrefab/GalponPrefab";
                ThemeScript.gridName = GridName;
                ThemeScript.themeLocation = "Fondo";
                ThemeScript.Initialize();
                break;

            case "favorites":

                break;
        }

        GridScript.type = type;
        GridScript.itemsOnServerPath = instanceServerPath;
        GridScript.serverpath = "https://ir7b164b04a7.us1.hana.ondemand.com";
        GridScript.prefabLocalPath = prefabPath;
        StartCoroutine(GridScript.Initialize());

        yield return new WaitUntil(() => GridScript.Ready);
        
        ServerClass.SendFlagToServer(1);
        print("FLAG EN 1 INSTANTIATE");
        yield break;
    }

}
