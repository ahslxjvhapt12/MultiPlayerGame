using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    int _damage = 0;
    ulong _ownerclientID;

    public void SetDamage(int knifeDamage)
    {
        _damage = knifeDamage;
    }

    public void SetOwner(ulong clientID)
    {
        _ownerclientID = clientID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"{collision.gameObject.name} is hit");

        if (collision.attachedRigidbody == null) return;

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage, _ownerclientID);
        }
    }
}
