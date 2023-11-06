using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAligner : MonoBehaviour
{
    private ParticleSystem.MainModule _particleMain;

    private void Awake()
    {
        _particleMain = GetComponent<ParticleSystem>().main;
    }

    private void Update()
    {
        _particleMain.startRotation = -transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
    }
}
