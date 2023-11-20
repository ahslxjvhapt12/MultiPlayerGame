using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class Player : NetworkBehaviour
{
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] CinemachineVirtualCamera _followCam;
    Rigidbody2D _rigidbody2D;

    NetworkVariable<FixedString32Bytes> _username = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        _username.OnValueChanged += HandleNameChanged;
        HandleNameChanged("", _username.Value);
        if (IsOwner)
            _followCam.Priority = 15;
    }

    private void HandleNameChanged(FixedString32Bytes prev, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    public void SetUserName(string username)
    {
        _username.Value = username;
    }
}
