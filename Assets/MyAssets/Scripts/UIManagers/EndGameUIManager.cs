using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject soloDeathPanel;
    [SerializeField] private GameObject twoPlayerEndPanel;

    [Header("Default Selection")]
    [SerializeField] private Button restartButton;

    [Header("Animation")]
    [SerializeField] private RectTransform soloDeathPanelRoot;
    [SerializeField] private RectTransform twoPlayerEndPanelRoot;

    [SerializeField] private float popDuration = 0.35f;

    [Header("Title Animation")]
    [SerializeField] private RectTransform soloTitle;
    [SerializeField] private RectTransform winnerTitle;
    [SerializeField] private float titleBounceScale = 1.08f;
    [SerializeField] private float titleBounceDuration = 0.45f;

    [Header("Solo Death / Highscore")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI soloScoreText;

    [Header("Two Player End")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI player1FinalScoreText;
    [SerializeField] private TextMeshProUGUI player2FinalScoreText;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool submitted;
    private int finalSoloScore;

    private Tween soloTitleTween;
    private Tween winnerTitleTween;

    private void Awake()
    {
        if (soloDeathPanel != null)
            soloDeathPanel.SetActive(false);

        if (twoPlayerEndPanel != null)
            twoPlayerEndPanel.SetActive(false);
    }

    public void ShowSoloDeathScreen()
    {
        Time.timeScale = 0f;

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (soloDeathPanel != null)
        {
            soloDeathPanel.SetActive(true);
            AnimatePanel(soloDeathPanelRoot);
        }

        AnimateTitle(soloTitle, ref soloTitleTween);

        finalSoloScore = ScoreManager.Instance != null
            ? ScoreManager.Instance.Player1Score
            : 0;

        if (soloScoreText != null)
            soloScoreText.text = $"Score: {finalSoloScore:000000}";

        if (nameInput != null)
            nameInput.text = "";

        submitted = false;
    }

    public void SubmitSoloHighScore()
    {
        if (submitted) return;
        submitted = true;

        string playerName = nameInput != null ? nameInput.text : "Unknown";

        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Unknown";

        HighScoreSystem.HighScoreService.AddHighScore(
            playerName,
            finalSoloScore
        );

        MainMenu();
    }

    public void ShowTwoPlayerEndScreen()
    {
        Time.timeScale = 0f;

        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (twoPlayerEndPanel != null)
        {
            twoPlayerEndPanel.SetActive(true);
            AnimatePanel(twoPlayerEndPanelRoot);
        }

        AnimateTitle(winnerTitle, ref winnerTitleTween);

        StartCoroutine(SelectRestartButton());

        int p1Score = ScoreManager.Instance != null ? ScoreManager.Instance.Player1Score : 0;
        int p2Score = ScoreManager.Instance != null ? ScoreManager.Instance.Player2Score : 0;

        if (player1FinalScoreText != null)
            player1FinalScoreText.text = $"P1: {p1Score:000000}";

        if (player2FinalScoreText != null)
            player2FinalScoreText.text = $"P2: {p2Score:000000}";

        if (winnerText != null)
        {
            if (p1Score > p2Score)
                winnerText.text = "PLAYER 1 WINS!";
            else if (p2Score > p1Score)
                winnerText.text = "PLAYER 2 WINS!";
            else
                winnerText.text = "DRAW!";
        }
    }

    private IEnumerator SelectRestartButton()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);

        if (restartButton != null)
        {
            restartButton.Select();
            EventSystem.current.SetSelectedGameObject(restartButton.gameObject);
        }
    }

    private void AnimatePanel(RectTransform panelRoot)
    {
        if (panelRoot == null)
            return;

        panelRoot.DOKill();

        panelRoot.localScale = Vector3.zero;

        panelRoot
            .DOScale(Vector3.one, popDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    private void AnimateTitle(RectTransform title, ref Tween tween)
    {
        if (title == null)
            return;

        tween?.Kill();
        title.DOKill();

        title.localScale = Vector3.one;

        tween = title
            .DOScale(Vector3.one * titleBounceScale, titleBounceDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    private void SelectButton(Button button)
    {
        if (button == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);

        button.Select();
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}