using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("참조 값")]
    [SerializeField] private Transform _healPowerBarTrm;

    [Header("세팅 값")]
    [SerializeField] private int _maxHealPower = 30; // 회복시킬 수 있는 틱수
    [SerializeField] private float _cooldown = 60f; // 1분 쿨다운
    [SerializeField] private float _healTickRate = 1f; // 힐 틱
    [SerializeField] private int _coinPerTick = 5; // 1틱당 소모할 코인 양
    [SerializeField] private int _healPerTick = 10; // 1틱에 채워질 체력 량

    private List<TankPlayer> _playersInZone = new List<TankPlayer>();
    private NetworkVariable<int> _healPower = new NetworkVariable<int>();

    private float _remainCooldown; // 쿨타임 찰 때 까지의 시간
    private float _tickTimer; // 다음 틱까지의 시간

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, _healPower.Value); // 나중에 들어온 애가 채워진다.
        }

        if (IsServer)
        {
            _healPower.Value = _maxHealPower; // 서버니까 자기 힐 파워 처음 시작시 세팅
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }

    private void HandleHealPowerChanged(int oldPower, int newPower)
    {
        _healPowerBarTrm.localScale = new Vector3((float)newPower / _maxHealPower, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 내가 서버가 아니면 이걸 할 필요가 없어

        // 서버인 경우 나에게 들어온 녀석에게서 TankPlayer가 있는지 검사해서
        // 있다면 리스트에 넣어라

        if (IsServer)
        {
            if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tp))
            {
                _playersInZone.Add(tp);
                Debug.Log(tp.name);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer tp))
            {
                _playersInZone.Remove(tp);
                Debug.Log(tp.name);
            }
        }
    }

    private void Update()
    {

    }
}
