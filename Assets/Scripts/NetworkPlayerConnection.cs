using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    public GameObject PlayerAvatar;



    [SerializeField]
    private AttributesObject PlayerAttributes;
    private Attributes attributes = new Attributes();

    [SyncVar (hook ="OnNameChanged")] public string playerName;
    [SyncVar (hook ="OnColorChanged") ] public Color playerColor;
    [SyncVar (hook ="OnPlayerTypeChanged")] public string playerType;



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
            //avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
            
            avatar.GetComponentInChildren<MeshRenderer>().material.color = playerColor;
            var inputController = avatar.GetComponent<FPSMouseLookController>();
            inputController.movementSettings.ForwardSpeed = attributes.ForwardSpeed;
            inputController.movementSettings.BackwardSpeed = attributes.BackwardSpeed;
            inputController.movementSettings.StrafeSpeed = attributes.StrafeSpeed;
        }
        else
        {
            avatar.GetComponent<FPSMouseLookController>().enabled = false;
            avatar.GetComponent<StepSimulator>().enabled = false;
            avatar.GetComponentInChildren<Camera>().enabled = false;
            avatar.GetComponentInChildren<AudioListener>().enabled = false;
        }
    }

    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        //set text
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        // demo had rendertoggler
        //GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);

    }

    void OnPlayerTypeChanged(string value)
    {
        playerType = value;
    }
	
}
