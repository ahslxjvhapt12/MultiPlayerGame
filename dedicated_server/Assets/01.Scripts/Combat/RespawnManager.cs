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
        ulong killerID = player.HealthCompo.LastHitDealerID;
        UserData killerUserdata = ServerSingleton.Instance.NetServer.GetUserDataByClientID(killerID);
        UserData victimUserData = ServerSingleton.Instance.NetServer.GetUserDataByClientID(player.OwnerClientId);

        if (victimUserData != null)
        {
            Debug.Log($"{victimUserData.username} is dead by {killerUserdata.username} [{killerID}]");

            //������ �������� 3���� ������ �ǵ��� �Լ��� �����
            StartCoroutine(DelayRespawn(player.OwnerClientId));
        }

    }

    IEnumerator DelayRespawn(ulong ownerID)
    {
        yield return new WaitForSeconds(3f);
        ServerSingleton.Instance.NetServer.RespawnPlayer(ownerID);
    }
}
