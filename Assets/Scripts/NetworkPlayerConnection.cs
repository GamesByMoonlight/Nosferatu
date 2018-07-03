using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    public GameObject PlayerAvatar;



    [SerializeField]
    private AttributesObject PlayerAttributesScriptableObject;
    public Attributes PlayerAttributes { get; private set; }

    [SyncVar (hook ="OnNameChanged")] public string playerName;
    [SyncVar (hook ="OnColorChanged") ] public Color playerColor;
    [SyncVar (hook ="OnPlayerTypeChanged")] public string playerType;



    private void Awake()
    {
        PlayerAttributes = new Attributes();
        PlayerAttributesScriptableObject.Initialize(PlayerAttributes);
    }

    // Use this for initialization
    void Start () {
        InitAvatar();
        GameManager.Instance.RegisterPlayer(gameObject, isLocalPlayer);
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
            //avatar.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
            
            avatar.GetComponentInChildren<MeshRenderer>().material.color = playerColor;
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
	

    public void ModifyAttributes(AttributesObject modification)
    {
        modification.Modify(PlayerAttributes);
    }

    public void UnModifyAttributes(AttributesObject modification)
    {
        modification.UnModify(PlayerAttributes);
    }

    
}
