using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientGameManager
{
    private NetworkManager _networkManager;
    private JoinAllocation _allocation;
    private bool _isLobbyRefresh = false; // 나중에 씀

    public ClientGameManager(NetworkManager networkManager)
    {
        _networkManager = networkManager;
    }

    public void Disconnect()
    {
        // 메뉴씬으로 보내기
        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public async Task StartClientAsync(string joinCode, UserData userData)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        // 트랜스포트를 받아서
        // 릴레이서버 데이터를 만들어서 설정해주고
        // user데이터를 json 으로 만들어서 connectionData에 넣은 후에
        // NetworkManager에 StartClient 를 해주면 된다.
    }
}
