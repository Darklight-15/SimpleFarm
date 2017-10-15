using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewsArrayAnim : MonoBehaviour {

    public int numNews;
    public float velocity;
    float arraySize, screenSize;
    
    private void Start()
    {
        //Initialization
        numNews = 0;
        velocity = 20.0f;

        if (GameObject.Find("Controller"))
        {   
            StartCoroutine(checkArraySize());
        }
    }

    void countNews()
    {
        int acts = 0;
        foreach (Transform child in GameObject.Find("NewsArray").transform)
        {
            if (child.GetComponent<Text>().text != "")
            {
                acts++;
            }
        }
        numNews = acts;
    }

    IEnumerator checkArraySize()
    {
        yield return new WaitForSeconds(4.0f); // Wait for Initialization 
        arraySize = GetComponent<RectTransform>().sizeDelta.x; //Size of the news string
        screenSize = GameObject.Find("NewsBG").GetComponent<RectTransform>().sizeDelta.x; //Size of the screen
        yield return new WaitForSeconds(2.0f); // Wait for Initialization 
        countNews();
        StartCoroutine( LerpElement( "NewsArray", screenSize, (arraySize * (-1)), velocity) );
    }

    public IEnumerator LerpElement(string elemName, float start, float end, float atime) // Moves scrollbar value to previous or next page
    {
        while (true) {

            for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / velocity)
            {
                GameObject.Find(elemName).GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(start, end, t), 0.0f);
                yield return null;
            }
            countNews();
        }
    }

}

