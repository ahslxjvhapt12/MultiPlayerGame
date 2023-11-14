using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class UserData
{
    public string username;
}

public class NetworkServer : IDisposable
{
    public delegate void UserChanged(ulong clientID, UserData userData);

    private NetworkObject _playerPrefab;
    private NetworkManager _networkManager;

    private Dictionary<ulong, UserData> _clientIdToUserDataDictionary = new Dictionary<ulong, UserData>();
    //여기에 아이디로 플레이어를 찾을 수 있는 것도 만들어야 한다.

    public NetworkServer(NetworkObject playerPrefab)
    {
        _playerPrefab = playerPrefab;
        _networkManager = NetworkManager.Singleton;
        _networkManager.ConnectionApprovalCallback += HandleConnectionApproval;
        _networkManager.OnServerStarted += HandleServerStarted;
    }

    private void HandleConnectionApproval(NetworkManager.ConnectionApprovalRequest req, 
                            NetworkManager.ConnectionApprovalResponse res)
    {
        string json = Encoding.UTF8.GetString(req.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(json);

        _clientIdToUserDataDictionary[req.ClientNetworkId] = userData;

        res.Approved = true;
        res.CreatePlayerObject = false;

        Debug.Log($"{userData.username} [ {req.ClientNetworkId} ] is logined!");
    }

    private void HandleServerStarted()
    {
        _networkManager.OnClientConnectedCallback += HandleClientConnect;
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientConnect(ulong clientID)
    {
        
    }

    private void HandleClientDisconnect(ulong clientID)
    {
        
    }

    public bool OpenConnection(string ipAddress, ushort port)
    {
        UnityTransport transport = _networkManager.GetComponent<UnityTransport>();
        transport.SetConnectionData(ipAddress, port);
        return _networkManager.StartServer();
    }

    public void Dispose()
    {
        if (_networkManager == null) return;
        _networkManager.ConnectionApprovalCallback  -= HandleConnectionApproval;
        _networkManager.OnServerStarted             -= HandleServerStarted;
        _networkManager.OnClientConnectedCallback   -= HandleClientConnect;
        _networkManager.OnClientDisconnectCallback  -= HandleClientDisconnect;

        if(_networkManager.IsListening)  //서버가 리스닝
        {
            _networkManager.Shutdown();
        }

    }
}
