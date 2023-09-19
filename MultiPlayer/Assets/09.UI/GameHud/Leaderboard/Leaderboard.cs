using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Leaderboard
{
    private VisualElement _root;
    private int _displayCount;
    private VisualElement _innerHolder;

    private VisualTreeAsset _boardItemAsset;
    private List<BoardItem> _itemList = new List<BoardItem>();
    Color _color;
    public Leaderboard(VisualElement root, VisualTreeAsset itemAsset, Color color, int displayCount = 7)
    {
        _color = color;
        _root = root;
        _innerHolder = _root.Q<VisualElement>("inner-holder");
        _boardItemAsset = itemAsset;
        _displayCount = displayCount;
    }

    public bool CheckExistByClientID(ulong clientID)
    {
        return _itemList.Any(x => x.ClientID == clientID);
    }

    public void AddItem(LeaderboardEntityState state)
    {
        var root = _boardItemAsset.Instantiate().Q<VisualElement>("board-item");
        _innerHolder.Add(root);
        BoardItem item = new BoardItem(root, state, _color);
        _itemList.Add(item);
    }

    public void RemoveByClientID(ulong clientID)
    {
        BoardItem item = _itemList.FirstOrDefault(x => x.ClientID == clientID);

        if (item != null)
        {
            item.Root.RemoveFromHierarchy(); // UI에서 사라진다
            _itemList.Remove(item);
        }
    }

    public void UpdateByClientID(ulong clientID, int coins)
    {
        // 지정된 클라이언트 아이디의 state를 찾아서 coins를 업데이트 시키시요.
        BoardItem item = _itemList.FirstOrDefault(x => x.ClientID == clientID);

        if (item != null)
            item.UpdateCoin(coins);
    }

    public void SortOrder()
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            for (int j = 0; j < _itemList.Count; j++)
            {
                if (_itemList[i].Coins > _itemList[j].Coins)
                {
                    BoardItem temp = _itemList[i];
                    _itemList[i] = _itemList[j];
                    _itemList[j] = temp;
                }
            }
        }
    }
}
