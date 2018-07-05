using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase : MonoBehaviour
{
    public AttributesObject Attributes;
    private Attributes attr = new Attributes();

    protected virtual void Awake()
    {
        Attributes.Initialize(attr);
    }
}