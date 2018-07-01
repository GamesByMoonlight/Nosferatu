using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InvertYHelperScript : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Y))
        {
            //var controllers = FindObjectsOfType<NetworkPlayerConnection>();
            NetworkManager.singleton.client.connection.playerControllers[0].gameObject.GetComponentInChildren<FPSMouseLookController>().mouseLook.YSensitivity *= -1f;
        }
	}
}
