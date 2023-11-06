using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="SO/Tank/TankData")]
public class TankDataSO : ScriptableObject
{
    public int tankID;
    public string tankName;
    public TurretDataSO basicTurretSO;
    public Sprite basicTurretSprite;
    public Sprite bodySprite;
    public float moveSpeed;
    public float rotateSpeed;
    public int maxHP;
}
