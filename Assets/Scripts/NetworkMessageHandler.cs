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
        public NetworkInstanceId forObjectID;
        public float time;

        // Transform
        public Vector3 objectPosition;
        public Quaternion objectRotation;
    }

    /// <summary>
    /// Gives 2 decimals of precision 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected float IntToFloat(int i)
    {
        return i / 100f;
    }

    /// <summary>
    /// Gives 2 decimals of precision 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected int FloatToInt(float i)
    {
        //Mathf.flo
        //return (int)(System.Math.Round(i * 100, 2));
        return Mathf.RoundToInt(i * 100);
    }

    //protected ushort
}
