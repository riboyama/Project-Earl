using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    public Camera FirstPersonCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        gameObject.transform.LookAt(FirstPersonCamera.transform);
        //transform.position+Camera.main.transform.forward
    }
}
