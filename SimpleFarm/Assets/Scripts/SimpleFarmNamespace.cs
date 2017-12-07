using ZXing;
using ZXing.QrCode;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SimpleFarmNamespace{

    public class Macros
    {
        static public string SitePath;
        static public int ElemsShow;
        static public string ChannelsContainer;
        static public string ChannelsGuide;
        static public string Controller;
        static public string SingleView;
        static public string EvSystem;
        
        static Macros()
        {
            //SitePath = "https://bhutan-slave.000webhostapp.com";
            SitePath = "https://localhost/SF_Services";
            //SitePath = "http://www.smesolutionslab.com/unity/simplefarm/";
            ElemsShow = 8;
            ChannelsContainer = "ChannelsContainerGrid";
            ChannelsGuide = "ChannelsGuide";
            Controller = "Controller";
            SingleView = "SingleView";
            EvSystem = "EventSystem";
        } 
    }

    public interface IBasicInterface{

        void Initialize();

        void InitializeScripts();

        void InitializeValues();

        void Play();

        void Stop();
    }

    public struct Canal
    {
        public int id;
        public string nombre;
        public int tipo;

        public Canal(int i, string n, int t)
        {
            id = i;
            nombre = n;
            tipo = t;
        }
    }

    public struct Programa
    {
        public int id;
        public string nombre;
        public string info;
        public int duracion;
        public int posicion;

        public Programa(int i, string n, string inf, int du, int po)
        {
            id = i;
            nombre = n;
            info = inf;
            duracion = du;
            posicion = po;
        }
    }

    public class AuxFunctions
    {

        //Has Changed Function
        public static bool ValueChanged<T>(string type, T storedValue, T newValue) where T : System.IComparable<T>
        {
            if (storedValue.CompareTo(newValue) != 0)
            {
                //Debug.Log(type + " Cambio");
                return true;
            }
            return false;
        }

        //RGBA with especific values
        public static Color ConvertColorRGBA(float r, float g, float b, float a)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a);
        }

        //Changes color of image in an especific time interval. MUST HAVE COMPONENT IMAGE.
        public static void ChangeColor(string path, Color endColor, float totalTime)
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                GameObject.Find(path).GetComponent<Image>().color = Color.Lerp(GameObject.Find(path).GetComponent<Image>().color, endColor, (elapsedTime / totalTime));
            }
        }

        // Lerp int values in time interval.
        public static IEnumerator LerpInt(int start, int end, float sec)
        {
            for (int i = start; i <= end; i++)
            {
                //Value
                yield return new WaitForSeconds(0.05f);
            }
        }

        // Lerp float values in time interval.
        public static IEnumerator LerpFloat(string elem, float start, float end, float atime)
        {
            for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / atime)
            {
                //GameObject.Find(elem).GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(start, end, t), 0.0f);
                yield return null;
            }
        }

        // Moves UI on x axis with property value
        public static IEnumerator LerpUI(string elemName, float start, float end, string axis, float speed) // Moves scrollbar value to previous or next page
        {
            switch (axis)
            {
                case "x":
                    for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / speed)
                    {
                        GameObject.Find(elemName).GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(start, end, t), 0.0f);
                        yield return null;
                    }
                    break;

                case "y":
                    for (float t = 0.0f; t <= 1.0; t += Time.deltaTime / speed)
                    {
                        GameObject.Find(elemName).GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, Mathf.Lerp(start, end, t));
                        yield return null;
                    }
                    break;
            }
        }

        // Encodes QR code to img
        private static Color32[] Encode(string textForEncoding, int width, int height) //Encode QR code.
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

        // Generate QR code with a text given
        public static Texture2D generateQR(string text) //generates QR code and returns as Texture2D
        {
            var encoded = new Texture2D(256, 256);
            var color32 = Encode(text, encoded.width, encoded.height);
            encoded.SetPixels32(color32);
            encoded.Apply();
            return encoded;
        }

        //Splits string due to index value
        public static string GetDataValue(string data, string index)
        {
            string value = data.Substring(data.IndexOf(index) + index.Length);
            if (value.Contains("|")) value = value.Remove(value.IndexOf("|"));
            return value;
        }

        //Authenticate for SAP Hana Cloud
        public static string Authenticate(string username, string password)
        {
            string auth = username + ":" + password;
            auth = Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
            auth = "Basic " + auth;
            return auth;
        }

        public static string GetGameObjectPath(ref GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
    }
}