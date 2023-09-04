using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuScreen : MonoBehaviour
{
    private TextField _txtIpAdress;
    private TextField _txtPort;

    private UIDocument _uiDocument;
    private const string GameSceneName = "Game";

    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        var root = _uiDocument.rootVisualElement;
        _txtIpAdress = root.Q<TextField>("txt-ip-address");
        _txtPort = root.Q<TextField>("txt-port");

        root.Q<Button>("btn-local-host").RegisterCallback<ClickEvent>(OnHandleLocalHost);
        root.Q<Button>("btn-local-client").RegisterCallback<ClickEvent>(OnHandleLocalClient);
    }

    private void OnDisable()
    {
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
            Debug.LogError("올바르지 못한 아이피 또는 포트 번호입니다.");
            return false;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)int.Parse(port));
        return true;
    }

    private void HandleClientDisconnect(ulong obj)
    {
        Debug.Log(obj + ", 에러발생");
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
