using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour {

    Light myLight;
    Camera myCamera;

	// Use this for initialization
	void Start () {
        myLight = GetComponentInChildren<Light>();
        if (myLight == null)
        {
            Debug.LogError("Light component not found on " + this);
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        myCamera = GetComponentInChildren<Camera>();
        if (myCamera == null)
        {
            Debug.LogError("Camera not found on " + this);
            return;
        }
           
        if (myLight)
            myLight.transform.rotation = myCamera.transform.rotation;
	}
}
