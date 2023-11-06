using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public struct TankCombatData : INetworkSerializable
{
    public float moveSpeed;
    public float rotateSpeed;
    public int damage;
    public int maxHP;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref moveSpeed);
        serializer.SerializeValue(ref rotateSpeed);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref maxHP);
    }
}

public struct UserListEntityState : INetworkSerializable, IEquatable<UserListEntityState>
{
    public ulong clientID;
    public FixedString32Bytes playerName;
    public bool ready;
    public int tankID;

    public TankCombatData combatData;

    public bool Equals(UserListEntityState other)
    {
        return clientID == other.clientID && playerName.Equals(other.playerName) && tankID == other.tankID;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref ready);
        serializer.SerializeValue(ref tankID);
        serializer.SerializeValue(ref combatData);
    }

    // 탱크 속도 회전력
}
