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
        public ushort time;

        // Transform
        //public Vector3 objectPosition;
        //public Quaternion objectRotation;
        public int positionX;
        public int positionY;
        public int positionZ;
        public int eulerY;


    }

    /// <summary>
    /// Gives 2 decimals of precision for floats up to magnitude of +/- ~2.1M.  3rd decimal rounded accurately.
    /// Useful for sending position floats across network.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected float IntToFloat(int i)
    {
        return i / 1000f;
    }

    /// <summary>
    /// Gives 2 decimals of precision for floats up to magnitude of +/- ~2.1M.  3rd decimal rounded accurately.
    /// Useful for sending position floats across network.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected int FloatToInt(float i)
    {
        //Mathf.flo
        //return (int)(System.Math.Round(i * 100, 2));
        return Mathf.RoundToInt(i * 1000);
    }

    /// <summary>
    /// Gives 4 decimals of precision for numbers less than 0.  5th decimal rounded accurately.
    /// Useful for sending deltaTime across network.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected float HalfToFloat(ushort i)
    {
        return Mathf.HalfToFloat(i);
    }

    /// <summary>
    /// Gives 4 decimals of precision for numbers less than 0.  5th decimal rounded accurately.
    /// Useful for sending deltaTime across network.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    protected ushort FloatToHalf(float i)
    {
        return Mathf.FloatToHalf(i);
    }
}
