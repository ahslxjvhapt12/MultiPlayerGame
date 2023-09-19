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
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [field: SerializeField] public Health HealthCompo { get; private set; }
    [field: SerializeField] public CoinCollector Coin {  get; private set; }

    [Header("���ð�")]
    [SerializeField] private int _ownerCamPriority;

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
}
