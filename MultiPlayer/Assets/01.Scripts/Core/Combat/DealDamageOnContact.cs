using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    private int _damage = 10;

    private ulong _ownerClientID;

    public void SetDamage(int value)
    {
        _damage = value;
    }

    public void SetOwner(ulong ownerClientId)
    {
        _ownerClientID = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody is null) return;

        if(other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (netObj.OwnerClientId == _ownerClientID) return;
        }

        if (other.attachedRigidbody.TryGetComponent<Health>(out Health h))
        {
            h.TakeDamage(_damage);
        }
    }
}
