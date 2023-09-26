using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("���� ��")]
    [SerializeField] private Transform _healPowerBarTrm;
    [SerializeField] private GameObject _bar;

    [Header("���� ��")]
    [SerializeField] private int _maxHealPower = 30; // ȸ����ų �� �ִ� ƽ��
    [SerializeField] private float _cooldown = 60f; // 1�� ��ٿ�
    [SerializeField] private float _healTickRate = 1f; // �� ƽ
    [SerializeField] private int _coinPerTick = 5; // 1ƽ�� �Ҹ��� ���� ��
    [SerializeField] private int _healPerTick = 10; // 1ƽ�� ä���� ü�� ��

    [SerializeField] private Color _originColor;
    [SerializeField] private Color _changeColor;
    
    private List<TankPlayer> _playersInZone = new List<TankPlayer>();
    private NetworkVariable<int> _healPower = new NetworkVariable<int>();

    private Material _barMat;
    private float _remainCooldown; // ��Ÿ�� �� �� ������ �ð�
    private float _tickTimer; // ���� ƽ������ �ð�
    
    private void Awake()
    {
        _barMat = _bar.GetComponent<SpriteRenderer>().material;    
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _healPower.OnValueChanged += HandleHealPowerChanged;
            HandleHealPowerChanged(0, _healPower.Value); // ���߿� ���� �ְ� ä������.
        }

        if (IsServer)
        {
            _healPower.Value = _maxHealPower; // �����ϱ� �ڱ� �� �Ŀ� ó�� ���۽� ����
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
        // ���� ������ �ƴϸ� �̰� �� �ʿ䰡 ����

        // ������ ��� ������ ���� �༮���Լ� TankPlayer�� �ִ��� �˻��ؼ�
        // �ִٸ� ����Ʈ�� �־��

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

        // ���⿡ �Դٴ°� �� �Ŀ��� �����Ѵ�.

        _tickTimer += Time.deltaTime;
        if (_tickTimer >= _healTickRate) // ���� �� ƽ�� �Ǿ���
        {
            // �ƹ�ư ���� ���� ���� �����ΰ��� ���ְ�

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
