using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using LitJson;
using SimpleFarmNamespace;

// Grid Class

public class GridClass : MonoBehaviour
{
    private void Update()
    {
        print("Grid Ready: " + Ready + "/ Grid Ready Instantiate: " + readyInstantiate);
    }

    public int NumberActiveElements; //Local number of active elements.

    //Identity and states

    public bool Online;
    public bool OnlineFromBD
    {
        get
        {
            return Online;
        }

        set
        {
            if (AuxFunctions.ValueChanged("online", Online, value))
            {
                Online = value;
            }
        }
    }

    public bool Ready; //Check if Instantiation its correct
    public bool readyInstantiate;
    public bool firstTime; //To first time initialize values

    public string type;
    public string authorization;
    public string serverpath; 
    public string itemsOnServerPath;
    public string prefabLocalPath;

    //Local Variables

    WindowManager WinManagerScript;
    Scrollbar GridScrollbar; //Scrollbar value

    public string ZoomLevel; //Zoom level local
    public int Zoom;
    public int ZoomFromBD
    {
        get
        {
            return Zoom;
        }

        set
        {
            if (Ready)
            {
                if (AuxFunctions.ValueChanged("zoom", Zoom, value))
                {
                    if (value > Zoom)
                    {
                        switch (value)
                        {
                            case 2:
                                SetZoom("75", true);
                                break;

                            case 3:
                                SetZoom("50", true);
                                break;
                            case 4:
                                SetZoom("25", true);
                                break;
                        }
                    }
                    else
                    {
                        if (value < Zoom)
                        {
                            switch (value)
                            {
                                case 3:
                                    SetZoom("25", false);
                                    break;

                                case 2:
                                    SetZoom("50", false);
                                    break;
                                case 1:
                                    SetZoom("75", false);
                                    break;
                            }
                        }
                    }

                    Zoom = value;
                    ServerClass.SendFlagToServer(1);
                    print("Mandando Flag en 1 desde cambio de Zoom");
                }
            }   
        }
    }

    public int CurrentPage;
    public int CurrentPageFromBD
    {
        get
        {
            return CurrentPage;
        }

        set
        {
            if (Ready)
            {
                if (AuxFunctions.ValueChanged("current", CurrentPage, value))
                {
                    if (value > CurrentPage)
                    {
                        JumpPage("down");
                    }

                    if (value < CurrentPage)
                    {
                        JumpPage("up");
                    }

                    CurrentPage = value;
                }
            }
        }
    }

    public int TotalPages; //Local number of pages corresponding to zoom level.
    public float JumpPageLevel; //Magnitude of scroll jump

    //Debugging
    private Text onlineView;
    private Button setPages;
    private Text zoomLevelView; //Zoom level in screen
    private Button zoomIn; //Button for zoom in
    private Button zoomOut; //Button for zoom out
    private Button next;
    private Button prev;

    private Text currentPageView; //Current Page in screen
    private Text totalPagesView;

    private void Awake()
    {
        if (gameObject.GetComponent<WindowManager>() == null)
        {
            gameObject.AddComponent<WindowManager>();
        }
    }

    // Interface

    public IEnumerator Initialize()
    {
        InitializeValues();

        yield return new WaitForSeconds(0.5f);

        WinManagerScript.InstanceAndShowPopUp("temporal", "BasicPopUpPrefab", "SimpleFarmCanvas", "Cargando", 1.0f, 0.5f);

        yield return new WaitUntil(() => Online);

        if (type != "" && itemsOnServerPath != "" && prefabLocalPath != "")
        {
            StartCoroutine(DataInstantiate(type, itemsOnServerPath, prefabLocalPath));
        }

        yield return new WaitUntil(() => (readyInstantiate));

        InitializeBD();

        CheckActiveElems();
        CheckPagination();

        yield return new WaitForSeconds(0.5f);

        Ready = true;
        print("Grid Ready");
    }

    void InitializeValues()
    {
        //Scripts

        WinManagerScript = gameObject.GetComponent<WindowManager>();
        GridScrollbar = GameObject.Find("GridScrollbar").GetComponent<Scrollbar>();

        //Properties

        NumberActiveElements = 0;

        Ready = false;
        firstTime = true;
        readyInstantiate = false;

        Online = false;

        ZoomLevel = "100";
        ZoomFromBD = 1;

        CurrentPage = 1;

        TotalPages = 0;
        JumpPageLevel = 0;

        //Debugging Interface

        zoomLevelView = GameObject.Find("zoomLevel").transform.Find("Text").GetComponent<Text>();
        zoomLevelView.text = "100";

        currentPageView = GameObject.Find("current").GetComponent<Text>();
        currentPageView.text = "1";

        totalPagesView = GameObject.Find("Pagination").transform.Find("total").GetComponent<Text>();
        totalPagesView.text = "0";

        zoomIn = GameObject.Find("zoomIn-btn").GetComponent<Button>();
        zoomIn.interactable = false;

        zoomOut = GameObject.Find("zoomOut-btn").GetComponent<Button>();
        zoomOut.onClick.AddListener(() => SetZoom("75", true));

        setPages = GameObject.Find("setPages-btn").GetComponent<Button>();
        setPages.onClick.AddListener(() => CheckActiveElems());
        setPages.onClick.AddListener(() => CheckPagination());

        next = GameObject.Find("nextPage-btn").GetComponent<Button>();
        next.onClick.AddListener(() => JumpPage("down"));
        prev = GameObject.Find("prevPage-btn").GetComponent<Button>();
        prev.onClick.AddListener(() => JumpPage("up"));
    }

    private void InitializeBD()
    {
        ServerClass.SendZoomToServer(1);
        ServerClass.SendCurrentPageToServer(1);
    }

    public IEnumerator DataInstantiate(string type, string itemsOnServerPath, string prefabPath)
    { 
        int numItems;
        string itemsFromBD = "";
        JsonData jsonvale;

        authorization = AuxFunctions.Authenticate("MLORETO", "Marco.2017");

        UnityWebRequest www = UnityWebRequest.Get(itemsOnServerPath);
        www.SetRequestHeader("AUTHORIZATION", authorization);

        yield return www.Send();

        DestroyEmpty();

        if (www.error == null)
        {
            itemsFromBD = www.downloadHandler.text;
            jsonvale = JsonMapper.ToObject(itemsFromBD);
            numItems = jsonvale["d"]["results"].Count;

            GameObject prefabInstance = Resources.Load(prefabPath) as GameObject;
            if (numItems > 0)
            {    
                for (int i = 0; i < numItems; i++)
                {
                    GameObject DataInstance = Instantiate(prefabInstance, GameObject.Find("Grid").transform) as GameObject;
                    DataInstance.name = type + i;
                    switch (type)
                    {
                        case "empresa":
                            DataInstance.transform.Find("Fondo").transform.Find("Titulo").GetComponent<Text>().text = jsonvale["d"]["results"][i]["NAME"].ToString();
                            DataInstance.GetComponent<EmpresaElementScript>().empresaID = jsonvale["d"]["results"][i]["PARTNERSHIPID"].ToString();
                            break;

                        case "granja":
                            DataInstance.transform.Find("Fondo").transform.Find("Titulo").GetComponent<Text>().text = jsonvale["d"]["results"][i]["FARMNAME"].ToString();
                            DataInstance.GetComponent<GranjaElementScript>().granjaID = jsonvale["d"]["results"][i]["BROILERSFARMID"].ToString();
                            break;

                        case "nucleo":
                            DataInstance.transform.Find("Fondo").transform.Find("Titulo").GetComponent<Text>().text = jsonvale["d"]["results"][i]["NAME"].ToString();
                            DataInstance.GetComponent<NucleoElementScript>().nucleoID = jsonvale["d"]["results"][i]["CENTERID"].ToString();
                            break;

                        case "galpon":
                            DataInstance.GetComponent<GalponElementScript>().id = i;
                            DataInstance.GetComponent<GalponElementScript>().galponID = jsonvale["d"]["results"][i]["SHEDCODE"].ToString();
                            DataInstance.transform.Find("QR").GetComponent<RawImage>().texture = AuxFunctions.generateQR(i.ToString());
                            //DataInstance.GetComponent<GalponElementScript>().Initialize();
                            break;
                    }

                }
            }else
            {
                GameObject DataInstance = Instantiate(prefabInstance, GameObject.Find("Grid").transform) as GameObject;
                DataInstance.transform.Find("Fondo").transform.Find("Titulo").GetComponent<Text>().text = "Vacio";
            }
            readyInstantiate = true;
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
    }

    // Main Functions

    public void DestroyEmpty()
    {
        foreach (Transform child in GameObject.Find("Grid").transform)
        {
            if (child.name.Contains("Empty"))
            {
                Destroy(GameObject.Find(child.name));
            }
        }
    }

    public void DestroyElems()
    {
        foreach (Transform child in GameObject.Find("Grid").transform)
        {
            Destroy(GameObject.Find(child.name));
        }
    }

    public void CheckActiveElems() //Set number of active elements.
    {
        int acts = 0;
        GameObject grid = GameObject.Find("Grid");
        if (grid.transform.childCount > 0)
        {
            foreach (Transform child in grid.transform)
            {
                if (!child.name.Contains("EmptySpace"))
                {
                    acts++;
                }
            }
        }
        NumberActiveElements = acts;
    }

    public void SetZoom(string zoomValue, bool zoom)
    {
        //Standard size its 100% = 500x250px

        Animation anim = GameObject.Find("Grid").GetComponent<Animation>();

        DestroyEmpty();

        switch (zoomValue)
        {

            case "75":
                if (zoom)
                {
                    anim.Play("Grid-Zoom-100-75");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomOut.onClick.AddListener(() => SetZoom("50", true));
                    zoomIn.onClick.AddListener(() => SetZoom("75", false));
                    ZoomLevel = "75";
                }
                else
                {
                    anim.Play("Grid-Zoom-75-100");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomOut.onClick.AddListener(() => SetZoom("75", true));
                    ZoomLevel = "100";
                }
                break;
            case "50":
                if (zoom)
                {
                    anim.Play("Grid-Zoom-75-50");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomOut.onClick.AddListener(() => SetZoom("25", true));
                    zoomIn.onClick.AddListener(() => SetZoom("50", false));
                    ZoomLevel = "50";
                }
                else
                {
                    anim.Play("Grid-Zoom-50-75");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomOut.onClick.AddListener(() => SetZoom("50", true));
                    zoomIn.onClick.AddListener(() => SetZoom("75", false));
                    ZoomLevel = "75";
                }

                break;
            case "25":
                if (zoom)
                {
                    anim.Play("Grid-Zoom-50-25");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomIn.onClick.AddListener(() => SetZoom("25", false));
                    ZoomLevel = "25";
                }
                else
                {
                    anim.Play("Grid-Zoom-25-50");
                    zoomIn.onClick.RemoveAllListeners();
                    zoomOut.onClick.RemoveAllListeners();
                    zoomOut.onClick.AddListener(() => SetZoom("25", true));
                    zoomIn.onClick.AddListener(() => SetZoom("50", false));
                    ZoomLevel = "50";
                }
                break;
        }

        if (JumpPageLevel > 0)
        {
            StartCoroutine(LerpScrollbarToStart(1.0f));
            StopCoroutine("LerpScrollbarToStart");
        }

        if (zoomValue == "limpiar")
        {
            anim.Play("Grid-Zoom-75-100");
            ZoomLevel = "100";
        }
        else
        {
            zoomLevelView.text = ZoomLevel.ToString();
            CurrentPage = 1;
            currentPageView.text = "1";
            CheckPagination();
            ServerClass.SendFlagToServer(1);
            print("Mandando Flag en 1 desde cambio de Pagina");
        }
    }

    public void CheckPagination()
    {
        switch (ZoomLevel)
        {
            case "100":
                //in this case pagination shows max 9 elems per page
                SetPagination(9);
                break;
            case "75":
                SetPagination(16);
                //in this case pagination shows max 16 elems per page
                break;
            case "50":
                //in this case pagination shows max 25 elems per page
                SetPagination(25);
                break;
            case "25":
                //in this case pagination shows max 36 elems per page
                SetPagination(36);
                break;
        }
    }

    public void SetPagination(int numShowElems) //Determines and set the number of pages in grid regarding of number of elements in view
    {
        int aux, mod;

        JumpPageLevel = 0;
        TotalPages = NumberActiveElements / numShowElems;
        mod = NumberActiveElements % numShowElems;

        if (mod > 0)
        {
            TotalPages += 1;
            aux = numShowElems - mod;
            for (int i = 1; i <= aux; i++)
            {
                GameObject prefabEmpty = Resources.Load("Prefabs/PopupPrefab/EmptySpace") as GameObject;
                GameObject empty = Instantiate(prefabEmpty, GameObject.Find("Grid").transform) as GameObject;
                empty.name = "EmptySpace" + i;
            }
        }

        if (TotalPages > 1)
        {
            JumpPageLevel = (float)(1 / ((float)TotalPages - 1));
            JumpPageLevel = JumpPageLevel + 0.0002f;
        }

        /*UNCOMMENT IF SEND VARIABLES TO SERVER*/

        ServerClass.SendCurrentPageToServer(1);
        ServerClass.SendTotalPagesToServer(TotalPages);
        totalPagesView.text = TotalPages.ToString();

        if (firstTime)
            firstTime = false;
        else
        {
            ServerClass.SendFlagToServer(1);
            print("Mandando Flag en 1 desde SET PAGINATION");
        }

        /*UNCOMMENT IF SEND VARIABLES TO SERVER*/
    }

    public void JumpPage(string type) //Moves de scrollbar with the value of scrollJump
    {
        float aux;

        switch (type)
        {
            case "down":
                if (GridScrollbar.value != 0)
                {
                    if (CurrentPage < TotalPages)
                    {
                        CurrentPage += 1;
                        aux = GridScrollbar.value - JumpPageLevel;
                        StartCoroutine(LerpFloatTo(aux, 1.0f));
                        StopCoroutine("LerpFloatTo");
                    }
                }
                break;

            case "up":
                if (GridScrollbar.value != 1)
                {
                    if (CurrentPage > 1)
                    {
                        CurrentPage -= 1;
                        aux = GridScrollbar.value + JumpPageLevel;
                        StartCoroutine(LerpFloatTo(aux, 1.0f));
                        StopCoroutine("LerpFloatTo");
                    }
                }
                break;
        }
        currentPageView.text = CurrentPage.ToString();
        ServerClass.SendFlagToServer(1);
        print("Mandando Flag en 1 desde cambio de Pagina");
    }

    // Grid Aux Functions

    public IEnumerator LerpScrollbarToStart(float aTime) // Move Scrollbar to top if value less than 1.
    {
        GridScrollbar = GameObject.Find("GridScrollbar").GetComponent<Scrollbar>();
        if (GridScrollbar.value < 1)
        {
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
            {
                GridScrollbar.value = Mathf.Lerp(GridScrollbar.value, 1.0f, t);
                yield return null;
            }
            GridScrollbar.value = 1.0f;
        }
    }

    public IEnumerator LerpFloatTo(float end, float atime) // Moves scrollbar value to previous or next page
    {
        GameObject.Find("nextPage-btn").GetComponent<Button>().interactable = false;
        GameObject.Find("prevPage-btn").GetComponent<Button>().interactable = false;
        for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / atime)
        {
            GridScrollbar.value = Mathf.Lerp(GridScrollbar.value, end, t);
            yield return null;
        }
        GameObject.Find("nextPage-btn").GetComponent<Button>().interactable = true;
        GameObject.Find("prevPage-btn").GetComponent<Button>().interactable = true;
    }

    // Destructors

    public void CleanGrid()
    {
        Ready = false;
        readyInstantiate = false;
        StopAllCoroutines();
        DestroyElems();
        CheckActiveElems();
        zoomIn.onClick.RemoveAllListeners();
        zoomOut.onClick.RemoveAllListeners();
        setPages.onClick.RemoveAllListeners();
        next.onClick.RemoveAllListeners();
        prev.onClick.RemoveAllListeners();
        SetZoom("limpiar", true);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
