using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private const string GameSceneName = "Game";
    private const int _maxConnections = 20;
    private string _joinCode;
    private string _lobbyId;
    private Allocation _allocation;

    public NetworkServer NetworkServer { get; private set; }

    private NetworkObject _playerPrefab;
    public HostGameManager(NetworkObject playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public async void ShutdownAsync()
    {
        HostSingletone.Instance.StopCoroutine(nameof(HeartBeatLobby));
        if (!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        NetworkServer.OnClientLeft -= HandleClientLeft;
        _lobbyId = string.Empty;
        NetworkServer?.Dispose();
    }

    public void Dispose()
    {
        ShutdownAsync();
    }

    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return;
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        var relayServerData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        string playerName = PlayerPrefs.GetString(BootstrapScreen.PlayerNameKey, "Unknown");

        try
        {
            CreateLobbyOptions option = new CreateLobbyOptions();
            option.IsPrivate = false;
            option.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode" , new DataObject(visibility:DataObject.VisibilityOptions.Member, value : _joinCode)
                }
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s lobby", _maxConnections, option);

            _lobbyId = lobby.Id;

            HostSingletone.Instance.StartCoroutine(HeartBeatLobby(15));
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogError(ex); // UI로 알아서 하셈
            return;
        }

        NetworkServer = new NetworkServer(NetworkManager.Singleton, _playerPrefab);

        UserData userData = new UserData()
        {
            username = playerName,
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        NetworkManager.Singleton.NetworkConfig.ConnectionData = userData.Serialize().ToArray();

        NetworkManager.Singleton.StartHost();
        NetworkServer.OnClientLeft += HandleClientLeft;
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    private async void HandleClientLeft(UserData userData, ulong authID)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobbyId, userData.userAuthId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private IEnumerator HeartBeatLobby(int sec)
    {
        var timer = new WaitForSecondsRealtime(sec);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return timer;
        }
    }
}