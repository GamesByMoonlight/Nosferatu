using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class BuffBase : NetworkBehaviour
{
    public AttributesObject Attributes;
    private Attributes attr = new Attributes();

    protected virtual void Awake()
    {
        Attributes.Initialize(attr);
    }
}