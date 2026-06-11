using UnityEngine;

public enum GameMode
{
    Solo,
    TwoPlayer
}

public class ModeManager : Singleton<ModeManager>
{
    [Header("Mode")]
    [SerializeField] private GameMode selectedMode = GameMode.Solo;

    [Header("Players")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Transform player2SpawnPoint;

    public GameMode CurrentMode => selectedMode;
    public bool IsSolo => selectedMode == GameMode.Solo;
    public bool IsTwoPlayer => selectedMode == GameMode.TwoPlayer;

    void Start()
    {
        SetupMode();
    }

    private void SetupMode()
    {
        if(selectedMode == GameMode.Solo)
            AddHealthIfMissing(player1Prefab);
        else if(selectedMode == GameMode.TwoPlayer)
            SpawnPlayer2();
    }

    private void AddHealthIfMissing(GameObject player)
    {
        if(player == null) return;

        if(!player.TryGetComponent<PlayerHealth>(out _))
            player.AddComponent<PlayerHealth>();
    }

    private void SpawnPlayer2()
    {
        if(player2Prefab == null || player2SpawnPoint == null)
            return;

        Instantiate(player2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
    }
}
