using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class TankPlayer : NetworkBehaviour
{
    [Header("참조변수")]
    [SerializeField] private GameObject _miniMapIcon;
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private ProjectileLauncher _launcher;
    [SerializeField] private SpriteRenderer _bodySprite;
    [SerializeField] private SpriteRenderer _turretSprite;

    [field: SerializeField] public Health HealthCompo { get; private set; }
    [field: SerializeField] public CoinCollector Coin {  get; private set; }

    [Header("세팅값")]
    [SerializeField] private int _ownerCamPriority;
    [SerializeField] private Color _ownerColor;

    // 32바이트 utf기준 한글(3바이트) 10글자 영어 31글자
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>();

    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer) // 나는 네트워크 서버가 있을거다.
        {
            // 네트워크 서버에 있는 딕셔너리를 이용해서 이 탱크의 이름을 알아내
            // 그 다음에 그걸 NetworkVariable에 넣어줄거야
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
