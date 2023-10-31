using Unity.Netcode;

public class ScoreManager : NetworkBehaviour
{
    public NetworkVariable<int> hostScore = new NetworkVariable<int>();
    public NetworkVariable<int> clientScore = new NetworkVariable<int>();

    private void HandleScoreChanged(int oldScore, int newScore)
    {
        SignalHub.OnScoreChanged(hostScore.Value, clientScore.Value);
    }

    public override void OnNetworkSpawn()
    {
        hostScore.OnValueChanged += HandleScoreChanged;
        clientScore.OnValueChanged += HandleScoreChanged;

        if (!IsServer) return;
        Egg.OnFallInWater += HandleFallInWater;
    }

    public override void OnNetworkDespawn()
    {
        hostScore.OnValueChanged -= HandleScoreChanged;
        clientScore.OnValueChanged -= HandleScoreChanged;

        if (!IsServer) return;
        Egg.OnFallInWater -= HandleFallInWater;
    }

    private void HandleFallInWater()
    {
        switch (GameManager.Instance.TurnManager.currentTurn.Value)
        {
            case GameRole.Host:
                clientScore.Value += 1;
                break;
            case GameRole.Client:
                hostScore.Value += 1;
                break;
        }

        CheckForEndGame();
    }

    private void CheckForEndGame()
    {
        if (hostScore.Value >= 3)
        {
            GameManager.Instance.SendResultToClient(GameRole.Host);
        }
        else if (clientScore.Value >= 3)
        {
            GameManager.Instance.SendResultToClient(GameRole.Client) ;
        }
        else
        {
            GameManager.Instance.EggManager.ResetEgg();
        }
    }

    private void Start()
    {
        InitializeScore();
    }

    private void InitializeScore()
    {
        hostScore.Value = 0;
        clientScore.Value = 0;
        // 나중에 UI 갱신까지 해줄거다
    }
}
