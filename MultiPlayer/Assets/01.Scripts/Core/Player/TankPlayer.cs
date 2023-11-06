using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("��������")]
    [SerializeField] private GameObject _miniMapIcon;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private ProjectileLauncher _launcher;
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _turretSprite;

    [field: SerializeField] public Health HealthCompo { get; private set; }
    [field: SerializeField] public CoinCollector Coin {  get; private set; }

    [Header("���ð�")]
    [SerializeField] private int _ownerCamPriority;
    [SerializeField] private Color _ownerColor;

    // 32����Ʈ utf���� �ѱ�(3����Ʈ) 10���� ���� 31����
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer) // ���� ��Ʈ��ũ ������ �����Ŵ�.
        {
            // ��Ʈ��ũ ������ �ִ� ��ųʸ��� �̿��ؼ� �� ��ũ�� �̸��� �˾Ƴ�
            // �� ������ �װ� NetworkVariable�� �־��ٰž�
            UserData userData = HostSingletone.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = userData.username;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            _miniMapIcon.GetComponent<SpriteRenderer>().color = _ownerColor;
            _followCam.Priority = _ownerCamPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }

    public void SetTankNetworkVariable(object userData)
    {
        
    }
}
