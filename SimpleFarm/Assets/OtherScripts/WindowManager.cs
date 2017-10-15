using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class WindowManager : MonoBehaviour
{
    //Changes component text of a gameobject given
    public void ChangeText(string elemName, string content)
    {
        try
        {
            GameObject.Find(elemName).transform.Find("Text").GetComponent<Text>().text = content;
        }
        catch (Exception e)
        {
            Debug.Log("ChangeText Error!: " + e);
        }

    }

    //instance a popup window with a type: temporal - static
    public void InstanceAndShowPopUp(string type, string prefabName, string parentName, string content, float duration, float fadeTime)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/PopupPrefab/" + prefabName, typeof(GameObject));
        GameObject prefabClone = Instantiate(prefab, GameObject.Find(parentName).transform);

        if (content != "")
            ChangeText(prefabClone.name, content);

        switch (type)
        {
            case "temporal":
                StartCoroutine(ShowPopUp(prefabClone.name, duration, fadeTime));
                StopCoroutine("ShowPopUp");
                break;

            case "static":
                StartCoroutine(FadeInPopUp(prefabClone.name, 0.5f));
                StopCoroutine("FadeInPopUp");
                GameObject.Find("close-btn").GetComponent<Button>().onClick.AddListener(() => Destroy(GameObject.Find(GameObject.Find("close-btn").transform.parent.name)));
                break;
        }

    }

    //Shows popup alerady instanced in an especific time interval
    public IEnumerator ShowPopUp(string elemName, float duration, float fadeTime)
    {
        float t;
        CanvasGroup a = GameObject.Find(elemName).GetComponent<CanvasGroup>();

        for (t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            a.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }

        yield return new WaitForSeconds(duration); //Cambiar

        for (t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            a.alpha = Mathf.Lerp(1.0f, 0.0f, t);
            yield return null;
        }

        Destroy(GameObject.Find(elemName));
    }

    //Fade in a existing popup alpha in a time interval
    IEnumerator FadeInPopUp(string elemName, float aTime)
    {
        CanvasGroup a = GameObject.Find(elemName).GetComponent<CanvasGroup>();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            a.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }
    }

    //Fade out and destroy a existing popup in a interval of time
    IEnumerator FadeOutPopUp(string elemName, float aTime)
    {
        CanvasGroup a = GameObject.Find(elemName).GetComponent<CanvasGroup>();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            a.alpha = Mathf.Lerp(1.0f, 0.0f, t);
            yield return null;
        }
        Destroy(GameObject.Find(elemName));
    }
}
