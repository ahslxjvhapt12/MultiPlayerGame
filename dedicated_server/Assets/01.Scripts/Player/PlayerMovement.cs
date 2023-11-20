using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private PlayerAnimation _playerAnimation;

    private Vector2 _movementInput;
    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader.MovementEvent += HandleMovement;
    }

    private void HandleMovement(Vector2 vec)
    {
        if (!IsOwner) return;

        _movementInput = vec;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        _rigidbody2D.velocity = _movementInput * _movementSpeed;

        _playerAnimation.SetMove(_rigidbody2D.velocity.magnitude > 0.1f);
        _playerAnimation.FilpController(_rigidbody2D.velocity.x);
    }
}
