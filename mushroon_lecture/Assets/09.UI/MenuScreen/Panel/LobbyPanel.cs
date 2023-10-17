using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyPanel
{
    private VisualElement _root;
    private Label _statusLabel;

    public ScrollView _lobbyScrollView;
    private bool _isLobbyRefresh = false;
    private VisualTreeAsset _lobbyAsset;

    public LobbyPanel(VisualElement root, VisualTreeAsset lobbyAsset)
    {
        _root = root;
        _lobbyAsset = lobbyAsset;
        _statusLabel = root.Q<Label>("status-label");
        _lobbyScrollView = root.Q<ScrollView>("lobby-scroll");

        root.Q<Button>("btn-refresh").RegisterCallback<ClickEvent>(HandleRefreshBtnClick);
    }

    private async void HandleRefreshBtnClick(ClickEvent evt)
    {
        if (_isLobbyRefresh) return;

        _isLobbyRefresh = true;
        var list = await ApplicationController.Instance.GetLobbyList();

        foreach (var lobby in list)
        {
            var lobbyTemplate = _lobbyAsset.Instantiate();
            _lobbyScrollView.Add(lobbyTemplate);

            lobbyTemplate.Q<Label>("lobby-name").text = lobby.Name;
            lobbyTemplate.Q<Button>("btn-join").RegisterCallback<ClickEvent>(evt =>
            {
                // 여기서 조인하고 머시기 넣기
            });
        }

        _isLobbyRefresh = false;
    }
}
