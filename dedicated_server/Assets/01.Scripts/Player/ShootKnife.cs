using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootKnife : NetworkBehaviour
{
    [Header("����������")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _shootPositionTrm; // �̰��� right �� ��� ��
    [SerializeField] private GameObject _serverKnife;
    [SerializeField] private GameObject _clientKnife;
    [SerializeField] private Collider2D _playerCollider;

    // ������ �������� �ʿ��ϰ�
    // Ŭ���̾�Ʈ�� �������� �ʿ��ϴ�
    // �ڱ��ڽŰ� �浹������ ���ؼ� �÷��̾� �ݶ��̴��� �ʿ��ϴ�

    [Header("���ð���")]
    [SerializeField] private float _knifeSpeed;
    [SerializeField] private int _knifeDamage;
    [SerializeField] private float _throwCooltime;

    private float _lastThrowTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.ShootEvent += HandleShootKnife;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.ShootEvent -= HandleShootKnife;
    }

    private void HandleShootKnife()
    {
        //��Ÿ���� ���ƿԴٸ� �߻� �ϴ°ž�
        if (_lastThrowTime + _throwCooltime > Time.time) return;
        _lastThrowTime = Time.time;


        Vector3 pos = _shootPositionTrm.position;
        Vector3 direction = _shootPositionTrm.right;

        SpawnDummyKnife(pos, direction);
        //Ŭ�� ���
        SpawnKnifeServerRpc(pos, direction);
        //���� RPC������

        //�ڱ��ڽ��� �ȸ�����ְ� ������ Ŭ��� �����.
        //��� �������� ���鶧 
        //Physics2D.IgnoreCollision �� �̿��ؼ� �ڱ��ڽŰ��� �浹���� �ʰ� �����.
    }

    [ServerRpc]
    private void SpawnKnifeServerRpc(Vector3 pos, Vector3 dir)
    {
        // �� ��� ������Ʈ�� ������ ���̵�� �̸�, ��Ÿ ����� �˾Ƴ���
        UserData user = ServerSingleton.Instance.NetServer.GetUserDataByClientID(OwnerClientId);
        var instance = Instantiate(_serverKnife, pos, Quaternion.identity);
        instance.transform.right = dir;
        Physics2D.IgnoreCollision(_playerCollider, instance.GetComponent<CircleCollider2D>());

        if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D _rigid))
        {
            _rigid.velocity = dir * _knifeSpeed;
        }

        if(instance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damage))
        {
            damage.SetDamage(_knifeDamage);
            damage.SetOwner(OwnerClientId);
        }

        SpawnDummyClientRPC(pos, dir);
    }

    [ClientRpc]
    private void SpawnDummyClientRPC(Vector3 pos, Vector3 dir)
    {
        if (IsOwner) return;

        SpawnDummyKnife(pos, dir);
    }

    private void SpawnDummyKnife(Vector3 pos, Vector3 dir)
    {
        var instance = Instantiate(_clientKnife, pos, Quaternion.identity);
        instance.transform.right = dir;
        Physics2D.IgnoreCollision(_playerCollider, instance.GetComponent<CircleCollider2D>());

        if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D _rigid))
        {
            _rigid.velocity = dir * _knifeSpeed;
        }
    }
}