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

    // ȣ��Ʈ�� ����ϴ� ����
    private int _readyUserCount = 0;

    public EggManager EggManager { get; private set; }
    public TurnManager TurnManager { get; private set; }

    private void Awake()
    {
        Instance = this;
        players = new NetworkList<GameData>();
        EggManager = GetComponent<EggManager>();
        TurnManager = GetComponent<TurnManager>();
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
                --_colorIdx;
            }
            catch
            {
                Debug.LogError($"{data} ������ ���� �߻�");
            }
            break;
        }
    }

    public void GameReady()
    {
        // ���� RPC�� ���ʸ� ���� �� �ִ�
        SendReadyStateServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendReadyStateServerRpc(ulong clientID)
    {
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].clientID != clientID) continue;

            var old = players[i];
            players[i] = new GameData
            {
                clientID = clientID,
                playerName = old.playerName,
                ready = !old.ready,
                colorIdx = old.colorIdx,
            };

            _readyUserCount += players[i].ready ? 1 : -1;
            break;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        players?.Dispose();
    }

    public void GameStart()
    {
        if (!IsHost) return;
        if (_readyUserCount >= 1)
        {
            SpawnPlayers();
            StartGameClientRpc();
        }
        else
        {
            Debug.Log("���� �غ���");
        }
    }

    private void SpawnPlayers()
    {
        foreach (var player in players)
        {
            HostSingleton.Instance.GameManager.NetServer.SpawnPlayer(player.clientID, _spawnPosition.position, player.colorIdx);
        }
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        _gameState = GameState.Game;
        GameStateChanged?.Invoke(_gameState);
    }

    public void SendResultToClient(GameRole winner)
    {
        HostSingleton.Instance.GameManager.NetServer.DestroyAllPlayer();
        SendResultToClientRpc(winner);
    }

    [ClientRpc]
    public void SendResultToClientRpc(GameRole winner)
    {
        if (winner == myGameRole)
        {
            _gameState = GameState.Win;
            SignalHub.OnEndGame?.Invoke(true);
        }
        else
        {
            _gameState = GameState.Lose;
            SignalHub.OnEndGame?.Invoke(false);
        }


        GameStateChanged?.Invoke(_gameState);
    }

}
