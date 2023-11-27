using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
    // Player�� �ִ� OnPlayerSpawned�� �����ϰ�
    // ���������� �� // ������

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Player.OnPlayerDespawn += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        Player.OnPlayerDespawn -= HandlePlayerDespawn;
    }

    private void HandlePlayerDespawn(Player player)
    {
        ulong ownerID = player.HealthCompo.OwnerClientId;
        ulong killerID = player.HealthCompo.LastHitDealerID;
        UserData killerData = ServerSingleton.Instance.NetServer.GetUserDataByClientID(killerID);

        if (killerData != null)
        {
            Debug.Log($"{ownerID} ���� �׾����ϴ�. by {killerData.username}");
        }
        // ���� �׿����� �˾Ƴ��� �α׸� ����
        // ownerID ���� �׾����ϴ�. (by KillerName);
        //LastHitDealerID
        // ������ �������� 3���� ������ �ǵ��� �Լ��� ������
        StartCoroutine(DelayRespawn(ownerID));
    }

    IEnumerator DelayRespawn(ulong ownerID)
    {
        yield return new WaitForSeconds(3f);
        ServerSingleton.Instance.NetServer.RespawnPlayer(ownerID);
    }
}
