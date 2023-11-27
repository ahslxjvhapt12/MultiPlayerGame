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

        // 클라이언트면 
        // 랭크리스트에 변화에 리스닝을 해줘야 겠지?
        // 맨 처음 접속시에는 리스트에 있는 모든 애들을 추가하는 작업도 해야해

        //서버면 

        if (IsServer)
        {
            ServerSingleton.Instance.NetServer.OnUserJoin += HandleUserJoin;
            ServerSingleton.Instance.NetServer.OnUserLeft += HandleUserLeft;
        }
    }

    public override void OnNetworkDespawn()
    {
        //여기다 클라도 알잘딱 끊어줘야 한다.
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
        //랭킹보드에 추가를 해줘야겠지? 알잘딱으로(리스트에서)
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
        //랭킹보드에서 해당 클라이언트 아이디를 제거해줘야겠지?(리스트에서)
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

        //중복이 있는지 검사후에 만들어서 
        //만들때 clientID넣어주는거 잊지말자.
        //UI에 추가하고 차후 중복검사를 위해서 _rankUIList 에도 넣어준다.
    }

    private void RemoveFromUIList(ulong clientID)
    {
        var instance = _rankUIList.FirstOrDefault((x) => x.clientID == clientID);
        if (instance != null)
        {
            _rankUIList.Remove(instance);
            Destroy(instance.gameObject);
        }
        //_rankUIList 에서 clientID가 일치하는 녀석을 찾아서 리스트에서 제거하고
        // 해당 게임오브젝트를 destroy()
    }
}