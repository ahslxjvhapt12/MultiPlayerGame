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
    [Header("참조변수들")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _shootPositionTrm; // 이거의 right 로 쏘면 됌
    [SerializeField] private GameObject _serverKnife;
    [SerializeField] private GameObject _clientKnife;
    [SerializeField] private Collider2D _playerCollider;

    // 서버용 프리팹이 필요하고
    // 클라이언트용 프리팹이 필요하다
    // 자기자신과 충돌방지를 위해서 플레이어 콜라이더도 필요하다

    [Header("셋팅값들")]
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
        //쿨타임이 돌아왔다면 발사 하는거야
        if (_lastThrowTime + _throwCooltime > Time.time) return;
        _lastThrowTime = Time.time;


        Vector3 pos = _shootPositionTrm.position;
        Vector3 direction = _shootPositionTrm.right;

        SpawnDummyKnife(pos, direction);
        //클라꺼 쏘고
        SpawnKnifeServerRpc(pos, direction);
        //서버 RPC날리고

        //자기자신은 안만들어주고 나머지 클라는 만든다.
        //모든 나이프는 만들때 
        //Physics2D.IgnoreCollision 을 이용해서 자기자신과는 충돌하지 않게 만든다.
    }

    [ServerRpc]
    private void SpawnKnifeServerRpc(Vector3 pos, Vector3 dir)
    {
        // 이 기사 오브젝트의 주인의 아이디로 이름, 기타 등등을 알아낸다
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