using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SyncObjectManager;

public class NetworkSyncMessenger : NetworkMessageHandler {
    public Transform SyncTransform;

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
            Manager.Instance.ConnectedObjects[_msg.forObjectID].GetComponent<NetworkSyncMessenger>().ReceiveMovementMessage(_msg.positionX, _msg.positionY, _msg.positionZ, _msg.eulerY, _msg.time);
        }
    }

    public void ReceiveMovementMessage(int posX, int posY, int posZ, int rotation, ushort lerpTime)
    {
        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = new Vector3(IntToFloat(posX), IntToFloat(posY), IntToFloat(posZ));
        realRotation = Quaternion.Euler(0f, IntToFloat(rotation), 0f);
        timeToLerp = HalfToFloat(lerpTime);

        isLerpingPosition = realPosition != transform.position;
        isLerpingRotation = realRotation.eulerAngles != transform.rotation.eulerAngles;

        timeStartedLerping = Time.time;
    }

    // Update is called once per frame
    void Update () {

        if (!hasAuthority)
            return;
        
        UpdatePlayerMovement();
	}

    void FixedUpdate()
    {
        if (!hasAuthority)
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
            positionX = FloatToInt(_position.x),
            positionY = FloatToInt(_position.y),
            positionZ = FloatToInt(_position.z),
            eulerY = FloatToInt(_rotation.eulerAngles.y),
            forObjectID = _playerID,
            time = FloatToHalf(_timeTolerp)
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void OnDestroy()
    {
        Manager.Instance.RemoveObjectsFromConnectedObjects(netId);
    }
}
