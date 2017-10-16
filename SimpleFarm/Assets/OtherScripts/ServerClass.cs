using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFarmNamespace;

public class ServerClass : MonoBehaviour{

    private void Update()
    {
        print("Canal: " + Canal+ " /Nivel: " + Nivel + "/Zoom: " + Zoom + "/CurrentPage: " + CurrentPage + "/Flag: " + Flag);
        print(Favorites);
    }

    public bool Ready;

    private Text onlineView;
    public bool Online;
    public bool OnlineFromBD
    {
        get
        {
            return Online;
        }

        set
        {
            if (Ready)
                GridScript.OnlineFromBD = value; //Send Online status to grid

            if (AuxFunctions.ValueChanged("online", Online, value))//Change ui online status
            {
                Online = value;

                if (value == true)
                {
                    onlineView.text = "Online";
                    AuxFunctions.ChangeColor("Online", AuxFunctions.ConvertColorRGBA(103, 221, 127, 1), 1f);
                }
                else
                {
                    onlineView.text = "Offline";
                    AuxFunctions.ChangeColor("Online", AuxFunctions.ConvertColorRGBA(247, 125, 115, 1), 1f);
                }
            }
        }
    }

    /* Scripts */

    private AppControllerScript AppController;
    private GridClass GridScript;

    /* Path Navigation */

    public int Canal;
    private int CanalFromBD
    {
        get
        {
            return Canal;
        }

        set
        {
            if (AuxFunctions.ValueChanged("canal", Canal, value))
            {
                Canal = value;
                if (Ready)
                    AppController.CanalFromBD = value;
            }
               
        }
    }

    public int Nivel;
    private int NivelFromBD
    {
        get
        {
            return Nivel;
        }

        set
        {
            if (AuxFunctions.ValueChanged("nivel", Nivel, value))
            {
                Nivel = value;
                if (Ready)
                    AppController.NivelFromBD = value;
            }
                
        }
    }

    public int EmpresaID;
    private int EmpresaIdFromBD
    {
        get
        {
            return EmpresaID;
        }

        set
        {
            if(AuxFunctions.ValueChanged("empresa", EmpresaID, value))
            {
                EmpresaID = value;
                if (Ready)
                    AppController.EmpresaIdFromBD = value;
            }
        }
    }

    public int GranjaID;
    private int GranjaIdFromBD
    {
        get
        {
            return GranjaID;
        }

        set
        {
            if(AuxFunctions.ValueChanged("granja", GranjaID, value))
            {
                GranjaID = value;
                if (Ready)
                    AppController.GranjaIdFromBD = value;
            }       
        }
    }

    public int NucleoID;
    private int NucleoIdFromBD
    {
        get
        {
            return NucleoID;
        }

        set
        {
            if(AuxFunctions.ValueChanged("nucleo", NucleoID, value))
            {
                NucleoID = value;
                if (Ready)
                    AppController.NucleoIdFromBD = value;
            }
                
        }
    }

    public int GalponID;
    private int GalponIdFromBD
    {
        get
        {
            return GalponID;
        }

        set
        {
            if(AuxFunctions.ValueChanged("galpon", GalponID, value))
            {
                GalponID = value;
                if (Ready)
                    AppController.GalponIdFromBD = value;
            }  
        }
    }

    public int SiloID;
    private int SiloIdFromBD
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
                if (Ready)
                    AppController.SiloIdFromBD = value;
            } 
        }
    }

    /* Grid Navigation */

    public int ActiveElem;
    private  int ActiveElemFromBD
    {
        get
        {
            return ActiveElem;
        }

        set
        {
            if (AuxFunctions.ValueChanged("active", ActiveElem, value))
            {
                ActiveElem = value;
            }
        }
    }

    public int Zoom;
    private int ZoomFromBD
    {
        get
        {
            return Zoom;
        }

        set
        {
            if (AuxFunctions.ValueChanged("zoom", Zoom, value))
            {
                Zoom = value;
                if (Ready)
                    GridScript.ZoomFromBD = value;
            }
        }
    }

    public int CurrentPage;
    private int CurrentPageFromBD
    {
        get
        {
            return CurrentPage;
        }

        set
        {
            if(AuxFunctions.ValueChanged("current", CurrentPage, value))
            {
                CurrentPage = value;
                if (Ready)
                    GridScript.CurrentPageFromBD = value;
            }    
        }
    }

    public int TotalPages;
    private int TotalPagesFromBD
    {
        get
        {
            return TotalPages;
        }

        set
        {
            if (AuxFunctions.ValueChanged("total", TotalPages, value))
            {
                TotalPages = value;
            }
        }
    }

    public int Flag;
    private int FlagFromBD
    {
        get
        {
            return Flag;
        }

        set
        {
            if (AuxFunctions.ValueChanged("flag", Flag, value))
            {
                Flag = value;
                //AppController.FlagFromBD = value;
            }
        }

    }

    private string RemoteControl;
    private string RemoteControlFromBD
    {
        get
        {
            return RemoteControl;
        }

        set
        {
            if (AuxFunctions.ValueChanged("remote", RemoteControl, value))
            {
                RemoteControl = value;
                ParseRemoteControl();
            }
        }
    }

    /* Favorites */
    public int NumFavs;

    private int EmpresaFav;
    private int EmpresaFavFromBD
    {
        get
        {
            return EmpresaFav;
        }

        set
        {
            if(AuxFunctions.ValueChanged("empresafav", EmpresaFav, value))
            {
                EmpresaFav = value;
            }
        }
    }

    private int GranjaFav;
    private int GranjaFavFromBD
    {
        get
        {
            return GranjaFav;
        }

        set
        {
            if (AuxFunctions.ValueChanged("granjafav", GranjaFav, value))
            {
                GranjaFav = value;
            }
        }
    }

    private int NucleoFav;
    private int NucleoFavFromBD
    {
        get
        {
            return NucleoFav;
        }

        set
        {
            if (AuxFunctions.ValueChanged("nucleofav", NucleoFav, value))
            {
                NucleoFav = value;
            }
        }
    }

    private int GalponFav;
    private int GalponFavFromBD
    {
        get
        {
            return GalponFav;
        }

        set
        {
            if (AuxFunctions.ValueChanged("galponfav", GalponFav, value))
            {
                GalponFav = value;
            }
        }
    }

    private int SiloFav;
    private int SiloFavFromBD
    {
        get
        {
            return SiloFav;
        }

        set
        {
            if (AuxFunctions.ValueChanged("silofav", SiloFav, value))
            {
                SiloFav = value;
            }
        }
    }

    private string Favorites;
    private string FavoritesFromBD
    {
        get
        {
            return Favorites;
        }

        set
        {
            if (AuxFunctions.ValueChanged("favorites", Favorites, value))
            {
                Favorites = value;
                ParseFavorites();
            }
        }
    }

    /* Interface */

    public void Play()
    {
        StartCoroutine(Initialize());
    }

    public void Stop()
    {
        StopCoroutine("CR_GetRemoteControl");
        StopCoroutine("CR_GetFavorites");
        Ready = false;
    }

    public void ResetBD()
    {
        WWW web = new WWW("https://commendable-rigs.000webhostapp.com/inicializarNavegacion.php");
    }

    private IEnumerator Initialize()
    {
        InitializeLocalValues();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(CR_GetConnectionStatus());

        yield return new WaitUntil(() => Online);

        ResetBD();

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(CR_GetRemoteControl());

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(CR_GetFavorites());

        yield return new WaitForSeconds(1.0f);

        Ready = true;
        print("Server Ready!");
    }

    private void InitializeLocalValues()
    {

        //Properties

        Ready = false;
        Online = false;
        RemoteControl = "";
        Favorites = "";

        //Navigation Values

        Canal = 0;
        Nivel = 0;
        EmpresaID = -1;
        GranjaID = -1;
        NucleoID = -1;
        GalponID = -1;
        SiloID = -1;

        ActiveElem = -1;
        Zoom = 1;
        CurrentPage = 1;
        TotalPages = 0;
        Flag = 0;

        //Get Scripts

        AppController = gameObject.GetComponent<AppControllerScript>();
        GridScript = gameObject.GetComponent<GridClass>();

        //UI Values

        onlineView = GameObject.Find("Online").transform.Find("Text").GetComponent<Text>();
        onlineView.text = "Offline";
    }   

    /* Server Talkers */

    /* Receive */

    private IEnumerator CR_GetConnectionStatus()
    {
        WWW OnlineStatus;

        while (true)
        {
            OnlineStatus = new WWW("https://commendable-rigs.000webhostapp.com");

            yield return OnlineStatus;

            if (OnlineStatus.error == null)
            {
                OnlineFromBD = true;
            }else
            {
                OnlineFromBD = false;
            }
                
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void ParseRemoteControl()
    {
        string[] items;

        items = RemoteControl.Split(';');

        CanalFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "canal:"));
        NivelFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "nivel:"));
        EmpresaIdFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "idempresa:"));
        GranjaIdFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "idgranja:"));
        NucleoIdFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "idnucleo:"));
        GalponIdFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "idgalpon:"));
        SiloIdFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "idsilo:"));
        ActiveElemFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "activa:"));
        ZoomFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "zoom:"));
        CurrentPageFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "pagina:"));
        TotalPagesFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "cantidadpag:"));
        FlagFromBD = int.Parse(AuxFunctions.GetDataValue(items[0], "flag:"));
    }
    private IEnumerator CR_GetRemoteControl()
    {
        WWW RemoteControlQuery;

        while (true)
        {
            RemoteControlQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarControlRemoto.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return RemoteControlQuery;

            if (RemoteControlQuery.error == null)
            {
                RemoteControlFromBD = RemoteControlQuery.text;
            }
            else
            {
                print("Server Error (Remote Control): (" + RemoteControlQuery.error + ")");
            }

            yield return new WaitForSeconds(1.0f);
        }

    }

    private void ParseFavorites()
    {
        string[] items;
        items = FavoritesFromBD.Split(';');
        int itemsLength = items.Length - 1;
        for (int i = 1; i < itemsLength; i++)
        {
            EmpresaFavFromBD = int.Parse(AuxFunctions.GetDataValue(items[i], "idempresa:"));
            GranjaFavFromBD = int.Parse(AuxFunctions.GetDataValue(items[i], "idgranja:"));
            NucleoFavFromBD = int.Parse(AuxFunctions.GetDataValue(items[i], "idnucleo:"));
            GalponFavFromBD = int.Parse(AuxFunctions.GetDataValue(items[i], "idgalpon:"));
            SiloFavFromBD = int.Parse(AuxFunctions.GetDataValue(items[i], "idsilo:"));
        }
    }
    private IEnumerator CR_GetFavorites()
    {
        WWW FavoritesQuery;

        while (true)
        {
            FavoritesQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarFavoritosResumen.php");
            //WWW zoomQuery = new WWW("http://localhost/indicadores/revisarElementos.php");

            yield return FavoritesQuery;

            if (FavoritesQuery.error == null)
            {
                FavoritesFromBD = FavoritesQuery.text;
            }
            else
            {
                print("(Favorites) Server Error: (" + FavoritesQuery.error + ")");
            }

            yield return new WaitForSeconds(1.0f);
        }

    }

    /* Send */

    static public void SendZoomToServer(int zoom)
    {
        WWWForm form = new WWWForm();
        form.AddField("zoom", zoom);
        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarZoom.php", form);
        //WWW www = new WWW("http://localhost/indicadores/insertarZoom.php", form);
    }

    static public void SendCurrentPageToServer(int currentPage)
    {
        WWWForm form = new WWWForm();
        form.AddField("currentPage", currentPage);
        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarPaginaActual.php", form);
        //WWW www = new WWW("http://localhost/indicadores/insertarPaginaActual.php", form);
    }

    static public void SendTotalPagesToServer(int totalPages)
    {
        WWWForm form = new WWWForm();
        form.AddField("totalPages", totalPages);
        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarTotalPaginas.php", form);
        //WWW www = new WWW("http://localhost/indicadores/insertarTotalPaginas.php", form);
        //print("Sending Total Pages Error: " + www.isDone);
    }

    static public void SendFlagToServer(int value)
    {
        WWWForm form = new WWWForm();
        form.AddField("flag", value);
        WWW www = new WWW("https://commendable-rigs.000webhostapp.com/insertarFlag.php", form);
        //WWW www = new WWW("http://localhost/indicadores/insertarFlag.php", form);
        //print("Sending Flag Current Error: " + www.isDone);
    }
}
