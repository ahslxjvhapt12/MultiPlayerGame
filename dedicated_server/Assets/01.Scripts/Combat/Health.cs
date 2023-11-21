using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth;

    private bool _isDead = false;
    public Action<Health> OnDie;
    public UnityEvent<int, int, float> OnHealthChanged;

    // ����ó���� �ؾ���. currentHealth�� ���ؼ�
    // ������ Ŭ�� �ٸ��� �ؾ���
    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += HandleChangeHealth;
    }
    // ������ ���� ������ �� �ְ�, Ŭ��� ����� ���� ������� ������ �̺�Ʈ�� �����ϴ°Ŵ�
    private void HandleChangeHealth(int prev, int newValue)
    {
        //���⼭ ���ߵ�.
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    public void ModifyHealth(int value)
    {
        // ���⼭�� �׾����� �ƹ��͵� ���ϰ�
        // ���� ü�¿��ٰ� �ִ�ü�� �ѵ��� ä���ְų� ����ش�.

        // ���� ü���� 0 ���Ϸ� �������� �̺�Ʈ ����.
    }
}
