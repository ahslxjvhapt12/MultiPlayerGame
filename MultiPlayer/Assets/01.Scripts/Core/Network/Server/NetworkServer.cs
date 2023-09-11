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

    // Ŭ���̾�Ʈ���� ������ ������ �� ������ �����༭ ��û�� ���� ���������� �����ְ� ���Ҽ���
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        UserData data = new UserData();
        data.Deserialize(req.Payload);
        Debug.Log(data.username);
        res.Approved = true;
    }
}