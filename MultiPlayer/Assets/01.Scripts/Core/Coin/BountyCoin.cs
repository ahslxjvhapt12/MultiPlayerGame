using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class BountyCoin : Coin
{
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    public override int Collect()
    {
        if (!IsServer)
        {
            SetVisible(false); // �ǰ��°� ������ �� ����
            // Ŭ��� �Ⱥ��̰Ը� ó��

            return 0;
        }
        if (_alreadyCollected) return 0;

        _alreadyCollected = true;
        Destroy(gameObject); // �̷��� ��ü���� �ǰ���
        return _coinValue;
    }

    public void SetCoinToVisible(float coinScale)
    {
        isActive.Value = true;
        CoinSpawnClientRpc(coinScale);
    }

    [ClientRpc]
    private void CoinSpawnClientRpc(float coinScale)
    {
        Vector3 destination = transform.position;
        transform.position = transform.position + new Vector3(0, 3f, 0);
        transform.localScale = 0.5f * Vector3.one;
        Vector3 destinationScale = coinScale * Vector3.one;

        SetVisible(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(destination, 0.8f).SetEase(Ease.OutBounce));
        seq.Join(transform.DOScale(destinationScale, 0.8f));
        seq.InsertCallback(0.6f, () =>
        {
            _impulseSource.GenerateImpulse(0.3f);
        });
    }
}
