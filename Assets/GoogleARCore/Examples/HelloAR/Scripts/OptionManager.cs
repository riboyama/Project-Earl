using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour {

    public GameObject panel;
    private bool isEnabled = false;


	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenOptionMenu()
    {
        if (!isEnabled) {            
            isEnabled = true;
            panel.SetActive(true);

        }
        else if (isEnabled) {
            isEnabled = false;
            panel.SetActive(false);
        }
      
    }
}
