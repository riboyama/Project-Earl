using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Earl;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

public class EmotionController : MonoBehaviour {

    public Text responseText;

    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    /// <summary>
    /// A gameobject parenting UI for displaying the "emotion" snackbar.
    /// </summary>
    public GameObject emoteSnackbar;

    /// <summary>
    /// Compatibility boolean, use true for duo-camera phones, false for old phones
    /// </summary>
    private bool comp = true;

    // Use this for initialization
    void Start () {
        emoteSnackbar.SetActive(false);
        defaultBackground = background.texture;
        if (comp==true)
        {
            startFrontCam();
        }
    }

    public void startFrontCam()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, 1080, 1920);
            }
        }
        if (backCam == null)
        {
            _ShowAndroidToastMessage("Unable to initialize frontcamera");
            Invoke("DoQuit", 0.5f);
            return;

        }
        background.texture = backCam;
        backCam.Play();
        camAvailable = true;
        emoteSnackbar.SetActive(true);
        StartCoroutine(emotionCall());
       
    }

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    IEnumerator emotionCall()
    {
        bool running = true;
        while (running)
        {
            yield return new WaitForSeconds(2);
            if (!backCam.isPlaying)
            {
                running = false;
                _ShowAndroidToastMessage("Earl encountered a problem connecting to the front camera. Please restart the app.");
                Invoke("DoQuit", 1f);
            }

            byte[] bytes = generateBytes();
            UnityWebRequest www = generateRequest(bytes);
            using (www)
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log("A network error occured");
                }

                string downloadedJson = www.downloadHandler.text;
                
                if (verifyJSON(downloadedJson))
                {
                    
                    responseText.text = ("You look " + generateResponse(EmotionData.FromJson(downloadedJson)));
                }
                  
            }
            yield return new WaitForSeconds(5);
        }


    }

    public bool verifyJSON(string json) { 
   
        if (json.Contains("Unauthorized"))
        {
            responseText.text = "API Key Expired";
            return false;
        }
        if (json.Contains("faceRectangle"))
        {
            Debug.Log("EarlDebug: Correct JSON!");
            return true;
        }
        return false;
    }

    public UnityWebRequest generateRequest(byte[] bytes)
    {
        UnityWebRequest www = new UnityWebRequest("https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?", UnityWebRequest.kHttpVerbPOST);
        // create upload and download handler for the given byte array and set proper content type
        www.uploadHandler = new UploadHandlerRaw(bytes);
        www.chunkedTransfer = false;
        www.uploadHandler.contentType = "application/octet-stream";
        www.downloadHandler = new DownloadHandlerBuffer();

        // set API key and upload and download handler to the request
        www.SetRequestHeader("Ocp-Apim-Subscription-Key", "84746e612e58443d9f6fbe6ff380f73f");

        return www;
    }

    public byte[] generateBytes()
    {
        Texture2D photo = new Texture2D(backCam.width, backCam.height, TextureFormat.RGB24, false);
        photo.SetPixels(backCam.GetPixels());
        photo = rotateTexture(photo, false);
        return photo.EncodeToPNG();
    }

    public string generateResponse(EmotionData[] data)
    {
        var item = data[0].Scores;


        Dictionary<string, double> boekje = new Dictionary<string, double>();
        boekje.Add("Angry", item.Anger);
        boekje.Add("Contemptiouos", item.Contempt);
        boekje.Add("Disgusted", item.Disgust);
        boekje.Add("Scared", item.Fear);
        boekje.Add("Happy", item.Happiness);
        boekje.Add("Neutrally", item.Neutral);
        boekje.Add("Sad", item.Sadness);
        boekje.Add("Surprised", item.Surprise);
        var max = boekje.FirstOrDefault(x => x.Value == boekje.Values.Max()).Key;
        return max;
    } 

    // Update is called once per frame
    void Update () {
        if (camAvailable && comp)
        {
            float ratio = (float)backCam.width / (float)backCam.height;
            fit.aspectRatio = ratio;

            float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -backCam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
