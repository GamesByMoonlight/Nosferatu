using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class cmNetworkPlayerHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();

        NetworkPlayerConnection gPlayer = gamePlayer.GetComponent<NetworkPlayerConnection>();

        gPlayer.playerName = lPlayer.playerName;
        gPlayer.playerColor = lPlayer.playerColor;
       
        gPlayer.playerType = lPlayer.playerType;

    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
