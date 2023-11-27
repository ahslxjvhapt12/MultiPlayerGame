using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth;

    private bool _isDead = false;
    public Action<Health> OnDie;
    public UnityEvent<int, int, float> OnHealthChanged;

    public ulong LastHitDealerID { get; private set; }

    // 구독처리를 해야함. currentHealth에 대해서
    // 서버와 클라가 다르게 해야함
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleChangeHealth;
            HandleChangeHealth(0, maxHealth);
        }

        if (!IsServer) return;
        currentHealth.Value = maxHealth; // 서버만
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleChangeHealth;
        }
    }

    // 서버는 값을 변경할 수 있고, 클라는 변경된 값을 기반으로 구독한 이벤트를 실행하는거다
    private void HandleChangeHealth(int prev, int newValue)
    {
        OnHealthChanged?.Invoke(prev, newValue, (float)newValue / maxHealth);
    }

    public void TakeDamage(int damageValue, ulong dealerId)
    {
        LastHitDealerID = dealerId;
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    public void ModifyHealth(int value)
    {
        if (_isDead) return;

        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            OnDie?.Invoke(this);
            _isDead = true;
        }

    }
}
