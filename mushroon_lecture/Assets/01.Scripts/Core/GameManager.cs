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
    public event Action<GameState> GameStateChanged; // 게임의 상태가 변했을 때 발행되는 것.
    private GameState _gameState; // 현재 게임 상태

    [SerializeField] private Transform _spawnPosition;
    public Color[] slimeColors; // 슬라임의 컬러
    public NetworkList<GameData> players;

    public GameRole myGameRole;

    private ushort _colorIdx = 0;

    // 호스트만 사용하는 변수
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

    // 이게 스폰보다 먼저 실행 될거다
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

            // 여기서 본인은 안되게 되어있다. 나중에 처리해줘야 한다.
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
                Debug.LogError($"{data} 삭제중 오류 발생");
            }
            break;
        }
    }

    public void GameReady()
    {
        // 서버 RPC는 오너만 보낼 수 있다
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
            Debug.Log("아직 준비중");
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
