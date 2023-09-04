using DG.Tweening;
using UnityEngine;

public class DecalCircle : MonoBehaviour
{
    [Header("참조 변수")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public bool showDecal = false;

    public void OpenCircle(Vector3 point, float radius)
    {
        _spriteRenderer.color = new Color(1, 1, 1, 0); // 투명상태
        transform.position = point;
        transform.localScale = Vector3.one;

        showDecal = true;
        Sequence seq = DOTween.Sequence()
            .Append(_spriteRenderer.DOFade(1, 0.3f))
            .Append(transform.DOScale(Vector3.one * (radius * 2), 0.8f));
    }

    public void CloseCircle()
    {
        showDecal = false;
        _spriteRenderer.color = new Color(1, 1, 1, 1);
        
        Sequence seq = DOTween.Sequence()
            .Append(_spriteRenderer.DOFade(0, 0.8f))
            .Join(transform.DOScale(Vector3.one, 0.8f));
    }

    //private void Update()
    //{
    //    if (Keyboard.current.qKey.wasPressedThisFrame)
    //    {
    //        OpenCircle(transform.position, 8);
    //    }
    //    if (Keyboard.current.eKey.wasPressedThisFrame)
    //    {
    //        CloseCircle();
    //    }
    //}

}
