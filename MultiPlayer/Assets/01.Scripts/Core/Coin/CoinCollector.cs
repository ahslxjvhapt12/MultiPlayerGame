using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    [Header("���� ����")]
    [SerializeField] private BountyCoin _bountyCoinPrefab;
    [SerializeField] protected Health _health;

    [Header("���� ����")]
    [SerializeField] private float _bountyRatio = 0.8f;

    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Coin>(out Coin c))
        {
            int value = c.Collect();
            if (!IsServer) return;
            totalCoins.Value += value;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        _health.OnDie -= HandleDie;
    }

    private void HandleDie(Health health)
    {
        // ��ü �����߿��� �ٿ�Ƽ�� ���� ������ ������ ����
        int amount = Mathf.FloorToInt(totalCoins.Value * _bountyRatio);

        if (totalCoins.Value < 10) return;
        // �翡 ���� ������ ũ�⸦ ����
        float coinScale = Mathf.Clamp(amount / 100.0f, 1f, 3f);
        // �����
        var bountyCoin = Instantiate(_bountyCoinPrefab, health.transform.position, Quaternion.identity);
        // �� ���ϰ�
        bountyCoin.SetValue(amount);
        // ����
        bountyCoin.NetworkObject.Spawn();
        bountyCoin.SetCoinToVisible(coinScale);
    }

    /// <summary>
    /// ������ ȣ���ϴ� �� ���̴� �ڵ�
    /// </summary>
    /// <param name="value"></param>
    public void SpendCoin(int value)
    {
        totalCoins.Value -= value;
    }
}
