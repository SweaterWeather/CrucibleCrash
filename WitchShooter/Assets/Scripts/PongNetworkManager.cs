using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PongNetworkManager : NetworkManager {

    //public GameObject ballPrefab;

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        if(this.numPlayers == 2)
        {
            //GameObject obj = Instantiate(ballPrefab);

            //NetworkServer.Spawn(obj);
        }
    }
}
