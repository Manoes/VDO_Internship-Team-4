using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private GameObject floatingScorePrefab;

    [Header("Combo")]
    [SerializeField] private float comboResetTime = 2f;
    [SerializeField] private int maxCombo = 5;

    public UnityEvent<int, int> OnScoreChanged; // playerIndex, newScore
    public UnityEvent<int, int> OnComboChanged; // playerIndex, Combo

    private int player1Score;
    private int player2Score;

    private int player1Combo;
    private int player2Combo;

    private float player1ComboTimer;
    private float player2ComboTimer;

    public int Player1Score => player1Score;
    public int Player2Score => player2Score;

    void Update()
    {
        TickCombo(1, ref player1ComboTimer, ref player1Combo);
        TickCombo(2, ref player2ComboTimer, ref player2Combo);
    }

    private void TickCombo(int playerIndex, ref float timer, ref int combo)
    {
        if (combo <= 0) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            OnComboChanged?.Invoke(playerIndex, 0);
            combo = 0;
        }
    }

    public void AddScore(int playerIndex, int baseAmount, Vector3 worldPosition)
    {
        int finalAmount = baseAmount;

        if (baseAmount > 0)
        {
            int combo = AddCombo(playerIndex);
            finalAmount *= combo;
        }
        else
        {
            ResetCombo(playerIndex);
        }

        if (playerIndex == 1)
        {
            player1Score = Mathf.Max(0, player1Score + finalAmount);

            print($"[ScoreManager] Player 1 now has Score: {player1Score}");
            OnScoreChanged?.Invoke(1, player1Score);
        }

        if (playerIndex == 2)
        {
            player2Score = Mathf.Max(0, player2Score + finalAmount);

            print($"[ScoreManager] Player 2 now has Score: {player2Score}");
            OnScoreChanged?.Invoke(2, player2Score);
        }

        SpawnFloatingScore(finalAmount, worldPosition);
    }

    private int AddCombo(int playerIndex)
    {
        if (playerIndex == 1)
        {
            player1Combo = Mathf.Clamp(player1Combo + 1, 1, maxCombo);
            OnComboChanged?.Invoke(1, player1Combo);
            player1ComboTimer = comboResetTime;
            return player1Combo;
        }

        player2Combo = Mathf.Clamp(player2Combo + 1, 1, maxCombo);
        OnComboChanged?.Invoke(2, player2Combo);
        player2ComboTimer = comboResetTime;
        return player2Combo;
    }

    private void ResetCombo(int playerIndex)
    {
        if (playerIndex == 1)
        {
            player1Combo = 0;
            player1ComboTimer = 0f;
            OnComboChanged?.Invoke(1, 0);
        }
        else
        {
            player2Combo = 0;
            player2ComboTimer = 0f;
            OnComboChanged?.Invoke(2, 0);
        }
    }

    private void SpawnFloatingScore(int amount, Vector3 position)
    {
        if (floatingScorePrefab == null) return;

        GameObject scoreObject = Instantiate(floatingScorePrefab, position, Quaternion.identity);

        if (scoreObject.TryGetComponent(out FloatingScoreText scoreText))
            scoreText.Initialize(amount);
    }
}
