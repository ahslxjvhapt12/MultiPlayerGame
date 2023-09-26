using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class CoinCollector : NetworkBehaviour
{
    [Header("참조 변수")]
    [SerializeField] private BountyCoin _bountyCoinPrefab;
    [SerializeField] protected Health _health;

    [Header("설정 값들")]
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
        // 전체 코인중에서 바운티로 만들 코인의 갯수를 구해
        int amount = Mathf.FloorToInt(totalCoins.Value * _bountyRatio);

        if (totalCoins.Value < 10) return;
        // 양에 따라 코인의 크기를 설정
        float coinScale = Mathf.Clamp(amount / 100.0f, 1f, 3f);
        // 만들고
        var bountyCoin = Instantiate(_bountyCoinPrefab, health.transform.position, Quaternion.identity);
        // 양 정하고
        bountyCoin.SetValue(amount);
        // 생성
        bountyCoin.NetworkObject.Spawn();
        bountyCoin.SetCoinToVisible(coinScale);
    }

    /// <summary>
    /// 서버만 호출하는 돈 줄이는 코드
    /// </summary>
    /// <param name="value"></param>
    public void SpendCoin(int value)
    {
        totalCoins.Value -= value;
    }
}
