using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("���� ������")]
    [SerializeField] private InputReader _inputReader;
    //[SerializeField] private Transform _projectileSpawnTrm;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private Collider2D _playerCollider;

    [Header("���� ����")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireCooltime;

    private bool _shouldFire;
    private float _prevFireTime;
    private NetworkVariable<int> _damage = new NetworkVariable<int>();
    private List<Transform> _firePosTrm = new List<Transform>();

    public UnityEvent OnFire;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.PrimaryFireEvent += HandleFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.PrimaryFireEvent -= HandleFire;
    }

    private void HandleFire(bool button)
    {
        _shouldFire = button;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!_shouldFire) return;

        if (Time.time < _prevFireTime + _fireCooltime) return;
        foreach (Transform t in _firePosTrm)
        {
            PrimaryFireServerRPC(t.position, t.up);
            SpawnDummyProjectile(t.position, t.up);
        }

        _prevFireTime = Time.time;
    }

    /// <summary>
    /// ������ �ִ� �� ��ũ�� �� �޼��带 �����Ű�°� RPC��
    /// </summary>
    [ServerRpc]
    private void PrimaryFireServerRPC(Vector3 position, Vector3 dir)
    {
        // ������ ������ �ִ°�
        var instance = Instantiate(_serverProjectilePrefab, position, Quaternion.identity);
        instance.transform.up = dir;
        Physics2D.IgnoreCollision(_playerCollider, instance.GetComponent<Collider2D>());

        if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }

        if (instance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealContact))
        {
            dealContact.SetDamage(_damage.Value);
            dealContact.SetOwner(OwnerClientId);
        }

        SpawnDummyProjectileClientRPC(position, dir);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRPC(Vector3 position, Vector3 dir)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(position, dir);
    }

    private void SpawnDummyProjectile(Vector3 position, Vector3 dir)
    {
        var instance = Instantiate(_clientProjectilePrefab, position, Quaternion.identity);
        instance.transform.up = dir; // �̻����� �ش� �������� ȸ����Ű��.
        Physics2D.IgnoreCollision(_playerCollider, instance.GetComponent<Collider2D>());

        OnFire?.Invoke();
        if (instance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = rigidbody.transform.up * _projectileSpeed;
        }
    }

    public void SetDamage(int damage)
    {
        _damage.Value = damage;
    }

    public void SetFirePos(Vector3[] firePos)
    {
        // �ͷ��� TurretPivot�� Turret�� ã�ƿͼ�
        Transform parent = transform.Find("TurretPivot/Turret");
        // firePos�� ��ġ���ٰ� ���ο� ���ӿ�����Ʈ�� ����
        foreach (Vector3 point in firePos)
        {
            GameObject spawnPoint = new GameObject();
            spawnPoint.transform.SetParent(parent);
            spawnPoint.transform.localPosition = point;

            _firePosTrm.Add(spawnPoint.transform);
        }
        // �Ʊ� ã�� Turret�� �ڽ����� �¸� �ٿ��ָ� �ȴ�.
        // �ڽ����� �ٿ��� ���ӿ�����Ʈ�� transform��
        // firePosTrm ����Ʈ�� ��Ĭ �־��ָ� �ϼ��ȴ�.
    }
}
