using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] CinemachineVirtualCamera _followCam;

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawn;

    public Health HealthCompo { get; private set; }
    NetworkVariable<FixedString32Bytes> _username = new NetworkVariable<FixedString32Bytes>();

    private void Awake()
    {
        HealthCompo = GetComponent<Health>();
        HealthCompo.OnDie += HandleDie;
    }

    private void HandleDie(Health health)
    {
        Destroy(gameObject); // 파티클 등등 이것저것 은 알잘딱 나중에
    }

    public override void OnNetworkSpawn()
    {
        _username.OnValueChanged += HandleNameChanged;
        HandleNameChanged("", _username.Value);
        if (IsOwner)
        {
            _followCam.Priority = 15;
        }

        if (IsServer)
        {
            OnPlayerSpawned?.Invoke(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        _username.OnValueChanged -= HandleNameChanged;
        if (IsServer)
        {
            OnPlayerDespawn?.Invoke(this);
        }
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
