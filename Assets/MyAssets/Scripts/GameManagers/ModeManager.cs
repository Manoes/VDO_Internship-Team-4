using UnityEngine;

public enum GameMode
{
    Solo,
    TwoPlayer
}

public class ModeManager : Singleton<ModeManager>
{
    public static GameMode SelectedMode { get; private set; } = GameMode.Solo;

    [Header("Players")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    [SerializeField] private Transform player2SpawnPoint;

    public GameMode CurrentMode => SelectedMode;
    public bool IsSolo => SelectedMode == GameMode.Solo;
    public bool IsTwoPlayer => SelectedMode == GameMode.TwoPlayer;

    public static void SetMode(GameMode mode)
    {
        SelectedMode = mode;
    }

    protected override void Awake()
    {
        base.Awake();
        LockCursor();
    }

    void Start()
    {
        SetupMode();
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void SetupMode()
    {
        if (SelectedMode == GameMode.Solo)
            AddHealthIfMissing(player1Prefab);
        else if (SelectedMode == GameMode.TwoPlayer)
            SpawnPlayer2();
    }

    private void AddHealthIfMissing(GameObject player)
    {
        if (player == null) return;

        if (!player.TryGetComponent<PlayerHealth>(out _))
            player.AddComponent<PlayerHealth>();
    }

    private void SpawnPlayer2()
    {
        if (player2Prefab == null || player2SpawnPoint == null)
            return;

        Instantiate(player2Prefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
    }
}
