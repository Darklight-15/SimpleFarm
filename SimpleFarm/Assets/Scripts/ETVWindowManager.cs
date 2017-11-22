using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleFarmNamespace;

public class ETVWindowManager : MonoBehaviour {

    //Changes component text of a gameobject given
    public void ChangeText(string elemPath, string content) 
    {
        try
        {
            GameObject.Find(elemPath).GetComponent<Text>().text = content;
        }
        catch (Exception e)
        {
            Debug.Log("ChangeText Error!: " + e);
        }
    }

    //Instance a window window with a type: temporal - static
    public void InstanceAndShowWindow(string type, string prefabName, string parentName, string content, float duration, float fadeTime)
    {

        GameObject prefab = (GameObject)Resources.Load("Prefabs/PopupPrefab/" + prefabName, typeof(GameObject));
        GameObject prefabClone = Instantiate(prefab, GameObject.Find(parentName).transform);
        string path = AuxFunctions.GetGameObjectPath(ref prefabClone);

        if (content != "")
            ChangeText(path, content);

        switch (type)
        {
            case "temporal":
                StartCoroutine(ShowInDuration(prefabClone.name, duration, fadeTime));
                break;

            case "static":
                StartCoroutine(FadeIn(prefabClone.name, 0.5f));
                GameObject.Find("close-btn").GetComponent<Button>().onClick.AddListener(() => Destroy(GameObject.Find(GameObject.Find("close-btn").transform.parent.name)));
                break;
        }

    }

    //Shows window alerady instanced in an especific time interval
    public IEnumerator ShowInDuration(string elemPath, float duration, float fadeTime)
    {
        float t;
        GameObject obj = GameObject.Find(elemPath);
        CanvasGroup a = obj.GetComponent<CanvasGroup>();

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

        Destroy(obj);

        yield break;
    }

    //Fade in a existing window alpha in a time interval
    IEnumerator FadeIn(string elemPath, float aTime)
    {
        CanvasGroup a = GameObject.Find(elemPath).GetComponent<CanvasGroup>();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            a.alpha = Mathf.Lerp(0.0f, 1.0f, t);
            yield return null;
        }

        yield break;
    }

    //Fade out and destroy a existing window in a interval of time
    IEnumerator FadeOut(string elemPath, float aTime)
    {
        GameObject obj = GameObject.Find(elemPath);
        CanvasGroup a = GetComponent<CanvasGroup>();

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            a.alpha = Mathf.Lerp(1.0f, 0.0f, t);
            yield return null;
        }

        Destroy(obj);

        yield break;
    }
}
