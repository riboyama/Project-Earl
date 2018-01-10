using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using EarlWatson;
using GoogleARCore;
using IBM.Watson.DeveloperCloud.Services.VisualRecognition.v3;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Connection;

public class WatsonController : MonoBehaviour {
    private RenderTexture overviewTexture;
    public Camera fcamera;
    public ARCoreBackgroundRenderer BackgroundRenderer;
    private Credentials credentials;
    private VisualRecognition _visualRecognition;
    private string _visualRecognitionVersionDate = "2016-05-20";
    private string _classifierID = "";

    public int resWidth = 1080;
    public int resHeight = 1920;

    // Use this for initialization
    void Start () {
        credentials = new Credentials("fa0d719942dea6bca4fe74a70b13ef9eff1d84c5", "https://watson-api-explorer.mybluemix.net/visual-recognition/api/v3/classify?" );
        _visualRecognition = new VisualRecognition(credentials);
        _visualRecognition.VersionDate = _visualRecognitionVersionDate;
        StartCoroutine(getClassification());
	}

    /// <summary>
    /// Earl Debug message method, remove in production
    /// </summary>
    public void earlDebug(string message)
    {
        Debug.Log("EarlDebug Watson: " + message);
    }

    private void OnClassify(ClassifyTopLevelMultiple classify, Dictionary<string, object> customData)
    {
        Log.Debug("ExampleVisualRecognition.OnClassify()", "Classify result: {0}", customData["json"].ToString());  
        earlDebug("WATSON SUCCES! : " + customData["json"].ToString());
    }
    private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
    {
        Log.Error("ExampleVisualRecognition.OnFail()", "Error received: {0}", error.ToString());
        earlDebug("WATSON FAIL! : " + error.ToString());
    }

    IEnumerator getClassification()
    {
        bool running = true;
        string path = Application.persistentDataPath + "temp.png";
        string[] owners = { "IBM", "me" };
        string[] classifierIDs = { "default", _classifierID };
        while (running)
        {
            byte[] bytes = null;
            try
            {
                RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
                fcamera.targetTexture = rt;
                Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
                fcamera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
                fcamera.targetTexture = null;
                RenderTexture.active = null; // JC: added to avoid errors
                Destroy(rt);
                bytes = screenShot.EncodeToPNG();   
            }
            catch (System.Exception e)
            {
                earlDebug("DIKKE ERROR: " + e);
            }
            if (bytes != null)
            {
                earlDebug("Bytes: " + bytes.Length);
                //  Classify using image path
                if (!_visualRecognition.Classify(OnClassify, OnFail, bytes, owners, classifierIDs, 0.5f)) { 
                    Log.Debug("ExampleVisualRecognition.Classify()", "Classify image failed!");
                    earlDebug("WATSON FAIL! NO CLASSIFICAITON");
                    //earlDebug("JSON: " + downloadedJson);  
                }

            } else
            {
                earlDebug("No bytedata");
            }
            
            yield return new WaitForSeconds(5);
        }
    }

    public string generateResponse(WjsonToClass[] data)
    {
        //var item = data[0].Scores;


        Dictionary<string, double> boekje = new Dictionary<string, double>();
        //boekje.Add("Angry", data[0].Images[0].Classifiers[0].Classes[0].Score);
        //TODO iterate over items in Classes...

        
        //var max = boekje.FirstOrDefault(x => x.Value == boekje.Values.Max()).Key;
        return "";
    }

    // Update is called once per frame
    void Update () {
		
	}
}
