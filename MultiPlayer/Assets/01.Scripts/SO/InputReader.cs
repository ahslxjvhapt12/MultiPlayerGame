using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(menuName = "SO/Input/Reader", fileName = "New Input reader")]
public class InputReader : ScriptableObject, IPlayerActions, IUIActions
{
    public event Action<bool> PrimaryFireEvent;
    public event Action<Vector2> MovementEvent;
    public event Action PressMenuEvent;

    public Vector2 AimPosition { get; private set; }
    private Controls _controlAction;

    private void OnEnable()
    {
        if (_controlAction == null)
        {
            _controlAction = new Controls();
            _controlAction.Player.SetCallbacks(this);
            _controlAction.UI.SetCallbacks(this);
        }
        _controlAction.Player.Enable();
        _controlAction.UI.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        MovementEvent?.Invoke(value);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimPosition = context.ReadValue<Vector2>();
    }

    public void OnOpenUI(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PressMenuEvent?.Invoke();
        }
    }

    public void SetPlayerInputStatus(bool value)
    {
        if (value)
        {
            _controlAction.Player.Enable();
        }
        else
        {
            _controlAction.Player.Disable();
        }
    }
}