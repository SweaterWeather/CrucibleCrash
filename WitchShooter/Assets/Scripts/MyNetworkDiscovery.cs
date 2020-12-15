using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkDiscovery : NetworkDiscovery
{

    public List<string> addresses;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        NetworkManager.singleton.networkAddress = fromAddress;
        //NetworkManager.singleton.StartClient();
        foreach(string addr in addresses)
        {
            if (addr == fromAddress) return;
        }
        addresses.Add(fromAddress);
    }
}
