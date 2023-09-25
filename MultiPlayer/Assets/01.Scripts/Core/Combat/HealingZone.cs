using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditorInternal;
using UnityEngine;

public class HealingZone : NetworkBehaviour
{
    [Header("���� ��")]
    [SerializeField] private Transform _healPowerBarTrm;

    [Header("���� ��")]
    [SerializeField] private int _maxHealPower = 30; // ȸ����ų �� �ִ� ƽ��
    [SerializeField] private float _cooldown = 60f; // 1�� ��ٿ�
    [SerializeField] private float _healTickRate = 1f; // �� ƽ
    [SerializeField] private int _coinPerTick = 5; // 1ƽ�� �Ҹ��� ���� ��
    [SerializeField] private int _healPerTick = 10; // 1ƽ�� ä���� ü�� ��

    private List<TankPlayer> _playersInZone = new List<TankPlayer>();
    private NetworkVariable<int> _healPower = new NetworkVariable<int>();

    private float _remainCooldown; // ��Ÿ�� �� �� ������ �ð�
    private float _tickTimer; // ���� ƽ������ �ð�

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
