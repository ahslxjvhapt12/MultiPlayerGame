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
        ulong ownerID = player.HealthCompo.OwnerClientId;
        ulong killerID = player.HealthCompo.LastHitDealerID;
        UserData killerData = ServerSingleton.Instance.NetServer.GetUserDataByClientID(killerID);

        if (killerData != null)
        {
            Debug.Log($"{ownerID} 님이 죽었습니다. by {killerData.username}");
        }
        // 누가 죽였는지 알아내서 로그를 찍자
        // ownerID 님이 죽었습니다. (by KillerName);
        //LastHitDealerID
        // 실제로 서버에서 3초후 리스폰 되도록 함수를 만들자
        StartCoroutine(DelayRespawn(ownerID));
    }

    IEnumerator DelayRespawn(ulong ownerID)
    {
        yield return new WaitForSeconds(3f);
        ServerSingleton.Instance.NetServer.RespawnPlayer(ownerID);
    }
}
