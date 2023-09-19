using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class GameHud : NetworkBehaviour
{
    [SerializeField] private VisualTreeAsset _boardItemsAsset;
    [SerializeField] private int _displayCount = 7;

    private Leaderboard _leaderboard;
    private NetworkList<LeaderboardEntityState> _leaderBoardEntities; // �������� ��ƼƼ�� �ִ°�

    private UIDocument _document;
    [SerializeField] Color _color;
    private void Awake()
    {
        _leaderBoardEntities = new NetworkList<LeaderboardEntityState>();
        _document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = _document.rootVisualElement;
        var boardContainer = root.Q<VisualElement>("leaderboard");
        _leaderboard = new Leaderboard(boardContainer, _boardItemsAsset, _color, _displayCount);
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _leaderBoardEntities.OnListChanged += HandleLeaderboardChanged;

            // ���� �ٲ� �̺�Ʈ�� �߻���Ű�µ�
            // �̹� �� �ֵ��� �̺�Ʈ�� ������ �ȹ޾�
            // �ȿ� �ִ� �ֵ鵵 �̺�Ʈ�� �ް�����
            foreach (var item in _leaderBoardEntities)
            {
                HandleLeaderboardChanged(new NetworkListEvent<LeaderboardEntityState>
                {
                    Type = NetworkListEvent<LeaderboardEntityState>.EventType.Add,
                    Value = item,
                });
            }
            // ���⿡�� �̵��� �ٸ� �߰� ������ �� ����
        }

        if (IsServer)
        {
            TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
            foreach (TankPlayer player in players)
            {
                HandlePlayerSpawned(player);
            }

            TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _leaderBoardEntities.OnListChanged -= HandleLeaderboardChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
        }
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        _leaderBoardEntities.Add(new LeaderboardEntityState
        {
            clientID = player.OwnerClientId,
            playerName = player.playerName.Value,
            coins = 0
        });

        player.Coin.totalCoins.OnValueChanged += (oldCoin, newCoin) =>
        {
            HandleCoinsChanged(player.OwnerClientId, newCoin);
        };
    }

    private void HandleCoinsChanged(ulong ownerClientId, int newCoin)
    {
        for (int i = 0; i < _leaderBoardEntities.Count; ++i)
        {
            if (_leaderBoardEntities[i].clientID != ownerClientId) continue;

            var oldItem = _leaderBoardEntities[i];
            _leaderBoardEntities[i] = new LeaderboardEntityState
            {
                clientID = ownerClientId,
                playerName = oldItem.playerName,
                coins = newCoin
            };
            break;
        }
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        if (_leaderBoardEntities == null) return;
        // ��ũ�� ������� �������� �̹� �� ������Ʈ�� ������ ���� �ִ�.(������ �������� �����غ�)

        foreach (var entity in _leaderBoardEntities)
        {
            if (entity.clientID != player.OwnerClientId) continue;

            try
            {
                _leaderBoardEntities.Remove(entity);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{entity.playerName}-{entity.clientID} ������ ����");
            }
        }
        player.Coin.totalCoins.OnValueChanged = null;
    }
    private void HandleLeaderboardChanged(NetworkListEvent<LeaderboardEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<LeaderboardEntityState>.EventType.Add:
                // ���� ����Ʈ�� ���� �߰��� ���� �������� ���� ��
                if (!_leaderboard.CheckExistByClientID(evt.Value.clientID))
                {
                    _leaderboard.AddItem(evt.Value);
                }
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Remove:
                _leaderboard.RemoveByClientID(evt.Value.clientID);
                break;
            case NetworkListEvent<LeaderboardEntityState>.EventType.Value:
                _leaderboard.UpdateByClientID(evt.Value.clientID, evt.Value.coins);
                break;
        }
        _leaderboard.SortOrder();
    }
}
