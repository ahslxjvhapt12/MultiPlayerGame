using System;
using System.Text;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ClientGameManager : IDisposable
{
    public void ConnectClient(UserData userData)
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(JsonUtility.ToJson(userData));
        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        // ÇÒ ÁþÀÌ ¾ø³×
        var networkManager = NetworkManager.Singleton;
        if (networkManager != null && networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }
}
