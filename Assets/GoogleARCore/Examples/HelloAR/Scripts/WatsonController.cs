using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using EarlWatson;

public class WatsonController : MonoBehaviour {
    private RenderTexture overviewTexture;
    public Camera FrontCam;


    // Use this for initialization
    void Start () {
        StartCoroutine(getClassification());
	}
	
   
    IEnumerator getClassification()
    {
        bool running = true;
        while (running)
        {
            yield return new WaitForEndOfFrame();
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = FrontCam.targetTexture;
            FrontCam.Render();
            Texture2D photo = new Texture2D(FrontCam.targetTexture.width, FrontCam.targetTexture.height, TextureFormat.RGB24, false);
            photo.ReadPixels(new Rect(0, 0, FrontCam.targetTexture.width, FrontCam.targetTexture.height), 0, 0);
            photo.Apply();
            RenderTexture.active = currentRT;

            byte[] bytes = photo.EncodeToPNG();
            UnityWebRequest www = generateRequest(bytes);
            using (www)
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.Log("A network error occured");
                }

                string downloadedJson = www.downloadHandler.text;
                Debug.Log("EarlDebug: " + downloadedJson);

                yield return new WaitForSeconds(5);
            }
        }
    }

    public UnityWebRequest generateRequest(byte[] bytes)
    {
        UnityWebRequest www = new UnityWebRequest("https://watson-api-explorer.mybluemix.net/visual-recognition/api/v3/classify?api_key=fa0d719942dea6bca4fe74a70b13ef9eff1d84c5&version=2016-05-20", UnityWebRequest.kHttpVerbPOST);
        // create upload and download handler for the given byte array and set proper content type
        www.uploadHandler = new UploadHandlerRaw(bytes);
        www.chunkedTransfer = false;
        www.uploadHandler.contentType = "application/json";
        www.downloadHandler = new DownloadHandlerBuffer();

        return www;
    }

    public string generateResponse(WjsonToClass[] data)
    {
        //var item = data[0].Scores;


        Dictionary<string, double> boekje = new Dictionary<string, double>();
        //boekje.Add("Angry", data[0].Images[0].Classifiers[0].Classes[0].Score);
        //TODO iterate over items in Classes...

        var max = boekje.FirstOrDefault(x => x.Value == boekje.Values.Max()).Key;
        return max;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
