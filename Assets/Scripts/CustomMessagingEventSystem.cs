using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class CustomMessagingEventSystem : NetworkBehaviour {
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }   // Takes GameObject argument

    public GameObjectEvent EntityDiedEvent;
    public UnityEvent GameStateChanged;
    public GameObjectEvent AttributesUpdatedFor;
}
