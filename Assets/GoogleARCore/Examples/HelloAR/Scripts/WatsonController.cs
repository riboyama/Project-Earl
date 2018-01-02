using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatsonController : MonoBehaviour {
    private RenderTexture overviewTexture;
    public Camera FrontCam;


    // Use this for initialization
    void Start () {
        
	}
	
    IEnumerable getClassification()
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


            // Encode texture into PNG
            byte[] bytes = photo.EncodeToPNG();

            //TODO: Generate formdata and Unity Webrequest
            yield return new WaitForSeconds(5);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
