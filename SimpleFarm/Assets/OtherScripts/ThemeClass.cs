using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThemeClass : MonoBehaviour
{
    //Properties
    public string gridName;
    public string themeLocation;
    public int themeFromBD; //Theme from database
    private int storedThemeFromBD; //Value stored from onchange

    //Initializers

    public void Initialize()
    {
        InitializeValues();
        ActivateCheckDataOnBD();
        ActivateDataHasChanged();
    }

    void InitializeValues()
    {
        themeFromBD = 0;
        storedThemeFromBD = 0;
    }

    void ActivateCheckDataOnBD()
    {
        StartCoroutine(CR_CheckThemeOnBD());
    }

    void ActivateDataHasChanged()
    {
        StartCoroutine(CR_ThemeHasChanged());
    }

    // Local Events

    private bool ThemeHasChanged(ref int newvalor)
    {
        if (storedThemeFromBD != newvalor)
        {
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

    private IEnumerator CR_ThemeHasChanged()
    {
        while (true)
        {
            if (CheckThemeHasChanged())
                ChangeTheme(themeFromBD);
            yield return new WaitForSeconds(1.0f);
        }
    }

    //Main Methods

    public void ChangeTheme(int themeID)
    {
        foreach (Transform child in GameObject.Find(gridName).transform)
        {
            if (!child.name.Contains("EmptySpace"))//if have any children named Titulo
            {
                child.transform.Find(themeLocation).GetComponent<Image>().sprite = Resources.Load<Sprite>("Themes/bg" + themeID);
            }
        }
        storedThemeFromBD = themeFromBD = themeID;
    }

    // Server Talkers

    private IEnumerator CR_CheckThemeOnBD() //Constantly check for BD for theme updates
    {
        WWW themeQuery;
        while (true)
        {
            themeQuery = new WWW("https://commendable-rigs.000webhostapp.com/revisarTema.php");
            //WWW themeQuery = new WWW("http://localhost/indicadores/revisarTema.php");

            yield return themeQuery;

            if (themeQuery.error == null)
            {
                themeFromBD = int.Parse(themeQuery.text);
            }
            else
            {
                print("Error: " + themeQuery.error);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}
