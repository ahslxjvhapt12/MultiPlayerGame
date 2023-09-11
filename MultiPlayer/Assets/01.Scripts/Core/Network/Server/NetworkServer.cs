using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    private NetworkManager _networkManager;
    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }

    // 클라이언트들이 서버에 접속할 때 실행을 시켜줘서 요청에 따라 승인응답할 수도있고 안할수도
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();
        data.Deserialize(req.Payload);
        Debug.Log(data.username);
        res.Approved = true;
    }
}
