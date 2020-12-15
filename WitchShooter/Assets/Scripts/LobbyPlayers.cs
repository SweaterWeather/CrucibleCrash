using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayers : NetworkLobbyPlayer {

	void Start () {
        MenuController.lobbyPlayers.Add(this);
    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }


}
