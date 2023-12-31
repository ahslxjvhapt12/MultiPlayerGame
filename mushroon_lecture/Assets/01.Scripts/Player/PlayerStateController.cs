using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateController : NetworkBehaviour
{
    [Header("������")]
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public NetworkVariable<GameRole> myRole;

    public override void OnNetworkSpawn()
    {
        GameManager.Instance.TurnManager.currentTurn.OnValueChanged += HandleTurnChange;
        if (IsServer)
        {
            if (IsOwner)
            {
                myRole.Value = GameRole.Host;
            }
            else
            {
                myRole.Value = GameRole.Client;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        GameManager.Instance.TurnManager.currentTurn.OnValueChanged -= HandleTurnChange;
    }

    private void HandleTurnChange(GameRole previousValue, GameRole newValue)
    {
        if (newValue == myRole.Value)
        {
            EnablePlayer(true, 100);
        }
        else
        {
            EnablePlayer(false);
        }
    }

    private async void EnablePlayer(bool value, int waitTime = 0)
    {
        await Task.Delay(waitTime);
        _collider.enabled = value;
        var color = _spriteRenderer.color;
        color.a = value ? 1f : 0.3f;
        _spriteRenderer.color = color;
    }

    [ClientRpc]
    public void InitStateClientRpc(bool value)
    {
        EnablePlayer(value);
    }

}