using System;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _lobbyTemplate;
    private TextField _txtIpAdress;
    private TextField _txtPort;
    private TextField _txtJoinCode;

    private UIDocument _uiDocument;
    private const string GameSceneName = "Game";
    private VisualElement _popupPanel;
    private LobbyUI _lobbyUI;



    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        //NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

        var root = _uiDocument.rootVisualElement;
        _txtIpAdress = root.Q<TextField>("txt-ip-address");
        _txtPort = root.Q<TextField>("txt-port");
        _txtJoinCode = root.Q<TextField>("txt-joincode");

        _popupPanel = root.Q<VisualElement>("popup-panel");
        var lobbyRoot = _popupPanel.Q<VisualElement>("lobby-frame");
        _lobbyUI = new LobbyUI(_lobbyTemplate, lobbyRoot, _popupPanel);

        root.Q<Button>("btn-local-host").RegisterCallback<ClickEvent>(OnHandleLocalHost);
        root.Q<Button>("btn-local-client").RegisterCallback<ClickEvent>(OnHandleLocalClient);
        root.Q<Button>("btn-relay-host").RegisterCallback<ClickEvent>(OnHandleRelayHost);
        root.Q<Button>("btn-joincode").RegisterCallback<ClickEvent>(OnHandleRelayJoin);
        root.Q<Button>("btn-lobby").RegisterCallback<ClickEvent>(OnHanddleLobbyOpen);
    }

    private void OnHanddleLobbyOpen(ClickEvent evt)
    {
        _popupPanel.AddToClassList("on");
        // �κ� �������� �ѹ� ���� �ϴµ� ���� �����ȵ�.
    }

    private async void OnHandleRelayJoin(ClickEvent evt)
    {
        string code = _txtJoinCode.value;
        await ClientSingletone.Instance.GameManager.StartClientAsync(code);

        _lobbyUI.RefreshList();
    }

    private async void OnHandleRelayHost(ClickEvent evt)
    {
        await HostSingletone.Instance.GameManager.StartHostAsync();
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    private bool SetUpNetworkPassport()
    {
        var ip = _txtIpAdress.text;
        var port = _txtPort.text;

        var portRegex = new Regex(@"^[0-9]{3,5}$");
        var ipRegex = new Regex(@"^[0-9\.]+$");

        var ipMatch = ipRegex.Match(ip);
        var portMatch = portRegex.Match(port);

        if (!portMatch.Success || !ipMatch.Success)
        {
            Debug.LogError("�ùٸ��� ���� ������ �Ǵ� ��Ʈ ��ȣ�Դϴ�.");
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)int.Parse(port));
        return true;
    }

    private void HandleClientDisconnect(ulong obj)
    {
        Debug.Log(obj + ", �����߻�");
    }

    private void OnHandleLocalClient(ClickEvent evt)
    {
        if (!SetUpNetworkPassport()) return;
        if (NetworkManager.Singleton.StartClient())
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void OnHandleLocalHost(ClickEvent evt)
    {
        if (!SetUpNetworkPassport()) return;
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
