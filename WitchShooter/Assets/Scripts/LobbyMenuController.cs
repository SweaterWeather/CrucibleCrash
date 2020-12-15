using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyMenuController : NetworkBehaviour
{
    public Animator magenta;
    public Animator cyan;
    public Animator black;
    public Animator yellow;

    public Button readyBttn;
    public Button notreadyBttn;
    public InputField username;
    //public Dropdown character;
    private bool started = false;

    [ServerCallback]
    private void Start()
    {
        Object.FindObjectOfType<NetworkLobbyManager>().TryToAddPlayer();
        username.text = PlayerStartup.username;
    }
    private void Update()
    {

        setCharacter(PlayerStartup.choseCharacter);
    }

    public void ReadyUp()
    {
        foreach (LobbyPlayers netPlayer in MenuController.lobbyPlayers)
        {
            if (netPlayer.isLocalPlayer)
            {
                netPlayer.SendReadyToBeginMessage();
                netPlayer.readyToBegin = true;
                readyBttn.interactable = false;
                notreadyBttn.interactable = true;
            }
        }
    }
    public void ReadyOff()
    {
        foreach (LobbyPlayers netPlayer in MenuController.lobbyPlayers)
        {
            if (netPlayer.isLocalPlayer)
            {
                netPlayer.SendNotReadyToBeginMessage();
                netPlayer.readyToBegin = false;
                readyBttn.interactable = true;
                notreadyBttn.interactable = false;
            }
        }
    }
    public void ReturnToTitle()
    {
        if (NetworkLobbyManager.singleton.isNetworkActive)
        {
            NetworkLobbyManager.singleton.StopHost();
            NetworkLobbyManager.singleton.StopClient();
        }
        SceneManager.LoadScene(0);
    }
    public void setUserName()
    {
        PlayerStartup.username = username.text;
    }
    public void setCharacter(int chara)
    {
        PlayerStartup.choseCharacter = chara;
        switch (chara)
        {
            case 1:
                magenta.Play("Victory");
                cyan.Play("Chibi");
                black.Play("Chibi");
                yellow.Play("Chibi");
                break;
            case 2:
                magenta.Play("Chibi");
                cyan.Play("Chibi");
                black.Play("Victory");
                yellow.Play("Chibi");
                break;
            case 3:
                magenta.Play("Chibi");
                cyan.Play("Victory");
                black.Play("Chibi");
                yellow.Play("Chibi");
                break;
            case 4:
                magenta.Play("Chibi");
                cyan.Play("Chibi");
                black.Play("Chibi");
                yellow.Play("Victory");
                break;
        }
    }
}
