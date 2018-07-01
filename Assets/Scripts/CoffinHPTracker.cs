using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoffinHPTracker : NetworkBehaviour {

    [SerializeField]
    private AttributesObject CoffinAttributes;
    private Attributes attributes = new Attributes();

    public NetworkHealthController networkHealthController;

    private void Awake()
    {
        CoffinAttributes.Initialize(attributes);
        GetComponent<NetworkHealthController>().ForGameObject = attributes;
    }

}

