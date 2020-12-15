using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenuController : NetworkBehaviour {

    public Dropdown servers;
    public GameObject mainMenu;
    public GameObject joinMenu;
    public GameObject connectingView;
    public GameObject hostMenu;
    public GameObject lobby;
    public InputField ipField;
    public MyNetworkDiscovery netDiscover;
    //public NetworkLobbyManager netManager;

    public static List<LobbyPlayers> lobbyPlayers;

    private void Start()
    {
        lobbyPlayers = new List<LobbyPlayers>();
        netDiscover.Initialize();
        //servers.options = netDiscover.broadcastsReceived;
    }

    private void Update()
    {
        if (connectingView.gameObject.activeInHierarchy && NetworkLobbyManager.singleton.IsClientConnected())
        {
            ShowLobby();
        }
        else if (joinMenu.gameObject.activeInHierarchy)
        {
            servers.options = new List<Dropdown.OptionData>();
            //print(netDiscover.addresses);
            List<string> strings = new List<string>();
            strings.Add("");
            servers.AddOptions(strings);
            servers.AddOptions(netDiscover.addresses);
        }
    }
    public void SelectIP()
    {
        print("selected");
        ipField.text = servers.options[servers.value].text;
    }

    public void HostGame()
    {
        if (!NetworkLobbyManager.singleton.isNetworkActive)
        {
            NetworkLobbyManager.singleton.StartHost();
        }
    }
    public void BeginBroadcast()
    {
        if (netDiscover.running)
        {
            EndBroadcast();
        }
        netDiscover.StartAsServer();
        netDiscover.broadcastData = Network.player.ipAddress;
    }
    public void BeginListening()
    {
        if (netDiscover.running)
        {
            EndBroadcast();
        }
        netDiscover.StartAsClient();
    }
    public void EndBroadcast()
    {
        netDiscover.StopBroadcast();
        netDiscover.Initialize();
    }
    public void CancelHost()
    {
        if (NetworkLobbyManager.singleton.isNetworkActive)
        {
            NetworkLobbyManager.singleton.StopHost();
        }
    }
    public void JoinGame()
    {
        if (!NetworkLobbyManager.singleton.isNetworkActive)
        {
            NetworkLobbyManager.singleton.networkAddress = ipField.text;
            NetworkLobbyManager.singleton.StartClient();
            ShowConnecting();
        }
    }
    public void CancelJoin()
    {
        if (NetworkLobbyManager.singleton.isNetworkActive)
        {
            NetworkLobbyManager.singleton.StopClient();
            ShowJoinMenu();
        }
    }
    public void ReadyUp()
    {
        foreach (LobbyPlayers netPlayer in lobbyPlayers)
        {
            if (netPlayer.isLocalPlayer)
            {
                netPlayer.SendReadyToBeginMessage();
                netPlayer.readyToBegin = true;
            }
        }
    }
    public void ReadyOff()
    {
        foreach (LobbyPlayers netPlayer in lobbyPlayers)
        {
            if (netPlayer.isLocalPlayer)
            {
                netPlayer.SendNotReadyToBeginMessage();
                netPlayer.readyToBegin = false;
            }
        }
    }
    public void ShowJoinMenu()
    {
        mainMenu.SetActive(false);
        joinMenu.SetActive(true);
        connectingView.SetActive(false);
        hostMenu.SetActive(false);
        lobby.SetActive(false);
    }
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        joinMenu.SetActive(false);
        connectingView.SetActive(false);
        hostMenu.SetActive(false);
        lobby.SetActive(false);
    }
    public void ShowConnecting()
    {
        mainMenu.SetActive(false);
        joinMenu.SetActive(false);
        connectingView.SetActive(true);
        lobby.SetActive(false);
        hostMenu.SetActive(false);
    }
    public void ShowHost()
    {
        mainMenu.SetActive(false);
        joinMenu.SetActive(false);
        connectingView.SetActive(false);
        lobby.SetActive(false);
        hostMenu.SetActive(true);
    }
    public void ShowLobby()
    {
        SceneManager.LoadScene(1);
        mainMenu.SetActive(false);
        joinMenu.SetActive(false);
        connectingView.SetActive(false);
        lobby.SetActive(true);
        hostMenu.SetActive(false);
    }
    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0);
    }
}
