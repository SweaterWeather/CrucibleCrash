using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameOverController : NetworkBehaviour {

    public Text congrats;
    public float timeTillLobby = 15;
    public float timeTillAnnounce = .25f;
    public SelectAnimator winner;
    public SelectAnimator loser1;
    public SelectAnimator loser2;
    public SelectAnimator loser3;
    private bool announced = false;


    [ServerCallback]
    void Update () {
        timeTillLobby -= Time.deltaTime;
        if (timeTillLobby < 0) Object.FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();

        timeTillAnnounce -= Time.deltaTime;
        if (timeTillAnnounce < 0 && !announced)
        {
            print(ServerScore.p1skin + " " + ServerScore.p2skin + " " + ServerScore.p3skin + " " + ServerScore.p4skin);
            RpcLocalSetWinner(ServerScore.winnerName, ServerScore.winner - 1, ServerScore.p1skin, ServerScore.p2skin, ServerScore.p3skin, ServerScore.p4skin);
            announced = true;
            ServerScore.refresh();
        }
    }
    [ClientRpc]
    void RpcLocalSetWinner(string winnerName, int winnerIndex, int p1witch, int p2witch, int p3witch, int p4witch)
    {
        winner.gameObject.SetActive(true);
        loser1.gameObject.SetActive(true);
        loser2.gameObject.SetActive(true);
        loser3.gameObject.SetActive(true);

        congrats.text = "Congratulations " + winnerName + "!";
        List<int> list = new List<int>();
        list.Add(p1witch);
        list.Add(p2witch);
        list.Add(p3witch);
        list.Add(p4witch);

        winner.setAnim(list[winnerIndex], "Victory");
        list.RemoveAt(winnerIndex);

        loser1.setAnim(list[0], "Defeat");
        loser2.setAnim(list[1], "Defeat");
        loser3.setAnim(list[2], "Defeat");
    }
}
