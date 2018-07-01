using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class GameEventSystem : NetworkBehaviour {
    public static GameEventSystem Instance;

    private void Awake()
    {
        if(Instance != null && Instance.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [System.Serializable]
    public class GameEvent : UnityEvent { }                     // An Event that does not take an arguments
    [System.Serializable]
    public class GameObjectEvent : UnityEvent<GameObject> { }   // Takes GameObject argument

    public GameEvent EnemyDied;
}
