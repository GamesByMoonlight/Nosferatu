using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoffinController : NetworkBehaviour {

    public AttributesObject CoffinAttributes;

    private void Start()
    {
        GetComponent<NetworkHealthController>().ForGameObject = CoffinAttributes.attributes;
    }

}

