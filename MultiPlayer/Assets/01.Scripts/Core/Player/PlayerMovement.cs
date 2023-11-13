using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("����������")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _bodyTrm;
    [SerializeField] private ParticleSystem _dustCloudEffect;
    private Rigidbody2D _rigidbody;

    [Header("���ð���")]
    [SerializeField] private NetworkVariable<float> _movementSpeed = new NetworkVariable<float>(4f);
    [SerializeField] private NetworkVariable<float> _turningRate = new NetworkVariable<float>(30f);
    [SerializeField] private float _dustParticleEmissionValue = 10;

    private ParticleSystem.EmissionModule _emissionModule;
    private const float particleStopThreshold = 0.005f;

    private Vector2 _prevMovementInput;
    private Vector3 _prevPosition;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _emissionModule = _dustCloudEffect.emission;
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
        _bodyTrm.Rotate(new Vector3(0, 0, -_prevMovementInput.x * _turningRate.Value * Time.deltaTime));

    }

    private void FixedUpdate()
    {
        //��ġ�� �̵���ų����
        //�������� �˻��ؼ�
        if (!IsOwner) return;

        if ((transform.position - _prevPosition).sqrMagnitude > particleStopThreshold)
        {
            _emissionModule.rateOverTime = _dustParticleEmissionValue;
        }
        else
        {
            _emissionModule.rateOverTime = 0;
        }
        _prevPosition = transform.position;

        // ������ٵ��� �ӵ����ٰ� �ٵ��� up�������� y���� �����ؼ� movementSpeed��ŭ �̵������ָ� �ȴ�.
        _rigidbody.velocity = _bodyTrm.up * _prevMovementInput.y * _movementSpeed.Value;

    }

    public void SetTankMovement(float moveSpeed, float rotateSpeed)
    {
        _movementSpeed.Value = moveSpeed;
        _turningRate.Value = rotateSpeed;
    }
}
