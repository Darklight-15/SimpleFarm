using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using SimpleFarmNamespace;


public class TarjetaFullScreen : MonoBehaviour {

    public string itemsTarjetaFullScreenString;

    private int themeFromBD;
    private int idtarjeta;
    private string[] items;
    private string itemsTarjetasString;
    
    private int storedThemeFromBD;

    private string ConfortBD;
    private string DensidadBD;
    private string TemperaturaBD;
    private string MortalidadBD;
    private string ConsumoBD;
    private string EficienciaBD;
    private string InventarioBD;

    private CanvasGroup ConfortA;
    private CanvasGroup ConfortL;
    private CanvasGroup ConfortP;
    private CanvasGroup ConfortE;

    private CanvasGroup DensidadB;
    private CanvasGroup DensidadM;
    private CanvasGroup DensidadA;

    private CanvasGroup TemperaturaEF;
    private CanvasGroup TemperaturaF;
    private CanvasGroup TemperaturaT;
    private CanvasGroup TemperaturaC;
    private CanvasGroup TemperaturaEC;

    private CanvasGroup MortalidadN;
    private CanvasGroup MortalidadA;

    private CanvasGroup ConsumoB;
    private CanvasGroup ConsumoN;
    private CanvasGroup ConsumoA;
    private CanvasGroup ConsumoI;

    private CanvasGroup Eficiencia1;
    private CanvasGroup Eficiencia2;
    private CanvasGroup Eficiencia3;
    private CanvasGroup Eficiencia4;
    private CanvasGroup Eficiencia5;

    private CanvasGroup InventarioE;
    private CanvasGroup InventarioB;
    private CanvasGroup InventarioA;
    private CanvasGroup InventarioX;

    void Start () {
        themeFromBD = 0;
        StartCoroutine(ActualizarTarjeta());
        StartCoroutine(CheckThemeOnBD());
        StartCoroutine(DataHasChanged());
    }
	
    public void BackToBoardView()
    {
        SceneManager.LoadScene("GridBoardView");
    }

    private IEnumerator DataHasChanged()
    {
        while (true)
        {
            if (CheckThemeHasChanged())
            {
                print("THEME CAMBIO");
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator ActualizarTarjeta()
    {
        while (true)
        {
            WWW itemsTarjetaFullScreen = new WWW("https://commendable-rigs.000webhostapp.com/revisarPantallaActiva.php");
            //WWW itemsTarjetaFullScreen = new WWW("http://localhost/indicadores/revisarPantallaActiva.php");
            yield return itemsTarjetaFullScreen;

            if (itemsTarjetaFullScreen.error == null)
                itemsTarjetaFullScreenString = itemsTarjetaFullScreen.text;

            int itemsTarjetaFullScreenInt = int.Parse(itemsTarjetaFullScreenString);

            if (itemsTarjetaFullScreenString == "-1")
            {
                SceneManager.LoadScene("GridBoardView");
            }
            else
            {
                this.transform.Find("Titulo").GetComponent<Text>().text = "Tarjeta " + itemsTarjetaFullScreenInt;
                //Revisamos en la Base de Datos los valores de la tarjeta específica
                WWWForm form = new WWWForm();
                form.AddField("tarjetaid", itemsTarjetaFullScreenInt);

                WWW itemsData = new WWW("https://commendable-rigs.000webhostapp.com/tarjetaconespecifica.php", form);
                //WWW itemsData = new WWW("http://localhost/indicadores/tarjetaconespecifica.php");
                yield return itemsData;

                if (itemsData.error == null)
                {
                    string itemsDataString = itemsData.text;

                    items = itemsDataString.Split(';');

                    ConfortBD =     AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Confort:");
                    DensidadBD =    AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Densidad:");
                    TemperaturaBD = AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Temperatura:");
                    MortalidadBD =  AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Mortalidad:");
                    ConsumoBD =     AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Consumo:");
                    EficienciaBD =  AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Eficiencia:");
                    InventarioBD =  AuxFunctions.GetDataValue(items[itemsTarjetaFullScreenInt], "Inventario:");

                    ConfortA = transform.Find("ConfortA").GetComponent<CanvasGroup>();
                    ConfortL = transform.Find("ConfortL").GetComponent<CanvasGroup>();
                    ConfortP = transform.Find("ConfortP").GetComponent<CanvasGroup>();
                    ConfortE = transform.Find("ConfortE").GetComponent<CanvasGroup>();

                    DensidadB = transform.Find("DensidadB").GetComponent<CanvasGroup>();
                    DensidadM = transform.Find("DensidadM").GetComponent<CanvasGroup>();
                    DensidadA = transform.Find("DensidadA").GetComponent<CanvasGroup>();

                    TemperaturaEF = transform.Find("TemperaturaEF").GetComponent<CanvasGroup>();
                    TemperaturaF = transform.Find("TemperaturaF").GetComponent<CanvasGroup>();
                    TemperaturaT = transform.Find("TemperaturaT").GetComponent<CanvasGroup>();
                    TemperaturaC = transform.Find("TemperaturaC").GetComponent<CanvasGroup>();
                    TemperaturaEC = transform.Find("TemperaturaEC").GetComponent<CanvasGroup>();

                    MortalidadA = transform.Find("MortalidadA").GetComponent<CanvasGroup>();
                    MortalidadN = transform.Find("MortalidadN").GetComponent<CanvasGroup>();

                    ConsumoB = transform.Find("ConsumoB").GetComponent<CanvasGroup>();
                    ConsumoN = transform.Find("ConsumoN").GetComponent<CanvasGroup>();
                    ConsumoA = transform.Find("ConsumoA").GetComponent<CanvasGroup>();
                    ConsumoI = transform.Find("ConsumoI").GetComponent<CanvasGroup>();

                    Eficiencia1 = transform.Find("Eficiencia1").GetComponent<CanvasGroup>();
                    Eficiencia2 = transform.Find("Eficiencia2").GetComponent<CanvasGroup>();
                    Eficiencia3 = transform.Find("Eficiencia3").GetComponent<CanvasGroup>();
                    Eficiencia4 = transform.Find("Eficiencia4").GetComponent<CanvasGroup>();
                    Eficiencia5 = transform.Find("Eficiencia5").GetComponent<CanvasGroup>();

                    InventarioE = transform.Find("InventarioE").GetComponent<CanvasGroup>();
                    InventarioB = transform.Find("InventarioB").GetComponent<CanvasGroup>();
                    InventarioA = transform.Find("InventarioA").GetComponent<CanvasGroup>();
                    InventarioX = transform.Find("InventarioX").GetComponent<CanvasGroup>();

                    ConfortA.alpha = 0f;
                    ConfortL.alpha = 0f;
                    ConfortP.alpha = 0f;
                    ConfortE.alpha = 0f;

                    DensidadB.alpha = 0f;
                    DensidadM.alpha = 0f;
                    DensidadA.alpha = 0f;

                    TemperaturaEF.alpha = 0f;
                    TemperaturaF.alpha = 0f;
                    TemperaturaT.alpha = 0f;
                    TemperaturaC.alpha = 0f;
                    TemperaturaEC.alpha = 0f;

                    MortalidadA.alpha = 0f;
                    MortalidadN.alpha = 0f;

                    ConsumoB.alpha = 0f;
                    ConsumoN.alpha = 0f;
                    ConsumoA.alpha = 0f;
                    ConsumoI.alpha = 0f;

                    Eficiencia1.alpha = 0f;
                    Eficiencia2.alpha = 0f;
                    Eficiencia3.alpha = 0f;
                    Eficiencia4.alpha = 0f;
                    Eficiencia5.alpha = 0f;

                    InventarioE.alpha = 0f;
                    InventarioB.alpha = 0f;
                    InventarioA.alpha = 0f;
                    InventarioX.alpha = 0f;


                    if (ConfortBD == "A")
                        ConfortA.alpha = 1f;
                    if (ConfortBD == "L")
                        ConfortL.alpha = 1f;
                    if (ConfortBD == "P")
                        ConfortP.alpha = 1f;
                    if (ConfortBD == "E")
                        ConfortE.alpha = 1f;

                    if (DensidadBD == "B")
                        DensidadB.alpha = 1f;
                    if (DensidadBD == "M")
                        DensidadM.alpha = 1f;
                    if (DensidadBD == "A")
                        DensidadA.alpha = 1f;

                    if (TemperaturaBD == "EF")
                        TemperaturaEF.alpha = 1f;
                    if (TemperaturaBD == "F")
                        TemperaturaF.alpha = 1f;
                    if (TemperaturaBD == "T")
                        TemperaturaT.alpha = 1f;
                    if (TemperaturaBD == "C")
                        TemperaturaC.alpha = 1f;
                    if (TemperaturaBD == "EC")
                        TemperaturaEC.alpha = 1f;

                    if (MortalidadBD == "A")
                        MortalidadA.alpha = 1f;
                    if (MortalidadBD == "N")
                        MortalidadN.alpha = 1f;

                    if (ConsumoBD == "B")
                        ConsumoB.alpha = 1f;
                    if (ConsumoBD == "N")
                        ConsumoN.alpha = 1f;
                    if (ConsumoBD == "A")
                        ConsumoA.alpha = 1f;
                    if (ConsumoBD == "I")
                        ConsumoI.alpha = 1f;

                    if (EficienciaBD == "1")
                        Eficiencia1.alpha = 1f;
                    if (EficienciaBD == "2")
                        Eficiencia2.alpha = 1f;
                    if (EficienciaBD == "3")
                        Eficiencia3.alpha = 1f;
                    if (EficienciaBD == "4")
                        Eficiencia4.alpha = 1f;
                    if (EficienciaBD == "5")
                        Eficiencia5.alpha = 1f;

                    if (InventarioBD == "E")
                        InventarioE.alpha = 1f;
                    if (InventarioBD == "B")
                        InventarioB.alpha = 1f;
                    if (InventarioBD == "A")
                        InventarioA.alpha = 1f;
                    if (InventarioBD == "X")
                        InventarioX.alpha = 1f;
                }
            }
        }
    }

    public void ChangeTheme(int themeID)
    {
        GameObject.Find("SingleTarjeta").transform.Find("Fondo").GetComponent<Image>().sprite = Resources.Load<Sprite>("Themes/bg" + themeID);
        storedThemeFromBD = themeFromBD = themeID;
    }

    private bool ThemeHasChanged(ref int newvalor)
    {
        if (storedThemeFromBD != newvalor)
        {
            ChangeTheme(themeFromBD);
            storedThemeFromBD = newvalor;
            return true;
        }
        storedThemeFromBD = newvalor;
        return false;
    }
    private bool CheckThemeHasChanged() // return if zoom value changes
    {
        return ThemeHasChanged(ref themeFromBD);
    }

    private IEnumerator CheckThemeOnBD() //Constantly check for BD for theme updates
    {
        while (true)
        {
            WWW themeQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarTema.php");
            //WWW themeQuery = new WWW("http://localhost/indicadores/revisarTema.php");
            yield return themeQuery;

            if (themeQuery.error == null)
                themeFromBD = int.Parse(themeQuery.text);
            else
                print("Error: " + themeQuery.error);

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
