using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class BoardItem
{
    private VisualElement _root;
    public VisualElement Root => _root;
    private Label _label;
    private Label _label_coin;

    private string playerName;
    public ulong ClientID { get; private set; }
    public int Coins { get; private set; }
    public int rank = 1;

    private Color _ownerColor;

    public string Text
    {
        get => _label.text;
        set => _label.text = value;
    }

    public string TextCoin
    {
        get => _label_coin.text;
        set => _label_coin.text = value;
    }

    public BoardItem(VisualElement root, LeaderboardEntityState state, Color ownerColor)
    {
        _root = root;
        _label = root.Q<Label>("board-label");
        _label_coin = root.Q<Label>("board-label-coin");
        playerName = state.playerName.Value;
        ClientID = state.clientID;
        _ownerColor = ownerColor;

        UpdateCoin(Coins);
    }

    public void UpdateCoin(int coins)
    {
        Coins = coins;
        UpdateText();
    }

    private void UpdateText()
    {
        if (ClientID == NetworkManager.Singleton.LocalClientId) // 내꺼 순위를 그리려고 한다
        {
            _label.style.color = new Color(_ownerColor.r, _ownerColor.g, _ownerColor.b, _ownerColor.a);
        }
        Text = $"{rank}. {playerName} ";
        TextCoin = $"[{Coins}]";
    }

    public void Show(bool value)
    {
        Root.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
    }
}
