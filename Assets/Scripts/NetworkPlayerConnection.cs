using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    public GameObject PlayerAvatar;

    [SerializeField]
    private AttributesObject PlayerAttributes;
    private Attributes attributes = new Attributes();

    private void Awake()
    {
        PlayerAttributes.Initialize(attributes);
    }

    // Use this for initialization
    void Start () {
        InitAvatar();
	}

    void InitAvatar()
    {
        var avatar = PlayerAvatar; //Instantiate(AvatarPrefab, transform.position, transform.rotation, transform);
        GetComponent<NetworkHealthController>().ForGameObject = attributes;
        GetComponent<NetworkFireController>().WeaponAttributes = attributes;

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
	
}
