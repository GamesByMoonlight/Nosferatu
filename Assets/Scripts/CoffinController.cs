using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoffinController : NetworkBehaviour {

    public AttributesObject CoffinAttributes;
    private Attributes attributes = new Attributes();

    private void Start()
    {
        CoffinAttributes.Initialize(attributes);
        GetComponent<NetworkHealthController>().ForGameObject = attributes;
    }

}

