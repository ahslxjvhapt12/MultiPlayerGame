using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    public Action<UserData, ulong> OnClientLeft;
    public Action<UserData, ulong> OnClientJoin;

    private Dictionary<ulong, string> _clientToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authToUserDataDictionary = new Dictionary<string, UserData>();

    private NetworkObject _playerPrefabs;

    public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefabs)
    {
        _networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
        _playerPrefabs = playerPrefabs;
    }

    // 클라이언트들이 서버에 접속할 때 실행을 시켜줘서 요청에 따라 승인응답할 수도있고 안할수도
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();
        data.Deserialize(req.Payload);

        //Debug.Log(data.username);
        _clientToAuthDictionary[req.ClientNetworkId] = data.userAuthId;
        _authToUserDataDictionary[data.userAuthId] = data;

        OnClientJoin?.Invoke(data, req.ClientNetworkId);

        res.Approved = true;
    }

    public void SpawnPlayer(ulong clientID, UserListEntityState userState)
    {
        NetworkObject player = GameObject.Instantiate(_playerPrefabs, TankSpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        player.SpawnAsPlayerObject(clientID);

        TankPlayer tankComponent = player.GetComponent<TankPlayer>();
        tankComponent.SetTankNetworkVariable(userState);
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (_clientToAuthDictionary.TryGetValue(clientId, out string authID))
        {
            _clientToAuthDictionary.Remove(clientId);
            UserData userData = _authToUserDataDictionary[authID];
            _authToUserDataDictionary.Remove(authID);

            OnClientLeft?.Invoke(userData, clientId); // 클라이언트 접속 종료시에 알려준다.
        }
    }

    public void Dispose()
    {
        if (_networkManager != null) return;

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnNetworkReady;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }

    public UserData GetUserDataByClientID(ulong clientId)
    {
        if (_clientToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            if (_authToUserDataDictionary.TryGetValue(authId, out UserData data))
            {
                return data;
            }
        }
        return null;
    }
}
