using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    public GameObject AvatarPrefab;


	// Use this for initialization
	void Start () {
        InitAvatar();
	}

    void InitAvatar()
    {
        var avatar = Instantiate(AvatarPrefab, transform.position, transform.rotation, transform);
        var syncMessenger = GetComponent<NetworkSyncMessenger>();
        syncMessenger.SyncTransform = avatar.transform;
        syncMessenger.SyncRigidbody = avatar.GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        }
        else
        {
            avatar.GetComponent<FPSMouseLookController>().enabled = false;
            avatar.GetComponent<StepSimulator>().enabled = false;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
