using UnityEngine;
using UnityEngine.UI;

//This scripts controls debbuging buttons when zooms the screen

public class GridScript : MonoBehaviour {

	public void EnableZoomInButton()
    {
        GameObject.Find("zoomOut-btn").GetComponent<Button>().interactable = true;
    }

    public void EnableZoomOutButton()
    {
        GameObject.Find("zoomIn-btn").GetComponent<Button>().interactable = true;
    }

    public void DisableZoomInButton()
    {
        GameObject.Find("zoomOut-btn").GetComponent<Button>().interactable = false;  
    }

    public void DisableZoomOutButton()
    {
        GameObject.Find("zoomIn-btn").GetComponent<Button>().interactable = false;
    }
}
