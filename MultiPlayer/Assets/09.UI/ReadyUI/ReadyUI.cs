using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReadyUI : MonoBehaviour
{
    public event Action<int> TankSelect;
    public event Action<bool> ReadyChanged;

    public event Action GameStarted;

    private UIDocument _uiDocument;

    private VisualElement _selectPanel;
    private Label _selectedTankNameLabel;
    private Label _selectedTankDescLabel;
    private Button _readyBtn;
    private Button _startBtn;

    // 현재 내 클라이언트가 레디인지를 넣는거
    private bool _isReady = false;

    private VisualElement _readyList;

    private Dictionary<int, TankDataSO> _tankDataDictionary = new Dictionary<int, TankDataSO>();
    private Dictionary<ulong, UserUI> _userDictionary = new Dictionary<ulong, UserUI>();

    [SerializeField] private VisualTreeAsset _tankTemplate;
    [SerializeField] private VisualTreeAsset _userTemplate;

    private List<TankUI> _tankUIList = new List<TankUI>(); // 이건 탱크 데이터로 UI 만들어서 배치

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDocument.rootVisualElement;

        _selectPanel = root.Q<VisualElement>("select-panel");
        _selectPanel.RegisterCallback<ClickEvent>(HandleTankClick);

        _selectPanel.Clear(); // 기존에 만들어져있는 더미 빨강 탱크들 다 지워준다.


        _selectedTankNameLabel = root.Q<Label>("tank-name");
        _selectedTankDescLabel = root.Q<Label>("tank-desc");

        _readyList = root.Q<VisualElement>("ready-list");
        _readyList.Clear();

        _readyBtn = root.Q<Button>("btn-ready");
        _startBtn = root.Q<Button>("btn-start");

        _readyBtn.RegisterCallback<ClickEvent>(HandleReadyClick);
        _startBtn.RegisterCallback<ClickEvent>(HandleGameStart);

        _startBtn.SetEnabled(false);
    }

    private void HandleGameStart(ClickEvent evt)
    {
        GameStarted?.Invoke();
    }

    private void HandleReadyClick(ClickEvent evt)
    {
        _isReady = !_isReady; // 반전시키고
        ReadyChanged?.Invoke(_isReady);
    }

    private void HandleTankClick(ClickEvent evt)
    {
        if (_isReady) return;

        TankVisualElement tankElement = evt.target as TankVisualElement;

        if (tankElement != null)
        {
            TankDataSO tankData = _tankDataDictionary[tankElement.tankIndex];
            _selectedTankNameLabel.text = tankData.tankName;

            string desc = $"상대 데이터\n" +
                $"기본 공격력 : {tankData.basicTurretSO.damage}\n " +
                $"이동속도 : {tankData.moveSpeed}\n " +
                $"최대체력 : {tankData.maxHP}\n " +
                $"회전속도 : {tankData.rotateSpeed}";

            _selectedTankDescLabel.text = desc;

            TankSelect?.Invoke(tankData.tankID);
        }
    }

    public void SetTankTemplate(List<TankDataSO> list)
    {
        _selectPanel.Clear();
        _tankUIList.Clear();

        foreach (TankDataSO tank in list)
        {
            _tankDataDictionary.Add(tank.tankID, tank);
            TemplateContainer template = _tankTemplate.Instantiate();
            _selectPanel.Add(template);

            TankUI tankUI = new TankUI(template, tank);
            _tankUIList.Add(tankUI);
        }
    }

    public void AddUserData(UserListEntityState userData)
    {
        if (_userDictionary.ContainsKey(userData.clientID)) return;

        Sprite sprite = userData.tankID != 0 ? _tankDataDictionary[userData.tankID].bodySprite : null;
        string name = userData.playerName.Value;
        TemplateContainer template = _userTemplate.Instantiate();
        UserUI userUI = new UserUI(template, name, sprite, userData.ready);

        _readyList.Add(template);
        _userDictionary.Add(userData.clientID, userUI);
    }

    public void RemoveUserData(UserListEntityState userData)
    {
        UserUI userUI = _userDictionary[userData.clientID];
        userUI.RemoveFromTree();
    }

    public void UpdateUserData(UserListEntityState userData)
    {
        Sprite sprite = userData.tankID != 0 ? _tankDataDictionary[userData.tankID].bodySprite : null;
        UserUI userUI = _userDictionary[userData.clientID];
        userUI.SetTank(sprite);
        userUI.SetReady(userData.ready);
    }

    public void SetHost(bool isHost)
    {
        if (!isHost)
        {
            _startBtn.style.visibility = Visibility.Hidden;
        }
    }

    public void ReadyToStart(bool value)
    {
        _startBtn.SetEnabled(value); // 모두 준비가 완료되었다면 버튼 활성화
    }

    public void HideFromScreen()
    {
        _uiDocument.enabled = false;
    }
}
