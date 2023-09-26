using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("참조 값")]
    [SerializeField] private Transform _healPowerBarTrm;
    [SerializeField] private GameObject _bar;

    [Header("세팅 값")]
    [SerializeField] private int _maxHealPower = 30; // 회복시킬 수 있는 틱수
    [SerializeField] private float _cooldown = 60f; // 1분 쿨다운
    [SerializeField] private float _healTickRate = 1f; // 힐 틱
    [SerializeField] private int _coinPerTick = 5; // 1틱당 소모할 코인 양
    [SerializeField] private int _healPerTick = 10; // 1틱에 채워질 체력 량

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _changeColor;
    
    private List<TankPlayer> _playersInZone = new List<TankPlayer>();
    private NetworkVariable<int> _healPower = new NetworkVariable<int>();

    private Material _barMat;
    private float _remainCooldown; // 쿨타임 찰 때 까지의 시간
    private float _tickTimer; // 다음 틱까지의 시간
    
    private void Awake()
    {
        _barMat = _bar.GetComponent<SpriteRenderer>().material;    
    }

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

        if (!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            _playersInZone.Add(player);
            Debug.Log(player.playerName.Value);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;

        if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            _playersInZone.Remove(player);
            Debug.Log(player.playerName.Value);
        }

    }

    private void Update()
    {
        if (!IsServer) return;

        if (_remainCooldown > 0)
        {
            _remainCooldown -= Time.deltaTime;
            if (_remainCooldown < 0)
            {
                _healPower.Value = _maxHealPower;
            }
            else
            {
                return;
            }
        }

        // 여기에 왔다는건 힐 파워가 존재한다.

        _tickTimer += Time.deltaTime;
        if (_tickTimer >= _healTickRate) // 힐이 들어갈 틱이 되었어
        {
            // 아무튼 뭔가 힐이 차는 무엇인가를 해주고

            foreach (var player in _playersInZone)
            {
                if (player.HealthCompo.currentHealth.Value == player.HealthCompo.MaxHealth) continue;
                if (player.Coin.totalCoins.Value < _coinPerTick) continue;

                player.Coin.SpendCoin(_coinPerTick);
                player.HealthCompo.RestoreHealth(_healPerTick);

                _healPower.Value--;
                if (_healPower.Value <= 0)
                {
                    _remainCooldown = _cooldown;
                    if (IsServer)
                        HealingZoneVisualClientRpc();

                    break;
                }
            }
            _tickTimer = _tickTimer % _healTickRate;
        }
    }

    [ClientRpc]
    private void HealingZoneVisualClientRpc()
    {
        StartCoroutine(HealingZoneCo());
    }

    IEnumerator HealingZoneCo()
    {
        _barMat.SetColor("_EmissionColor", _changeColor);
        _bar.transform.parent.localScale = Vector3.one;
        yield return new WaitForSeconds(_cooldown);
        _barMat.SetColor("_EmissionColor", _originColor);
    }
}
