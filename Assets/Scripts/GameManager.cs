using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class GameManager : CustomMessagingEventSystem {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public float MatchTime = 10f * 60f; // 10min
    public float CurrentTime;
    public float SyncRate = .2f;    // Synce every (1 / SyncRate) seconds
        
    private float TimeLastSyncSent;
    private bool running = false;
    private List<GameObject> ConnectedPlayers;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        CurrentTime = MatchTime;
        StartMatch();

        if(isServer)
        {
            SynchronizeTime();
            EntityDiedEvent.AddListener(OnEntityDeath);
        }
    }

    private void Update()
    {
        if(running)
            CurrentTime -= Time.deltaTime;
        
        if(isServer && Time.time - TimeLastSyncSent > (1 / SyncRate))
        {
            SynchronizeTime();
        }

        if(CurrentTime <= 0f)
        {
            MatchOver();
        }
    }

    private void SynchronizeTime()
    {
        RpcSyncTime(CurrentTime);
        TimeLastSyncSent = Time.time;
    }

    [ClientRpc]
    private void RpcSyncTime(float updatedTime)
    {
        CurrentTime = updatedTime;
    }

    private void MatchOver()
    {
        running = false;
        Debug.Log("Match Over [Do Something]");
    }
    
    private void OnEntityDeath(GameObject entity)
    {
        bool somePlayersAlive = false;
        Attributes attr;

        foreach (GameObject player in ConnectedPlayers)
        {
            attr = player.GetComponent<NetworkPlayerConnection>().PlayerAttributes;
            if (attr.Team == Teams.good && attr.CurrentHealth > 0f)
            {
                somePlayersAlive = true;
            }
        }

        if(!somePlayersAlive)
        {
            Debug.Log("Game Over.  All good players dead");
        }
    }

    public void StartMatch()
    {
        running = true;
    }

    public void RegisterPlayer(GameObject player)
    {
        if (ConnectedPlayers == null)
            ConnectedPlayers = new List<GameObject>();
        ConnectedPlayers.Add(player);
    }

    public void UnregisterPlayer(GameObject player)
    {
        ConnectedPlayers.Remove(player);
    }
}
