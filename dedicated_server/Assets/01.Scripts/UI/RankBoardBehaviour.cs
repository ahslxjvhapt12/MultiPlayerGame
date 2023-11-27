using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RankBoardBehaviour : NetworkBehaviour
{
    [SerializeField] private RecordUI _recordPrefab;
    [SerializeField] private RectTransform _recordParentTrm;

    private NetworkList<RankBoardEntityState> _rankList;

    private List<RecordUI> _rankUIList = new List<RecordUI>();

    private void Awake()
    {
        _rankList = new NetworkList<RankBoardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            _rankList.OnListChanged += HandleRankListChanged;
            foreach (var r in _rankList)
            {
                var instance = Instantiate(_recordPrefab, _recordParentTrm);
                _rankUIList.Add(instance);
            }
        }

        // Ŭ���̾�Ʈ�� 
        // ��ũ����Ʈ�� ��ȭ�� �������� ����� ����?
        // �� ó�� ���ӽÿ��� ����Ʈ�� �ִ� ��� �ֵ��� �߰��ϴ� �۾��� �ؾ���

        //������ 

        if (IsServer)
        {
            ServerSingleton.Instance.NetServer.OnUserJoin += HandleUserJoin;
            ServerSingleton.Instance.NetServer.OnUserLeft += HandleUserLeft;
        }
    }

    public override void OnNetworkDespawn()
    {
        //����� Ŭ�� ���ߵ� ������� �Ѵ�.
        if (IsClient)
        {
            _rankList.OnListChanged -= HandleRankListChanged;
        }

        if (IsServer)
        {
            ServerSingleton.Instance.NetServer.OnUserJoin -= HandleUserJoin;
            ServerSingleton.Instance.NetServer.OnUserLeft -= HandleUserLeft;
        }
    }

    private void HandleUserJoin(ulong clientID, UserData userData)
    {
        var instance = new RankBoardEntityState();
        instance.clientID = clientID;
        instance.playerName = ServerSingleton.Instance.NetServer.GetUserDataByClientID(clientID).username;
        _rankList.Add(instance);
        //��ŷ���忡 �߰��� ����߰���? ���ߵ�����(����Ʈ����)
    }

    private void HandleUserLeft(ulong clientID, UserData userData)
    {
        foreach (var instance in _rankList)
        {
            if (instance.clientID == clientID)
            {
                _rankList.Remove(instance);
            }
        }
        //��ŷ���忡�� �ش� Ŭ���̾�Ʈ ���̵� ��������߰���?(����Ʈ����)
    }

    private void HandleRankListChanged(NetworkListEvent<RankBoardEntityState> evt)
    {
        switch (evt.Type)
        {
            case NetworkListEvent<RankBoardEntityState>.EventType.Add:
                AddUIToList(evt.Value);
                break;
            case NetworkListEvent<RankBoardEntityState>.EventType.Remove:
                RemoveFromUIList(evt.Value.clientID);
                break;
            case NetworkListEvent<RankBoardEntityState>.EventType.Value:
                break;
        }
    }

    private void AddUIToList(RankBoardEntityState value)
    {
        foreach (var item in _rankUIList)
        {
            if (item.clientID == value.clientID) return;
        }

        var instance = Instantiate(_recordPrefab, _recordParentTrm);
        instance.SetOwner(value.clientID);
        instance.SetText(1, ServerSingleton.Instance.NetServer.GetUserDataByClientID(value.clientID).username, 0);
        _rankUIList.Add(instance);

        //�ߺ��� �ִ��� �˻��Ŀ� ���� 
        //���鶧 clientID�־��ִ°� ��������.
        //UI�� �߰��ϰ� ���� �ߺ��˻縦 ���ؼ� _rankUIList ���� �־��ش�.
    }

    private void RemoveFromUIList(ulong clientID)
    {
        var instance = _rankUIList.FirstOrDefault((x) => x.clientID == clientID);
        if (instance != null)
        {
            _rankUIList.Remove(instance);
            Destroy(instance.gameObject);
        }
        //_rankUIList ���� clientID�� ��ġ�ϴ� �༮�� ã�Ƽ� ����Ʈ���� �����ϰ�
        // �ش� ���ӿ�����Ʈ�� destroy()
    }
}