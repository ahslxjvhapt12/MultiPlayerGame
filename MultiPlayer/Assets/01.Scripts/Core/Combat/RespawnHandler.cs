using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer _playerPrefab;

    //private Action<Health> DieAction = null;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; //��Ȱ������ ������ �Ѵ�.

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            HandlePlayerSpawned(player); //�� ������Ʈ�� �����Ǳ����� ���� �÷��̾ �����Ǿ����ٸ�
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDeSpawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return; //��Ȱ������ ������ �Ѵ�.
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDeSpawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.HealthCompo.OnDie += HandlePlayerDie;
    }

    private void HandlePlayerDie(Health player)
    {
        Destroy(player.gameObject);
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientID)
    {
        yield return null; //�Ǵ� ���⼭ 10�� ī��Ʈ�ٿ��� ���̰� ������ �� ���ִ�.

        var instance = Instantiate(_playerPrefab, TankSpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        //�������� ������� �÷��̾ ��� Ŭ���̾�Ʈ���� ������ �����ϸ鼭
        //���ÿ� �� �÷��̾ ������ ���������� �˷��ִ°ž�
        instance.NetworkObject.SpawnAsPlayerObject(ownerClientID);
    }

    private void HandlePlayerDeSpawned(TankPlayer player)
    {
        player.HealthCompo.OnDie -= HandlePlayerDie;
    }
}