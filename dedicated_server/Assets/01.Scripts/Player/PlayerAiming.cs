using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _handTrm;

    private Camera _mainCam;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    // 마우스 위치를 _inputReader 로 받아서
    // 적절하게 회전시켜주면 된다.

    private void LateUpdate()
    {
        if (!IsOwner) return;

        Vector2 mousePos = _inputReader.AimPosition;
        Vector3 worldPos = _mainCam.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        Vector3 dir = (worldPos - transform.position).normalized;

        _handTrm.right = dir;
    }

}
