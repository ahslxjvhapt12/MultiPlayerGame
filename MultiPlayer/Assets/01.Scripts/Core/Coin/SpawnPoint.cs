using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SpawnPoint : NetworkBehaviour
{
    public string pointName;
    public Vector3 Position => transform.position;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public float Radius { get; private set; } = 10f;

    public List<Vector3> spawnPointList { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            spawnPointList = MapManager.Instance.GetAvailablePositionList(Position, Radius);
        }
    }

    public void ShowIcon(bool show)
    {
        _spriteRenderer.enabled = show;
    }

    public void BlinkIcon(bool start)
    {
        if (start)
            _spriteRenderer.DOFade(0.2f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        else
        {
            _spriteRenderer.DOKill();
            _spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Position, Radius);
        Gizmos.color = Color.white;
    }
#endif
}
