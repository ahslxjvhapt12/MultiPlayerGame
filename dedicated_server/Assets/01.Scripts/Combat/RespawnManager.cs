using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : NetworkBehaviour
{
    // Player에 있는 OnPlayerSpawned를 구독하고
    // 구독해제도 함 // 서버가

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

            //실제로 서버에서 3초후 리스폰 되도록 함수를 만들어
            StartCoroutine(DelayRespawn(player.OwnerClientId));
        }

    }

    IEnumerator DelayRespawn(ulong ownerID)
    {
        yield return new WaitForSeconds(3f);
        ServerSingleton.Instance.NetServer.RespawnPlayer(ownerID);
    }
}
