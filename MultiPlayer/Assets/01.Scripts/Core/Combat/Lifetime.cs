using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private float _currenttime = 0;

    private void Update()
    {
        _currenttime += Time.deltaTime;
        if (_currenttime >= _lifetime)
        {
            Destroy(gameObject);
        }
    }
}
