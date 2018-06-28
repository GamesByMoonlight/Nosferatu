using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkMessageHandler : NetworkBehaviour
{
    public const short movement_msg = 3778;

    public class SyncMovementMessage : MessageBase
    {
        public string forObjectID;
        public float time;

        // Transform
        public Vector3 objectPosition;
        public Quaternion objectRotation;

        // Rigidbody
        public Vector3 objectVelocity;
        public float objectDrag;
    }

}
