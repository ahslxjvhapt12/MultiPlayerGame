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
    [SerializeField] private CoinSpawner _coinSpawner; 

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
            HandleUserJoin(userData, NetworkManager.Singleton.LocalClientId);
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

    private void HandleUserLeft(UserData userData, ulong clientID)
    {
        if (_userList == null) return;

        foreach (var item in _userList)
        {
            if (item.clientID != clientID) continue;

            try
            {
                _userList.Remove(item);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e} 삭제중 오류 발생");
            }
            break;
        }
    }

    private void HandleGameStarted()
    {
        for (int i = 0; i < _userList.Count; ++i)
        {
            var user = _userList[i];
            TankDataSO tankData = GetTankDataSO(user.tankID);

            user.combatData = new TankCombatData
            {
                moveSpeed = tankData.moveSpeed,
                damage = tankData.basicTurretSO.damage,
                rotateSpeed = tankData.rotateSpeed,
                maxHP = tankData.maxHP,
            };

            // 여기서 탱크를 스폰하도록 코드를 작성한다.
            HostSingletone.Instance.GameManager.NetworkServer.SpawnPlayer(user.clientID, user);
        }

        GameStartClientRpc();
        _coinSpawner.StartSpawn();
    }

    public TankDataSO GetTankDataSO(int tankID)
    {
        return _tankDatas.Find(x => x.tankID == tankID);
    }

    [ClientRpc]
    private void GameStartClientRpc()
    {
        Debug.Log("알삐씨콜ㄹ");
        _readyUI.HideFromScreen();
    }

    private void HandleTankSelected(int tankID)
    {
        SelectTankServerRpc(tankID, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectTankServerRpc(int tankID, ulong clientID)
    {
        // 적절하게 타겟을 찾아서
        // 해당 리스트의 타겟을 갱신
        // 리스트의 원소를 아예 새롭게갱신

        int idx = FindIndex(clientID);

        UserListEntityState oldUser = _userList[idx];
        _userList[idx] = new UserListEntityState
        {
            clientID = oldUser.clientID,
            ready = oldUser.ready,
            playerName = oldUser.playerName,
            tankID = tankID,
            combatData = oldUser.combatData,
        };

    }

    private void HandleReadyChanged(bool value)
    {
        SetReadyServerRpc(value, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(bool readyValue, ulong clientId)
    {
        int idx = FindIndex(clientId);

        var oldItem = _userList[idx];

        _userList[idx] = new UserListEntityState
        {
            clientID = oldItem.clientID,
            ready = readyValue,
            playerName = oldItem.playerName,
            tankID = oldItem.tankID,
            combatData = oldItem.combatData
        };


        _readyUI.ReadyToStart(CheckAllReady());
    }

    // 전체가 다 레디상태일때 true를 리턴하겠지
    private bool CheckAllReady()
    {
        bool result = true;

        foreach (var user in _userList)
        {
            if (!user.ready)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    private void HandleUserListChanged(NetworkListEvent<UserListEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<UserListEntityState>.EventType.Add:
                _readyUI.AddUserData(evt.Value);
                break;
            case NetworkListEvent<UserListEntityState>.EventType.Remove:
                _readyUI.RemoveUserData(evt.Value);
                break;
            case NetworkListEvent<UserListEntityState>.EventType.Value:
                _readyUI.UpdateUserData(evt.Value);
                break;
        }
    }

    public UserListEntityState GetUserEntity(ulong clientID)
    {
        return _userList[FindIndex(clientID)];
    }
}
