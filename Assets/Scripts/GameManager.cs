using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public enum GameState { Stopped, Running, VampireWinsByTime, VampireWinsByElimination, AdventurersWin }

public class GameManager : CustomMessagingEventSystem {
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameState CurrentState = GameState.Stopped;
    public float MatchTime = 10f * 60f; // 10min
    public float CurrentTime;
    public float SyncRate = .2f;    // Synce every (1 / SyncRate) seconds
    public GameObject[] ConnectedPlayers { get { return connectedPlayers.ToArray(); } }
    public GameObject LocalPlayer { get { return localPlayer; } }

    private float TimeLastSyncSent;
    private bool running = false;
    private List<GameObject> connectedPlayers;
    private GameObject localPlayer;

    private void Awake()
    {
        _instance = this;
        connectedPlayers = new List<GameObject>();
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
        if (running)
            CurrentTime -= Time.deltaTime;

        if (!isServer)
            return;
        
        if(Time.time - TimeLastSyncSent > (1 / SyncRate))
        {
            SynchronizeTime();
        }

        if(running && CurrentTime <= 0f)
        {
            SynchronizeTime();
            RpcMatchOver(GameState.VampireWinsByTime);
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
    
    private void OnEntityDeath(GameObject entity)
    {
        if (!running)
            return;

        // Check coffin death
        if(entity.GetComponent<CoffinController>() != null)
        {
            RpcMatchOver(GameState.AdventurersWin);
            return;
        }

        // Check if all good players dead
        bool somePlayersAlive = false;
        Attributes attr;
        foreach (GameObject player in connectedPlayers)
        {
            attr = player.GetComponent<NetworkPlayerConnection>().PlayerAttributes;
            if (attr.Team == Teams.good && attr.CurrentHealth > 0f)
            {
                somePlayersAlive = true;
            }
        }

        if(!somePlayersAlive)
        {
            RpcMatchOver(GameState.VampireWinsByElimination);
        }
    }

    [ClientRpc]
    private void RpcMatchOver(GameState state)
    {
        running = false;
        CurrentState = state;

        switch (state)
        {
            case GameState.AdventurersWin:
                Debug.Log("Adventurers win!");
                break;
            case GameState.VampireWinsByElimination:
                Debug.Log("Vampire wins.  All adventurers dead or converted.");
                break;
            case GameState.VampireWinsByTime:
                Debug.Log("Vampire wins.  Time ran out and adventurers lost forever.");
                break;
        }

        GameStateChanged.Invoke();
    }

    public void StartMatch()
    {
        running = true;
        CurrentState = GameState.Running;
        GameStateChanged.Invoke();
    }

    public void RegisterPlayer(GameObject player, bool theLocalPlayer)
    {
        if(isServer)
        {
            Debug.Log("How many on server?");
        }

        connectedPlayers.Add(player);
        if(theLocalPlayer)
        {
            localPlayer = player;
        }
    }

    public void UnregisterPlayer(GameObject player)
    {
        connectedPlayers.Remove(player);
    }
}
