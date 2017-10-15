using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

//This script controls de camera device.
public class CameraDeviceScript : MonoBehaviour {

    private WebCamTexture webcamTexture;
    private RawImage raw;

    private static Color32[] Encode(string textForEncoding, int width, int height)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }

    public Texture2D generateQR(string text)
    {
        var encoded = new Texture2D(256, 256);
        var color32 = Encode(text, encoded.width, encoded.height);
        encoded.SetPixels32(color32);
        encoded.Apply();
        return encoded;
    }

    // Use this for initialization
    Texture2D myQR, qr;

    private void OnGUI()
    {
        myQR = generateQR("1");
        if (GUI.Button(new Rect(300, 300, 256, 256), myQR, GUIStyle.none)) { }
    }

    void Start()
    {
        //Starts Camera
        
        webcamTexture = new WebCamTexture();
        raw = this.GetComponent<RawImage>();
        raw.texture = webcamTexture;
        raw.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        
        qr = generateQR("3");
        GameObject.Find("QR").GetComponent<RawImage>().texture = qr;

    }

    public void CameraControl()
    {
        Texture2D aux;
        webcamTexture.Pause();
        aux = new Texture2D(640, 380);
        aux.SetPixels(webcamTexture.GetPixels(0, 0, 640, 380));
        aux.Apply();
        GameObject.Find("Shot").GetComponent<RawImage>().texture = aux;
        string _SavePath = "C:/Users/PEDRO SANCHEZ/Desktop/";
        int _CaptureCounter = 0;
        File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", aux.EncodeToPNG());
        webcamTexture.Play();
    }
}