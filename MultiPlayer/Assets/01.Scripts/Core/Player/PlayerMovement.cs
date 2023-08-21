using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("����������")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _bodyTrm;
    private Rigidbody2D _rigidbody;

    [Header("���ð���")]
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float _turningRate = 30f;

    private Vector2 _prevMovementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent += HandleMovement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 move)
    {
        _prevMovementInput = move;
    }

    private void Update()
    {
        //��ü�� Tread �� ���� ����

        //���� owner������ �˻��ؾ���. ���ʰ� �ƴ϶�� ������ �ʿ䰡 ���� 

        if (!IsOwner) return;
        // TurningRate�ӵ���ŭ _prevMovement���� X�Է��� ȸ�� ġ�� ���ؼ� 
        // �ٵ� Ʈ�������� ȸ�������ָ� �ȴ�.
        _bodyTrm.Rotate(new Vector3(0, 0, -_prevMovementInput.x * _turningRate * Time.deltaTime));

    }

    private void FixedUpdate()
    {
        //��ġ�� �̵���ų����
        //�������� �˻��ؼ�
        if (!IsOwner) return;

        // ������ٵ��� �ӵ����ٰ� �ٵ��� up�������� y���� �����ؼ� movementSpeed��ŭ �̵������ָ� �ȴ�.
        _rigidbody.velocity = _bodyTrm.up * _prevMovementInput.y * _movementSpeed;

    }

}
