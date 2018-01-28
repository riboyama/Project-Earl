using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    public Camera FirstPersonCamera;

    // Use this for initialization
    void Start()
    {
        try
        {
            FirstPersonCamera = GameObject.Find("First Person Camera").GetComponent<Camera>();
        } catch (System.Exception e)
        {
            Debug.Log("LookatDebug: ERROR   " + e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(transform.position + FirstPersonCamera.transform.forward);
        //transform.position+Camera.main.transform.forward
    }
}
