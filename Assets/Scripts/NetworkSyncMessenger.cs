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
    Vector3 realPosition;
    Quaternion realRotation;
    Vector3 lastRealPosition;
    Quaternion lastRealRotation;
    
    float timeStartedLerping;
    float timeToLerp;

    void Start()
    {
        name = gameObject.name + netId.ToString();
        Manager.Instance.AddObjectToConnectedObjects(netId, gameObject);

        if(isLocalPlayer)
        {
            Manager.Instance.SetLocalPlayerID(netId);
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

        if (_msg.forObjectID != netId)
        {
            Manager.Instance.ConnectedObjects[_msg.forObjectID].GetComponent<NetworkSyncMessenger>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
        }
    }

    public void ReceiveMovementMessage(Vector3 position, Quaternion rotation, float lerpTime)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = position;
        realRotation = rotation;
        timeToLerp = lerpTime;

        isLerpingPosition = realPosition != transform.position;
        isLerpingRotation = realRotation.eulerAngles != transform.rotation.eulerAngles;

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
        SendMovementMessage(netId, SyncTransform.position, SyncTransform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(NetworkInstanceId _playerID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        SyncMovementMessage _msg = new SyncMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            forObjectID = _playerID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void OnDestroy()
    {
        Manager.Instance.RemoveObjectsFromConnectedObjects(netId);
    }
}
