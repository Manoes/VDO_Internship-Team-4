using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreUI : MonoBehaviour
{
    [Header("Score Text")]
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;

    [Header("Combo Text")]
    [SerializeField] private TextMeshProUGUI player1ComboText;
    [SerializeField] private TextMeshProUGUI player2ComboText;


    void OnEnable()
    {
        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.AddListener(UpdateScore);
            ScoreManager.Instance.OnComboChanged.AddListener(UpdateCombo);
        }
    }

    void OnDisable()
    {
        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged.RemoveListener(UpdateScore);
            ScoreManager.Instance.OnComboChanged.RemoveListener(UpdateCombo);
        }
    }

    void Start()
    {
        UpdateScore(1, ScoreManager.Instance != null ? ScoreManager.Instance.Player1Score : 0);
        UpdateScore(2, ScoreManager.Instance != null ? ScoreManager.Instance.Player2Score : 0);
    }

    public void UpdateScore(int playerIndex, int score)
    {
        if(playerIndex == 1 && player1ScoreText != null)
            player1ScoreText.text = $"P1: {score:000000}";

        if(playerIndex == 2 && player2ScoreText != null)
            player2ScoreText.text = $"P2: {score:000000}";
    }

    public void UpdateCombo(int playerIndex, int combo)
    {
        TextMeshProUGUI comboText = playerIndex == 1
            ? player1ComboText
            : player2ComboText;
        
        if(comboText == null)
            return;

        comboText.transform.DOKill();
        
        if(combo <= 1)
        {
            comboText.gameObject.SetActive(false);
            comboText.transform.localScale = Vector3.one;
            return;
        }

        comboText.gameObject.SetActive(true);
        comboText.text = $"x{combo}";

        comboText.transform.localScale = Vector3.one * 1.4f;

        comboText.transform
            .DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack);
    }
}
