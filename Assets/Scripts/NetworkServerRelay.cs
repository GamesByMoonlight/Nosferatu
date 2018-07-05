using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkServerRelay : NetworkMessageHandler
{
    

    private void Start()
    {
        if(isServer)
        {
            RegisterNetworkMessages();
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler(movement_msg, OnReceivePlayerMovementMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        SyncTransformMessage _msg = _message.ReadMessage<SyncTransformMessage>();
        NetworkServer.SendToAll(movement_msg, _msg);
    }
}
