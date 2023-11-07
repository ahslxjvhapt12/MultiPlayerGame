using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class UserListBehaviour : NetworkBehaviour
{
    [SerializeField] private ReadyUI _readyUI;
    [SerializeField] private List<TankDataSO> _tankDatas;

    public static UserListBehaviour Instance;
    public NetworkList<UserListEntityState> _userList = new NetworkList<UserListEntityState>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        _readyUI.SetTankTemplate(_tankDatas);
        _readyUI.TankSelect += HandleTankSelected;
        _readyUI.ReadyChanged += HandleReadyChanged;

        if (IsClient)
        {
            _userList.OnListChanged += HandleUserListChanged;
            foreach (var user in _userList)
            {
                HandleUserListChanged(new NetworkListEvent<UserListEntityState>
                {
                    Type = NetworkListEvent<UserListEntityState>.EventType.Add,
                    Value = user
                });
            }

            // 여기에 ReadyUI에서 호스트인지 일반 유저인지 보내주는 코드가 필요하다
            // 호스트는 스타트 버튼을 활성화 하고 일반 유저는 스타트 버튼을 비활설화 한다
        }

        if (IsServer)
        {
            UserData userData = HostSingletone.Instance.GameManager.NetworkServer.GetUserDataByClientID(NetworkManager.Singleton.LocalClientId);
            _readyUI.GameStarted += HandleGameStarted;

            HostSingletone.Instance.GameManager.NetworkServer.OnClientJoin += HandleUserJoin;
            HostSingletone.Instance.GameManager.NetworkServer.OnClientLeft += HandleUserLeft;


        }
    }

    public override void OnNetworkDespawn()
    {
        _readyUI.TankSelect -= HandleTankSelected;
        _readyUI.ReadyChanged -= HandleReadyChanged;
        if (IsClient)
        {
            _userList.OnListChanged -= HandleUserListChanged;
        }
        if (IsServer)
        {
            _readyUI.GameStarted -= HandleGameStarted;

            HostSingletone.Instance.GameManager.NetworkServer.OnClientJoin -= HandleUserJoin;
            HostSingletone.Instance.GameManager.NetworkServer.OnClientLeft -= HandleUserLeft;
        }

        _userList?.Dispose();
    }

    private void HandleUserJoin(UserData userData, ulong clientID)
    {
        int idx = FindIndex(clientID);
        if (idx >= 0) return;
        UserListEntityState newUser = new UserListEntityState
        {
            clientID = clientID,
            playerName = userData.username,
            ready = false,
            tankID = 0
        };
        _userList.Add(newUser);
    }

    private int FindIndex(ulong clientID)
    {
        for (int i = 0; i < _userList.Count; ++i)
        {
            if (_userList[i].clientID != clientID) continue;
            return i;
        }
        return -1;
    }

    private void HandleUserLeft(string authID)
    {

    }

    private void HandleGameStarted()
    {

    }

    private void HandleTankSelected(int obj)
    {

    }

    private void HandleReadyChanged(bool obj)
    {

    }

    private void HandleUserListChanged(NetworkListEvent<UserListEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<UserListEntityState>.EventType.Add:
                _readyUI.AddUserData(evt.Value);
                break;
            case NetworkListEvent<UserListEntityState>.EventType.Remove:
                break;
            case NetworkListEvent<UserListEntityState>.EventType.Value:
                break;
        }
    }
}
