using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Earl;
using EarlWatson;
using System.Linq;
using dbman;
using nsTTS;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;
using System;

public class EmotionController : MonoBehaviour {

    public Text responseText;

    private bool camAvailable;
    private WebCamTexture backCam;
    //private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;

    //Watson stuff
    private string _visualRecognitionVersionDate = "2016-05-20";
    private string _classifierID = "";

    private int resWidth = 1080;
    private int resHeight = 1920;

    public Camera fcamera;
    private Credentials credentials;
    private VisualRecognition _visualRecognition;

    

    private string curEmote = "";

    private dbManager ds;

    private DateTime oldDT = DateTime.Now;
    
    /// <summary>
    /// A gameobject parenting UI for displaying the "emotion" snackbar.
    /// </summary>
    public GameObject emoteSnackbar;

    /// <summary>
    /// Compatibility boolean, use true for duo-camera phones, false for old phones
    /// </summary>
    private bool comp = true;

    /// <summary>
    /// Setting integers
    /// </summary>
    public int emotePlaySoundInt = 10;
    
    public int emoteLoopTime = 10;

    // Use this for initialization
    void Start () {
        ds = new dbManager("earldb.s3db");
        ds.createTable();
        emoteSnackbar.SetActive(false);
        //defaultBackground = background.texture;
        if(comp)
        {
            credentials = new Credentials("fa0d719942dea6bca4fe74a70b13ef9eff1d84c5", "https://watson-api-explorer.mybluemix.net/visual-recognition/api/v3/classify?");
            _visualRecognition = new VisualRecognition(credentials);
            _visualRecognition.VersionDate = _visualRecognitionVersionDate;
            startFrontCam();
        }
    }

    /// <summary>
    /// Starts up the frontcamera, contains confusing code
    /// </summary>
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

    /// <summary>
    /// Earl Debug message method, remove in production
    /// </summary>
    public void earlDebug(string message)
    {
        Debug.Log("EarlDebug: " + message);
    }

    /// <summary>
    /// Since the phone is in Portrait mode rotate the image.
    /// </summary>
    /// <param name="originalTexture"> Landscape image</param>
    /// <param name="clockwise">Rotate clockwise or not</param>
    /// <returns></returns>
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

    /// <summary>
    /// Starts capturing an image from the front camera, making webrequests
    /// </summary>
    /// <returns></returns>
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

                string downloadedJson = www.downloadHandler.text;

                if (www.isNetworkError) { Debug.Log("A network error occured"); }
                else if (verifyJSON(downloadedJson))
                {
                    string response = generateResponse(EmotionData.FromJson(downloadedJson));
                    curEmote = response;
                    responseText.text = ("You look " + response + " about ");
                    getClassification();
                    if(response == "sad")
                    {
                        var eAndy = GameObject.Find("andyObject");
                        if(eAndy != null){
                            Animator anim = eAndy.GetComponent<Animator>();
                            anim.SetBool("isSad", true);
                            anim.Play("Sad");
                            anim.SetBool("isSad", false);
                        }
                    }
                }
                  
            }
            var emotes = ds.getEmotions();
            ds.ePrintData(emotes);
            yield return new WaitForSeconds(emoteLoopTime);
        }
    }

    public void playEmoteSound(string emote)
    {
        DateTime nowDT = DateTime.Now;
        double diffInSeconds = (nowDT - oldDT).TotalSeconds;
        if (diffInSeconds > emotePlaySoundInt)
        {
            ttsController ttsCon = new ttsController();
            ttsCon.Synthesize("You look " + emote);
        }
    }

    /// <summary>
    /// Use the WatsonSDK to ask for a classification of the image captured from the backcamera
    /// </summary>
    /// <param name="response">The recieved emotion<s/param>
    public void getClassification()
    {
        string[] owners = { "IBM", "me" };
        string[] classifierIDs = { "default", _classifierID };
        byte[] bytes = getFCameraBytes();
        if (bytes != null)
        {
            earlDebug("Bytes: " + bytes.Length);
            //  Classify using image path
            if (!_visualRecognition.Classify(OnClassify, OnFail, bytes, owners, classifierIDs, 0.5f))
            {
                Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");
                earlDebug("Failed to get a Watson image classification");
            }
        }
        else
        {
            earlDebug("ARCore image has invalid bytedata");
        }
    }

    /// <summary>
    /// Get the byte[] from the backcamera
    /// </summary>
    /// <returns> byte array</returns>
    private byte[] getFCameraBytes()
    {
        try
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            fcamera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            fcamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            fcamera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            return screenShot.EncodeToPNG();
        } catch (System.Exception e)
        {
            earlDebug("Failed to generate ARCore image" + e);
        }

        return null;
    }

    /// <summary>
    /// Called on Succes, parses json.
    /// </summary>
    /// <param name="classify"></param>
    /// <param name="customData"></param>
    private void OnClassify(ClassifyTopLevelMultiple classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassify()", "Classify result: {0}", customData["json"].ToString());
        earlDebug("WATSON SUCCES! : " + customData["json"].ToString());

        WjsonToClass data = WjsonToClass.FromJson(customData["json"].ToString());
        responseText.text = data.Images[0].Classifiers[0].Classes[0].PurpleClass;
        try
        {
            ds.CreateEmotion(curEmote, System.Convert.ToSingle(data.Images[0].Classifiers[0].Classes[0].Score), System.DateTime.Now, data.Images[0].Classifiers[0].Classes[0].PurpleClass, data.Images[0].Classifiers[0].Classes[1].PurpleClass);

        } catch(System.Exception e)
        {
            _ShowAndroidToastMessage("Failed to Query Emotions to database: " + e);
            earlDebug("Failed to Query Emotions to database: " + e);
            
        }

        //Debug stuff
        for (int i = 0; i < 2; i++)
        {
            earlDebug("Classifier scores begin: ");
            earlDebug(data.Images[0].Classifiers[0].Classes[i].PurpleClass);
            earlDebug(data.Images[0].Classifiers[0].Classes[i].Score.ToString());
            earlDebug("Classifier scores end");
            
        }

    }

    /// <summary>
    /// Called on failure from Visual Recognition
    /// </summary>
    /// <param name="error"></param>
    /// <param name="customData"></param>
    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleVisualRecognition.OnFail()", "Error received: {0}", error.ToString());
        earlDebug("WATSON FAIL! : " + error.ToString());
    }

    /// <summary>
    /// Verify JSON for errors
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public bool verifyJSON(string json) { 
   
        if (json.Contains("Unauthorized"))
        {
            responseText.text = "API Key Expired";
            return false;
        }
        if (json.Contains("faceRectangle"))
        {    
            return true;
        }
        return false;
    }

    /// <summary>
    /// Create WebRequest to Emotion API
    /// </summary>
    /// <param name="bytes">byte data from frontcamera</param>
    /// <returns></returns>
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

    /// <summary>
    /// Generate bytes from device camera
    /// </summary>
    /// <returns>byte array</returns>
    public byte[] generateBytes()
    {
        Texture2D photo = new Texture2D(backCam.width, backCam.height, TextureFormat.RGB24, false);
        photo.SetPixels(backCam.GetPixels());
        photo = rotateTexture(photo, false);
        return photo.EncodeToPNG();
    }

    /// <summary>
    /// Change text in response snackbar
    /// </summary>
    /// <param name="data">JSON data class</param>
    /// <returns>The highest rated emotion</returns>
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
