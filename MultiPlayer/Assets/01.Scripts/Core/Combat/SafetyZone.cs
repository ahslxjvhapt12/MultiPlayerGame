using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            if (player.NetworkObject.IsOwner)
            {
                Debug.Log($"Enter : {player.playerName.Value}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            if (player.NetworkObject.IsOwner)
            {
                Debug.Log($"Exit : {player.playerName.Value}");
            }
        }
    }
}
