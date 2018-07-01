using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    public GameObject PlayerAvatar;

    [SerializeField]
    private AttributesObject PlayerAttributesScriptableObject;
    public Attributes PlayerAttributes { get; private set; }

    private void Awake()
    {
        PlayerAttributes = new Attributes();
        PlayerAttributesScriptableObject.Initialize(PlayerAttributes);
    }

    // Use this for initialization
    void Start () {
        InitAvatar();

        if(isServer)
        {
            GameManager.Instance.RegisterPlayer(gameObject);
        }
	}

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (isServer)
        {
            GameManager.Instance.UnregisterPlayer(gameObject);
            Debug.Log("Disconnected");
        }
    }

    void InitAvatar()
    {
        var avatar = PlayerAvatar; //Instantiate(AvatarPrefab, transform.position, transform.rotation, transform);
        GetComponent<NetworkHealthController>().ForGameObject = PlayerAttributes;
        GetComponent<NetworkFireController>().WeaponAttributes = PlayerAttributes;

        if (isLocalPlayer)
        {
            avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
            var inputController = avatar.GetComponent<FPSMouseLookController>();
            inputController.movementSettings.ForwardSpeed = PlayerAttributes.ForwardSpeed;
            inputController.movementSettings.BackwardSpeed = PlayerAttributes.BackwardSpeed;
            inputController.movementSettings.StrafeSpeed = PlayerAttributes.StrafeSpeed;
        }
        else
        {
            avatar.GetComponent<FPSMouseLookController>().enabled = false;
            avatar.GetComponent<StepSimulator>().enabled = false;
            avatar.GetComponentInChildren<Camera>().enabled = false;
            avatar.GetComponentInChildren<AudioListener>().enabled = false;
        }
    }
	

    public void ModifyAttributes(AttributesObject modification)
    {
        modification.Modify(PlayerAttributes);
    }

    public void UnModifyAttributes(AttributesObject modification)
    {
        modification.UnModify(PlayerAttributes);
    }

    
}
