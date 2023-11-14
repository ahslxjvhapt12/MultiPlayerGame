using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private NetworkObject _playerPrefab;
    [SerializeField] private ServerSingleton _serverPrefab;
    [SerializeField] private ClientSingleton _clientPrefab;

    [SerializeField] private string _ipAddress;
    [SerializeField] private ushort _port;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LaunchByMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private void LaunchByMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            //서버 만들어주고.
            ServerSingleton server = Instantiate(_serverPrefab, transform);
            server.StartServer(_playerPrefab, _ipAddress, _port);
        }
        else
        {
            ClientSingleton client = Instantiate(_clientPrefab, transform);
            client.CreateClient(_ipAddress, _port);

            SceneManager.LoadScene(SceneList.Menu);
        }
    }
}
