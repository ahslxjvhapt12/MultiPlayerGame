using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "SO/InputReader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    public Vector2 TouchPosition { get; private set; }
    public event Action<bool> OnTouchEvent;

    private Controls _controls;

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }

        _controls.Player.Enable();
    }

    public void OnMoveDelta(InputAction.CallbackContext context)
    {
        TouchPosition = context.ReadValue<Vector2>();
    }

    public void OnTouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnTouchEvent?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnTouchEvent?.Invoke(false);
        }
    }
}
