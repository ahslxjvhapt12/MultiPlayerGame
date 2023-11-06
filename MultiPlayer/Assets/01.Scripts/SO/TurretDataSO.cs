using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Tank/Turret")]
public class TurretDataSO : ScriptableObject
{
    public int damage;
    public Sprite sprite;
    public Vector3[] firePos;
}
