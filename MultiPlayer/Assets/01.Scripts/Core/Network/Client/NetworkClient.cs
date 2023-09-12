using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;
    private const string MenuSceneName = "Menu";

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientId != 0 && clientId != _networkManager.LocalClientId) return;

        Disconnect();
    }

    public void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        if (_networkManager.IsConnectedClient) // 아직 서버에 연결수립이 되어 있는 상태라면
        {
            _networkManager.Shutdown(); // 강제 연결종료
        }
    }

    public void Dispose()
    {
        if (_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
}
