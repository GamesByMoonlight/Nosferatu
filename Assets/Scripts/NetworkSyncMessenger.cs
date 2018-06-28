using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SyncObjectManager;

public class NetworkSyncMessenger : NetworkMessageHandler {
    [Header("Leave null if not syncing")]
    public Transform SyncTransform;
    public Rigidbody SyncRigidbody;
    
    // Player properties
    string objectID;

    // Movement Properties
    [Header("Movement Properties")]
    [SerializeField]
    float networkSendRate = 5;
    bool canSendNetworkMovement;
    float timeBetweenMovementStart;
    float timeBetweenMovementEnd;

    // Lerping properties
    bool isLerpingPosition;
    bool isLerpingRotation;
    bool isLerpingVelocity;
    Vector3 realPosition;
    Vector3 realVelocity;
    Quaternion realRotation;
    Vector3 lastRealPosition;
    Vector3 lastRealVelocity;
    Quaternion lastRealRotation;
    
    float timeStartedLerping;
    float timeToLerp;

    void Start()
    {
        objectID = gameObject.name + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = objectID;
        Manager.Instance.AddObjectToConnectedObjects(objectID, gameObject);

        if(isLocalPlayer)
        {
            Manager.Instance.SetLocalPlayerID(objectID);
            canSendNetworkMovement = false;
            RegisterNetworkMessages();
        }
    }
    
    private void RegisterNetworkMessages()
    {
        NetworkManager.singleton.client.RegisterHandler(movement_msg, OnReceiveMovementMessage);
    }

    private void OnReceiveMovementMessage(NetworkMessage _message)
    {
        SyncMovementMessage _msg = _message.ReadMessage<SyncMovementMessage>();

        if (_msg.forObjectID != transform.name)
        {
            Manager.Instance.ConnectedObjects[_msg.forObjectID].GetComponent<NetworkSyncMessenger>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.objectVelocity, _msg.objectDrag, _msg.time);
        }
    }

    public void ReceiveMovementMessage(Vector3 position, Quaternion rotation, Vector3 velocity, float drag, float lerpTime)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        lastRealVelocity = realVelocity;
        realPosition = position;
        realRotation = rotation;
        realVelocity = velocity;
        timeToLerp = lerpTime;
        SyncRigidbody.drag = drag;  // Just set drag directly.  It's an instant change anyway.

        isLerpingPosition = realPosition != transform.position;
        isLerpingRotation = realRotation.eulerAngles != transform.rotation.eulerAngles;
        isLerpingVelocity = realVelocity != SyncRigidbody.velocity;

        timeStartedLerping = Time.time;
    }

    // Update is called once per frame
    void Update () {

        if (!isLocalPlayer)
            return;

        UpdatePlayerMovement();
	}

    void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            NetworkLerp();
        }
    }

    void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            SyncTransform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            SyncTransform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }

        if(isLerpingVelocity)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;
            SyncRigidbody.velocity = Vector3.Lerp(lastRealVelocity, realVelocity, lerpPercentage);
        }
    }

    void UpdatePlayerMovement()
    {
        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(objectID, SyncTransform.position, SyncTransform.rotation, SyncRigidbody.velocity, SyncRigidbody.drag, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _playerID, Vector3 _position, Quaternion _rotation, Vector3 _velocity, float _drag, float _timeTolerp)
    {
        SyncMovementMessage _msg = new SyncMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectVelocity = _velocity,
            objectDrag = _drag,
            forObjectID = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void OnDestroy()
    {
        Manager.Instance.RemoveObjectsFromConnectedObjects(objectID);
    }
}
