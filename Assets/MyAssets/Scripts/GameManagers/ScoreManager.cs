using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : Singleton<ScoreManager>
{
    public UnityEvent<int, int> OnScoreChanged; // playerIndex, newScore

    private int player1Score;
    private int player2Score;

    public int Player1Score => player1Score;
    public int Player2Score => player2Score;

    public void AddScore(int playerIndex, int amount)
    {
        if(playerIndex == 1)
        {
            player1Score += amount;
            player1Score = Mathf.Max(0, player1Score);

            print($"[ScoreManager] Player 1 now has Score: {player1Score}");
            OnScoreChanged?.Invoke(1, player1Score);
        }

        if(playerIndex == 2)
        {
            player2Score += amount;
            player2Score = Mathf.Max(0, player2Score);

            print($"[ScoreManager] Player 2 now has Score: {player2Score}");
            OnScoreChanged?.Invoke(2, player2Score);
        }
    }
}
