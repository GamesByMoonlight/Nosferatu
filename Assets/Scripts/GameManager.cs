using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public float MatchTime = 10f * 60f; // 10min
    public float CurrentTime;
    public float SyncRate = .2f;    // Synce every (1 / SyncRate) seconds
        
    float TimeLastSyncSent;
    bool running = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        CurrentTime = MatchTime;
        if(isServer)
        {
            SynchronizeTime();
        }
    }

    public void StartMatch()
    {
        running = true;
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

    private void MatchOver()
    {
        running = false;
        Debug.Log("Match Over [Do Something]");
    }
    
    [ClientRpc]
    private void RpcSyncTime(float updatedTime)
    {
        CurrentTime = updatedTime;
    }
}
