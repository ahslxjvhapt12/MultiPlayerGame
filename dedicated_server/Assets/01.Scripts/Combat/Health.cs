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

    // ����ó���� �ؾ���. currentHealth�� ���ؼ�
    // ������ Ŭ�� �ٸ��� �ؾ���
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleChangeHealth;
            HandleChangeHealth(0, maxHealth);
        }

        if (!IsServer) return;
        currentHealth.Value = maxHealth; // ������
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleChangeHealth;
        }
    }

    // ������ ���� ������ �� �ְ�, Ŭ��� ����� ���� ������� ������ �̺�Ʈ�� �����ϴ°Ŵ�
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
