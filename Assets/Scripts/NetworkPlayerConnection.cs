using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerConnection : NetworkBehaviour {
    [SerializeField] private AttributesObject[] PossibleClassTypes;
    [SyncVar] public string playerName;
    [SyncVar] public Color playerColor;
    [SyncVar] public PlayerClass playerType;
    public GameObject PlayerAvatar;

    
    private AttributesObject PlayerAttributesScriptableObject;
    public Attributes PlayerAttributes { get; private set; }

    

    // Use this for initialization
    void Start () {
        PlayerAttributesScriptableObject = PossibleClassTypes[(int)playerType];
        PlayerAttributes = new Attributes();
        PlayerAttributesScriptableObject.Initialize(PlayerAttributes);
        GameManager.Instance.RegisterPlayer(gameObject, isLocalPlayer);
        InitAvatar();
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

        //Debug.Log("playerType in NetworkPlayerConnection.cs is " + playerType);
        avatar.GetComponentInChildren<ModelSelector>().ChooseModel(playerType); 

        if(isServer)
        {
            GetComponent<NetworkFireController>().SpawnBulletPool();
        }

        if (isLocalPlayer)
        {
            //avatar.GetComponentInChildren<MeshRenderer>().material.color = playerColor;
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
        CmdModify(modification.attributes);
    }

    [Command]
    public void CmdModify(Attributes attr)
    {
        RpcModify(attr);
    }

    [ClientRpc]
    public void RpcModify(Attributes attr)
    {
        PlayerAttributes.Modify(attr);
    }


    public void UnModifyAttributes(AttributesObject modification)
    {
        modification.UnModify(PlayerAttributes);
    }

    
}
