using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerScore : NetworkBehaviour {
    
    public static int p1 = 0;    
    public static int p2 = 0;
    public static int p3 = 0;
    public static int p4 = 0;

    public static int p1skin = 0;
    public static int p2skin = 0;
    public static int p3skin = 0;
    public static int p4skin = 0;

    public static string p1name = "";
    public static string p2name = "";
    public static string p3name = "";
    public static string p4name = "";

    public static int winner;
    public static int winnerSkin;
    public static string winnerName;

    private static ServerScore me;

    public static void updateScore(PlayerDamage scoringPlayer)
    {
        foreach (NetworkConnection client in NetworkServer.connections)
        {
            foreach(PlayerController player in client.playerControllers)
            {
                GameObject obj = player.gameObject;
                if (obj.GetComponent<PlayerDamage>() == scoringPlayer)
                {
                    switch (NetworkServer.connections.IndexOf(client))
                    {
                        case 0:
                            p1++;
                            break;
                        case 1:
                            p2++;
                            break;
                        case 2:
                            p3++;
                            break;
                        case 3:
                            p4++;
                            break;
                    }
                    /*print("p1: " + p1);
                    print("p2: " + p2);
                    print("p3: " + p3);
                    print("p4: " + p4);*/
                }
            }
        }
    }
    public static void refresh()
    {
        p1 = 0;
        p2 = 0;
        p3 = 0;
        p4 = 0;
    }
    public static bool checkForWinner(int winThreshold)
    {
        if (p1 >= winThreshold)
        {
            winner = 1;
            winnerName = p1name;
            winnerSkin = p1skin;
            return true;
        }
        if (p2 >= winThreshold)
        {
            winner = 2;
            winnerName = p2name;
            winnerSkin = p2skin;
            return true;
        }
        if (p3 >= winThreshold)
        {
            winner = 3;
            winnerName = p3name;
            winnerSkin = p3skin;
            return true;
        }
        if (p4 >= winThreshold)
        {
            winner = 4;
            winnerName = p4name;
            winnerSkin = p4skin;
            return true;
        }
        return false;
    }
    public static void setSkin(int skin, string name, PlayerMovement playerMove)
    {

        foreach (NetworkConnection client in NetworkServer.connections)
        {
            foreach (PlayerController player in client.playerControllers)
            {
                GameObject obj = player.gameObject;
                if (obj.GetComponent<PlayerMovement>() == playerMove)
                {
                    switch (NetworkServer.connections.IndexOf(client))
                    {
                        case 0:
                            ServerScore.p1name = name;
                            ServerScore.p1skin = skin;
                            break;
                        case 1:
                            ServerScore.p2name = name;
                            ServerScore.p2skin = skin;
                            break;
                        case 2:
                            ServerScore.p3name = name;
                            ServerScore.p3skin = skin;
                            break;
                        case 3:
                            ServerScore.p4name = name;
                            ServerScore.p4skin = skin;
                            break;
                    }
                    /*print("p1: " + p1);
                    print("p2: " + p2);
                    print("p3: " + p3);
                    print("p4: " + p4);*/
                }
            }
        }
        print(ServerScore.p1skin + " " + ServerScore.p2skin + " " + ServerScore.p3skin + " " + ServerScore.p4skin);
    }
}
