using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("Solo UI")]
    [SerializeField] private GameObject healthPanel;

    [Header("Two Player UI")]
    [SerializeField] private GameObject twoPlayerPanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject player2ScorePanel;

    [Header("Shared UI")]
    [SerializeField] private GameObject player1ScorePanel;

    void Start()
    {
        ApplyModeUI();
    }

    public void ApplyModeUI()
    {
        bool isTwoPlayer = ModeManager.Instance != null && ModeManager.Instance.IsTwoPlayer;
        bool isSolo = !isTwoPlayer;

        if (healthPanel != null)
            healthPanel.SetActive(isSolo);

        if (twoPlayerPanel != null)
            twoPlayerPanel.SetActive(isTwoPlayer);

        if (timerPanel != null)
            timerPanel.SetActive(isTwoPlayer);

        if (player2ScorePanel != null)
            player2ScorePanel.SetActive(isTwoPlayer);

        if (player1ScorePanel != null)
            player1ScorePanel.SetActive(true); 
    }
}
