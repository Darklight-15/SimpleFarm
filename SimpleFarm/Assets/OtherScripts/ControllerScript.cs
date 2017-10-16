using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFarmNamespace;

public class ControllerScript : MonoBehaviour {

    int NumberCanales;
    
    //Scripts
    private GridClass gridScript;
    private ThemeClass themeScript;


    //Grid Info
    public string gridName;
    GameObject grid;
    GameObject gridCanales;

    // Remote Control & Navigation Values
    public string remoteControlFromBD;
    private string storedRemoteControlFromBD;

    public int Canal;
    public int CanalFromBD;
    public int Nivel;
    public int NivelFromBD;

    public string EmpresaID;
    public string EmpresaIdFromBD;
    public string GranjaID;
    public string GranjaIdFromBD;
    public string NucleoID;
    public string NucleoIdFromBD;
    public string GalponID;
    public string SiloID;

    public int activeElem;
    public int zoomFromBD;
    public int currentPageFromBD;
    public int totalPagesFromBD;
    public int themeFromBD;
    public int flagFromBD;

    //Stored Values

    public int storedCanalFromBD;
    public int storedNivelFromBD;
    public string storedEmpresaFromBD;
    public string storedGranjaFromBD;
    public string storedNucleoFromBD;
    public string storedGalponFromBD;
    public string storedSiloFromBD;
    public int storedZoomFromBD;
    public int storedCurrentPageFromBD;

    public string galponesDataFromBD;
    private string storedGalponesDataFromBD;

    //Debugging and Navigation Buttons

    //private List<string> navigationPath;
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

    private void Update()
    {
        //print("DEBUG = canal: " + Canal + "/" + "nivel: " + Nivel + "/" + "empresaID: " + EmpresaIdFromBD + "/" + "granjaID: " + GranjaIdFromBD + "/" + "nucleoID: " + NucleoIdFromBD);
        //print("DEBUG = activa: " + activeElem + "/zoom: " + zoomFromBD + "/currentpage: " + currentPageFromBD + "/total: " + totalPagesFromBD + "/theme: " + themeFromBD);
        print("FLAG: " + flagFromBD);
    }

    private void Start()
    {
        StartCoroutine(Initialize());
    }

    public IEnumerator CheckConnection()
    {
        WWW www;
        while (true)
        {
            www = new WWW("https://commendable-rigs.000webhostapp.com");
            yield return www;

            if (www.error == null)
            {
                print("online");
            }
            else
            {
                print("offline");
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator Initialize()
    {
        StartCoroutine(CheckConnection());
        yield return new WaitForSeconds(2.0f);
        InitializeValues();
        yield return new WaitForSeconds(2.0f);
        InitializeBD();
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(CR_ReviewRemoteControl());
        StartCoroutine(CR_DataHasChanged());
        print("Controller Loading Complete");
        //StartFavorites(true);
    }

    public void InitializeValues()
    {
        NumberCanales = 4;
        // Control & Navigation Values
        remoteControlFromBD = "";
        storedRemoteControlFromBD = "";

        Canal = 0;
        CanalFromBD = 0;
        storedCanalFromBD = 0;

        Nivel = 0;
        NivelFromBD = 0;
        storedNivelFromBD = 0;

        EmpresaID = "-1";
        EmpresaIdFromBD = "-1";
        storedEmpresaFromBD = "-1";

        GranjaID = "-1";
        GranjaIdFromBD = "-1";
        storedGranjaFromBD = "-1";

        NucleoID = "-1";
        NucleoIdFromBD = "-1";
        storedNucleoFromBD = "-1";

        GalponID = "-1";
        galponesDataFromBD = "-1";
        storedGalponesDataFromBD = "-1";

        SiloID = "-1";
        storedSiloFromBD = "-1";

        activeElem = -1;
        zoomFromBD = 1;
        currentPageFromBD = 1;
        totalPagesFromBD = 0;
        themeFromBD = 0;
        flagFromBD = 0;

        //Initialize Grids
        gridName = "Grid";
        grid = GameObject.Find("Grid");
        gridCanales = GameObject.Find("GridCanales");
        grid.SetActive(false);

        // Add Scripts from Namespace
        if(gameObject.GetComponent<GridClass>() == null)
            gameObject.AddComponent<GridClass>();

        if(gameObject.GetComponent< ThemeClass>() == null)
            gameObject.AddComponent< ThemeClass>();
 
        // Get Scripts
        gridScript = GetComponent<GridClass>();
        themeScript = GetComponent<ThemeClass>();

        //UI Interface

        //Channels Buttons
        GameObject.Find("Temas").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => TemasChannel());
        GameObject.Find("Indicadores").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => IndicadoresChannel());
        GameObject.Find("Resumen").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => ResumenChannel());
        GameObject.Find("Temperatura").transform.Find("Fondo").GetComponent<Button>().onClick.AddListener(() => TemperaturaChannel());

        //Debugging an navigation
        //navigationPath = new List<string>();
        backBtn = GameObject.Find("back-btn").GetComponent<Button>();
        backBtn.onClick.AddListener(() => JumpToComponent("back"));
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

    public void InitializeBD()
    {
        WWW web = new WWW("https://commendable-rigs.000webhostapp.com/inicializarNavegacion.php");
    }

    //==========================================================================================
    // Channels List
    //==========================================================================================

    private void TemasChannel()
    {
        print("Theme Channel");
    }
    private void IndicadoresChannel()
    {
        //Indicadores Channel
        Canal = 1;
        Nivel = 1;
        SetGridCanales(false);
        StartCoroutine(Instantiate("empresa", ""));
        StartCoroutine(CR_NavigationChanged());
        empresaImg.color = enabledColor;
    }
    private void ResumenChannel()
    {
        print("Resumen Channel");
    }
    private void TemperaturaChannel()
    {
        print("Temperatura Channel");
    }

    //==========================================================================================
    // Has Change Functions
    //==========================================================================================

    private bool ZoomHasChanged(ref int newvalor)
    {
        if (storedZoomFromBD != newvalor)
        {
            storedZoomFromBD = newvalor;
            return true;
        }

        storedZoomFromBD = newvalor;
        return false;
    }
    private bool CheckZoomHasChanged()
    {
        return ZoomHasChanged(ref zoomFromBD);
    }

    private bool CurrentPageHasChanged(ref int newvalor)
    {
        if (storedCurrentPageFromBD != newvalor)
        {
            storedCurrentPageFromBD = newvalor;
            return true;
        }

        storedCurrentPageFromBD = newvalor;
        return false;
    }
    private bool CheckCurrentPageHasChanged()
    {
        return CurrentPageHasChanged(ref currentPageFromBD);
    }

    private bool CanalHasChanged(ref int newvalor)
    {
        if (storedCanalFromBD != newvalor)
        {
            storedCanalFromBD = newvalor;
            return true;
        }

        storedCanalFromBD = newvalor;
        return false;
    }
    private bool CheckCanalHasChanged()
    {
        return CanalHasChanged(ref CanalFromBD);
    }

    private bool NivelHasChanged(ref int newvalor)
    {
        if (storedNivelFromBD != newvalor)
        {
            if (newvalor < storedNivelFromBD)
            {
                JumpToComponent("back");
            }
            storedNivelFromBD = newvalor;
            return true;
        }

        storedNivelFromBD = newvalor;
        return false;
    }
    private bool CheckNivelHasChanged()
    {
        return NivelHasChanged(ref NivelFromBD);
    }

    private bool EmpresaHasChanged(ref string newvalor)
    {
        if (storedEmpresaFromBD != newvalor)
        {
            storedEmpresaFromBD = newvalor;
            return true;
        }

        storedEmpresaFromBD = newvalor;
        return false;
    }
    private bool CheckEmpresaHasChanged()
    {
        return EmpresaHasChanged(ref EmpresaIdFromBD);
    }

    private bool GranjaHasChanged(ref string newvalor)
    {
        if (storedGranjaFromBD != newvalor)
        {
            storedGranjaFromBD = newvalor;
            return true;
        }

        storedGranjaFromBD = newvalor;
        return false;
    }
    private bool CheckGranjaHasChanged()
    {
        return GranjaHasChanged(ref GranjaIdFromBD);
    }

    private bool NucleoHasChanged(ref string newvalor)
    {
        if (storedNucleoFromBD != newvalor)
        {
            storedNucleoFromBD = newvalor;
            return true;
        }

        storedNucleoFromBD = newvalor;
        return false;
    }
    private bool CheckNucleoHasChanged()
    {
        return NucleoHasChanged(ref NucleoIdFromBD);
    }

    private IEnumerator CR_DataHasChanged()
    {
        while (true)
        {
            if (CheckCanalHasChanged()) {
                if(CanalFromBD > 0 && CanalFromBD <= NumberCanales) Canal = CanalFromBD;

                if (CanalFromBD == 1)
                {
                    IndicadoresChannel();
                }
            }

            if (CheckNivelHasChanged())
            {
                if(NivelFromBD >= 0 && NivelFromBD <= 4)
                {
                    Nivel = NivelFromBD;
                }
            }

            if (CheckZoomHasChanged())
            {
                if(zoomFromBD > 0 && zoomFromBD <= 4) gridScript.ZoomFromBD = zoomFromBD;
            }

            if (CheckCurrentPageHasChanged())
            {
                gridScript.CurrentPageFromBD = currentPageFromBD;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator CR_NavigationChanged() {
        while (true)
        {
            
            if (CheckEmpresaHasChanged())
            {
                if(EmpresaIdFromBD != "-1" && EmpresaIdFromBD != "0")
                {
                    EmpresaID = EmpresaIdFromBD;
                    print("Navego hacia granjas con empresa ID: " + EmpresaID);
                    JumpToComponent("granja");
                } 
            }
            
            if (CheckGranjaHasChanged())
            {
                if(GranjaIdFromBD != "-1" && GranjaIdFromBD != "0")
                {
                    GranjaID = GranjaIdFromBD;
                    print("Navego hacia nucleo con granja ID: " + GranjaID);
                    JumpToComponent("nucleo");
                }
            }
            
            if (CheckNucleoHasChanged())
            {
                if (NucleoIdFromBD != "-1" && NucleoIdFromBD != "0")
                {
                    NucleoID = NucleoIdFromBD;
                    print("Navego hacia galpon con gnucleo ID: " + NucleoID);
                    JumpToComponent("galpon");
                }
            }
           
            yield return new WaitForSeconds(1.0f);
        }
    }
    //==========================================================================================
    // Navigation Functions
    //==========================================================================================

    public void SetGridCanales(bool state) //Changes between Main Screen and Grid Screen
    {
        gridCanales.SetActive(state);
        if (state)
        {
            gridScript.CleanGrid();
            grid.SetActive(false);
        }
        else
        {
            grid.SetActive(true);
        }
    }

    private bool RemoteControlHasChanged(ref string newvalor)
    {
        if (storedRemoteControlFromBD != newvalor)
        {
            storedRemoteControlFromBD = newvalor;
            return true;
        }

        storedRemoteControlFromBD = newvalor;
        return false;
    }
    private bool CheckRemoteControlHasChanged()
    {
        return RemoteControlHasChanged(ref remoteControlFromBD);
    }
    void ParseData()
    {
        string[] items;

        items = remoteControlFromBD.Split(';');

        CanalFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "canal:"));
        NivelFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "nivel:"));
        EmpresaIdFromBD = AuxFunctions.GetDataValue(items[0], "idempresa:");
        GranjaIdFromBD = AuxFunctions.GetDataValue(items[0], "idgranja:");
        NucleoIdFromBD = AuxFunctions.GetDataValue(items[0], "idnucleo:");
        //GalponIdFromBD = AuxFunctions.GetDataValue(items[0], "idgalpon:");
        //SiloID = AuxFunctions.GetDataValue(items[0], "idsilo:");

        activeElem = int.Parse(AuxFunctions.GetDataValue(items[0], "activa:"));
        zoomFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "zoom:"));
        currentPageFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "pagina:"));
        totalPagesFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "cantidadpag:"));
 
        flagFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "flag:"));

    }
    private IEnumerator CR_ReviewRemoteControl()
    {
        WWW remoteControlQuery;

        while (true)
        {
            remoteControlQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarControlRemoto.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return remoteControlQuery;

            if (remoteControlQuery.error == null)
            {
                //print(remoteControlQuery.text);

                remoteControlFromBD = remoteControlQuery.text;

                if (CheckRemoteControlHasChanged())
                {
                    ParseData();
                }
            }
            else
            {
                print("(Galpones) Server Error: (" + remoteControlQuery.error + ")");
            }

            yield return new WaitForSeconds(1.0f);
        }
        
    }       

    //==========================================================================================
    // Main Functions
    //==========================================================================================

    public void JumpToComponent(string type)
    {
        StopCoroutine("Instantiate");
        gridScript.CleanGrid();

        switch (type)
        {

            case "back":
                if(NivelFromBD == 0)
                {
                    SetGridCanales(true);
                    InitializeBD();
                    ServerClass.SendFlagToServer(1);
                }
                if(Nivel == 1)
                {
                    SetGridCanales(true);
                    empresaBtn.interactable = false;
                    empresaImg.color = disabledColor;
                    InitializeBD();
                    ServerClass.SendFlagToServer(1);
                    print("back from empresa");
                }

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
                    print("back from galpon"); 
                    JumpToComponent("nucleo");
                }

                break;

            case "empresa":

                Nivel = 1;
                StartCoroutine(Instantiate("empresa", ""));

                EmpresaID = "-1";
                GranjaID = "-1";
                NucleoID = "-1";
                GalponID = "-1";

                //navigationPath.Remove("granja");
                //navigationPath.Remove("nucleo");
                //navigationPath.Remove("galpon");

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

                Nivel = 2;
                StartCoroutine(Instantiate("granja", EmpresaID));

                GranjaID = "-1";
                NucleoID = "-1";
                GalponID = "-1";

                //navigationPath.Remove("granja");
                //navigationPath.Remove("galpon");

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

                Nivel = 3;
                StartCoroutine(Instantiate("nucleo", GranjaID));

                NucleoID = "-1";
                GalponID = "-1";

                //navigationPath.Remove("galpon");

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

                Nivel = 4;
                StartCoroutine(Instantiate("galpon", NucleoID));

                GalponID = "-1";

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

    private IEnumerator Instantiate(string type, string ID)
    {
        string prefabPath = "", instanceServerPath="";
        switch (type){

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
                themeScript.gridName = gridName;
                themeScript.themeLocation = "Fondo";
                themeScript.Initialize();
                break;

        }

        gridScript.type = type;
        gridScript.itemsOnServerPath = instanceServerPath;
        gridScript.serverpath = "https://ir7b164b04a7.us1.hana.ondemand.com";
        gridScript.prefabLocalPath = prefabPath;
        StartCoroutine(gridScript.Initialize());

        yield return new WaitUntil(() => gridScript.Ready); 
    }

    //==========================================================================================
    // Server Talkers
    //==========================================================================================

    //==========================================================================================
    // Galpones - Sheds
    //==========================================================================================
    
        /*
    private void ActivateGalponesCheckForUpdates()
    {
        StartCoroutine(CR_CheckGalponesDataOnBD());
        StartCoroutine(CR_GalponesDataHasChanged());
    }
    private bool GalponesDataHasChanged(ref string newvalor)
    {
        if (storedGalponesDataFromBD != newvalor)
        {
            storedGalponesDataFromBD = newvalor;
            return true;
        }

        storedGalponesDataFromBD = newvalor;
        return false;
    }
    private bool CheckGalponesDataHasChanged()
    {
        return GalponesDataHasChanged(ref galponesDataFromBD);
    }
    private IEnumerator CR_GalponesDataHasChanged()
    {
        while (true)
        {
            if (CheckGalponesDataHasChanged()) {
                //ChangeGalponesData();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
    public void ChangeGalponesData() //Send farm data to elements
    {
        foreach (Transform child in GameObject.Find(gridName).transform)
        {
            if (!child.name.Contains("EmptySpace"))
            {
                child.GetComponent<GalponElementScript>().changeData = true;
                child.GetComponent<GalponElementScript>().itemsData = galponesDataFromBD;
            }
        }
    }
    private IEnumerator CR_CheckGalponesDataOnBD()
    {
        WWW elementsQuery;
        while (true)
        {
            elementsQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarElementos.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");
            
            yield return elementsQuery;

            if (elementsQuery.error == null)
            {
                galponesDataFromBD = elementsQuery.text;
            }
            else
            {
                print("(Galpones) Server Error: (" + elementsQuery.error + ")");
            }

            yield return new WaitForSeconds(2.0f);
        }
    }
    */

    /* Favorites Section */
    public int IdEmpresaFav;
    public int IdGranjaFav;
    public int IdNucleoFav;
    public int IdGalponFav;
    public int IdSiloFav;
    public string favoritesFromBD;
    public string storedFavoritesFromBD;

    private void StartFavorites(bool state)
    {
        if (state)
        {
            IdEmpresaFav = -1;
            IdGranjaFav = -1;
            IdNucleoFav = -1;
            IdGalponFav = -1;
            IdSiloFav = -1;
            favoritesFromBD = "-1";
            storedFavoritesFromBD = "-1";
            StartCoroutine(CR_ReviewFavoritesFromBD());
        }
        else
        {
            StopCoroutine(CR_ReviewFavoritesFromBD());
        }
    }
    private bool FavoritesHasChanged(ref string newvalor)
    {
        if (storedFavoritesFromBD != newvalor)
        {
            storedFavoritesFromBD = newvalor;
            return true;
        }

        storedFavoritesFromBD = newvalor;
        return false;
    }
    private bool CheckFavoritesHasChanged()
    {
        return FavoritesHasChanged(ref favoritesFromBD);
    }
    private void ParseFavoritesFromBD()
    {
        string[] items;
        items = favoritesFromBD.Split(';');
        int itemsLength = items.Length - 2;
        for(int i = 0; i<= itemsLength; i++) {
            IdEmpresaFav = int.Parse(AuxFunctions.GetDataValue(items[i], "idempresa:"));
            IdGranjaFav = int.Parse(AuxFunctions.GetDataValue(items[i], "idgranja:"));
            IdNucleoFav = int.Parse(AuxFunctions.GetDataValue(items[i], "idnucleo:"));
            IdGalponFav = int.Parse(AuxFunctions.GetDataValue(items[i], "idgalpon:"));
            IdSiloFav = int.Parse(AuxFunctions.GetDataValue(items[i], "idsilo:"));
            print(IdEmpresaFav + "/" + IdGranjaFav + "/" + IdNucleoFav + "/" + IdGalponFav + "/" + IdSiloFav);
        }

    }
    private IEnumerator CR_ReviewFavoritesFromBD()
    {
        WWW FavoritesQuery;

        while (true)
        {
            FavoritesQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarFavoritosResumen.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return FavoritesQuery;

            if (FavoritesQuery.error == null)
            {
                //print(remoteControlQuery.text);

                favoritesFromBD = FavoritesQuery.text;

                if (CheckFavoritesHasChanged())
                {
                    ParseFavoritesFromBD();
                }
            }
            else
            {
                print("(Favorites) Server Error: (" + FavoritesQuery.error + ")");
            }

            yield return new WaitForSeconds(1.0f);
        }

    }

}
