using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _createPanelAsset;
    [SerializeField] private VisualTreeAsset _lobbyPaenlAsset;
    [SerializeField] private VisualTreeAsset _lobbyTemplateAsset;

    private UIDocument _uiDocument;
    private VisualElement _contentElem;
    private readonly string _nameKey = "userName";

    private bool _isCreatingLobby = false; // 로비가 생성중인가?
    private CreatePanel _createPanel;
    private LobbyPanel _lobbyPanel;


    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var root = _uiDocument.rootVisualElement;
        root.Q<VisualElement>("popup-panel").RemoveFromClassList("off");

        _contentElem = root.Q<VisualElement>("content");
        root.Q<VisualElement>("tab-container").RegisterCallback<ClickEvent>(TabButtonClickHandle);

        var nameText = root.Q<TextField>("name-text");
        nameText.SetValueWithoutNotify(PlayerPrefs.GetString(_nameKey, string.Empty));

        root.Q<Button>("btn-ok").RegisterCallback<ClickEvent>(e =>
        {
            string name = root.Q<TextField>("name-text").value;
            if (string.IsNullOrEmpty(name)) return;

            PlayerPrefs.SetString(_nameKey, name);
            root.Q<VisualElement>("popup-panel").AddToClassList("off");
        });

        // 크리에이트 패널 만들기
        var createPanel = _createPanelAsset.Instantiate();
        createPanel.AddToClassList("panel");
        root.Q<VisualElement>("page-one").Add(createPanel);
        _createPanel = new CreatePanel(createPanel);
        _createPanel.MakeLobbyBtnEvent += HandleCreateLobby;

        // 로비패널 만들기
        var lobbyPanel = _lobbyPaenlAsset.Instantiate();
        lobbyPanel.AddToClassList("panel");
        root.Q<VisualElement>("page-two").Add(lobbyPanel);
        _lobbyPanel = new LobbyPanel(lobbyPanel, _lobbyTemplateAsset);
    }

    private async void HandleCreateLobby(string lobbyName)
    {
        if (_isCreatingLobby) return; // 이미 만드는중

        if (string.IsNullOrEmpty(lobbyName))
        {
            _createPanel.SetStatusText("로비 이름은 공백일 수 없습니다.");
            return;
        }

        _isCreatingLobby = true;
        string username = PlayerPrefs.GetString(_nameKey);

        LoadText(_createPanel.StatusLabel);
        bool result = await ApplicationController.Instance.StartHost(lobbyName, lobbyName);
        if (result)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneList.GameScene, LoadSceneMode.Single);
        }
        else
        {
            _createPanel.SetStatusText("로비 생성중 오류 발생!");
        }
        _isCreatingLobby = false;
    }

    private async void LoadText(Label targetLabel)
    {
        string[] makings = { "Loading", "Loading.", "Loading..", "Loading...", "Loading...." };
        int idx = 0;
        while (_isCreatingLobby)
        {
            targetLabel.text = makings[idx];
            idx = (idx + 1) % makings.Length;
            await Task.Delay(300);
        }
    }

    private void TabButtonClickHandle(ClickEvent evt)
    {
        if (evt.target is DataVisualElement)
        {
            var dve = evt.target as DataVisualElement;
            var percent = dve.panelIndex * 100;

            _contentElem.style.left = new Length(-percent, LengthUnit.Percent);
            Debug.Log(dve.panelIndex);
        }
    }
}
