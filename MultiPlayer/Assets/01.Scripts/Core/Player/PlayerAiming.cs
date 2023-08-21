using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _turretTrm;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        Vector2 dir = _turretTrm.position - CameraManager.Instance.MainCam.ScreenToWorldPoint(_inputReader.AimPosition);
        float degree = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _turretTrm.rotation = Quaternion.Euler(new Vector3(0, 0, degree + 90));
    }

}
