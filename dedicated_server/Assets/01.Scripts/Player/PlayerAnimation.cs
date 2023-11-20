using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private readonly int _isMoveHash = Animator.StringToHash("is_move");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetMove(bool value)
    {
        _animator.SetBool(_isMoveHash, value);
    }

    public void FilpController(float xDirection)
    {
        bool isRightTurn = xDirection > 0 && _spriteRenderer.flipX;
        bool isLeftTurn = xDirection < 0 && !_spriteRenderer.flipX;

        if(isRightTurn || isLeftTurn)
        {
            Flip();
        }
    }

    public void Flip()
    {
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}