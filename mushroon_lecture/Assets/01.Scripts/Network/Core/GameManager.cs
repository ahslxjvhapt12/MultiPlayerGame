using System;
using Unity.Netcode;
using UnityEngine;

public enum GameState
{
    Ready,
    Game,
    Win,
    Lose
}

public enum GameRole : ushort
{
    Host,
    Client
}

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public event Action<GameState> GameStateChanged; // ������ ���°� ������ �� ����Ǵ� ��.
    private GameState _gameState; // ���� ���� ����

    [SerializeField] private Transform _spawnPosition;
    public Color[] slimeColors; // �������� �÷�
    public NetworkList<GameData> players;

    public GameRole myGameRole;

    private ushort _colorIdx = 0;


    private void Awake()
    {
        Instance = this;
        players = new NetworkList<GameData>();
    }

    // �̰� �������� ���� ���� �ɰŴ�
    private void Start()
    {
        _gameState = GameState.Ready;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            HostSingleton.Instance.GameManager.OnPlayerConnect += OnPlayerConnectHandle;
            HostSingleton.Instance.GameManager.OnPlayerDisconnect += OnPlayerDisconeectHandle;

            // ���⼭ ������ �ȵǰ� �Ǿ��ִ�. ���߿� ó������� �Ѵ�.
            var gameData = HostSingleton.Instance.GameManager.NetServer.GetUserDataByClientID(OwnerClientId);
            OnPlayerConnectHandle(gameData.userAuthID, OwnerClientId);
            myGameRole = GameRole.Host;
        }
        else
        {
            myGameRole = GameRole.Client;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsHost)
        {
            HostSingleton.Instance.GameManager.OnPlayerConnect -= OnPlayerConnectHandle;
            HostSingleton.Instance.GameManager.OnPlayerDisconnect -= OnPlayerDisconeectHandle;
        }
    }

    private void OnPlayerConnectHandle(string authID, ulong clientID)
    {
        UserData data = HostSingleton.Instance.GameManager.NetServer.GetUserDataByClientID(clientID);
        players.Add(new GameData
        {
            clientID = clientID,
            playerName = data.name,
            ready = false,
            colorIdx = 0,
        });
        ++_colorIdx;
    }

    private void OnPlayerDisconeectHandle(string authID, ulong clientID)
    {
        foreach (GameData data in players)
        {
            if (data.clientID != clientID) continue;
            try
            {
                players.Remove(data);
            }
            catch
            {
                Debug.LogError($"{data} ������ ���� �߻�");
            }
            break;
        }
    }
}
