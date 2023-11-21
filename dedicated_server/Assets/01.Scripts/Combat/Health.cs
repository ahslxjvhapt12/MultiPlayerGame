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

    // 구독처리를 해야함. currentHealth에 대해서
    // 서버와 클라가 다르게 해야함
    public override void OnNetworkSpawn()
    {
        currentHealth.OnValueChanged += HandleChangeHealth;
    }
    // 서버는 값을 변경할 수 있고, 클라는 변경된 값을 기반으로 구독한 이벤트를 실행하는거다
    private void HandleChangeHealth(int prev, int newValue)
    {
        //여기서 알잘딱.
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
        // 여기서는 죽었으면 아무것도 안하고
        // 현재 체력에다가 최대체력 한도로 채워주거나 깍아준다.

        // 여기 체력이 0 이하로 떨어지면 이벤트 발행.
    }
}
