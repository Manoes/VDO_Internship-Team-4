using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject highScoresPanel;

    [Header("Highscores")]
    [SerializeField] private TextMeshProUGUI[] highScoreTexts;
    [SerializeField] private string emptyHighScoreText = "--- 000000";

    [Header("Default Buttons")]
    [SerializeField] private Button soloButton;
    [SerializeField] private Button highScoresBackButton;

    [Header("Title Animation")]
    [SerializeField] private RectTransform title;
    [SerializeField] private float titleBounceScale = 1.08f;
    [SerializeField] private float titleBounceDuration = 0.45f;

    [Header("Scenes")]
    [SerializeField] private string mainGameSceneName = "MainGame";

    private Tween titleTween;

    private void Start()
    {
        LockCursor();

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (highScoresPanel != null)
            highScoresPanel.SetActive(false);

        AnimateTitle();
        StartCoroutine(SelectNextFrame(soloButton));
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void AnimateTitle()
    {
        if (title == null) return;

        titleTween?.Kill();
        title.DOKill();

        title.localScale = Vector3.one;

        titleTween = title
            .DOScale(Vector3.one * titleBounceScale, titleBounceDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
    }

    public void PlaySolo()
    {
        Time.timeScale = 1f;
        ModeManager.SetMode(GameMode.Solo);
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void PlayVersus()
    {
        Time.timeScale = 1f;
        ModeManager.SetMode(GameMode.TwoPlayer);
        SceneManager.LoadScene(mainGameSceneName);
    }

    public void OpenHighScores()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (highScoresPanel != null)
            highScoresPanel.SetActive(true);

        RefreshHighScores();

        StartCoroutine(SelectNextFrame(highScoresBackButton));
    }

    private void RefreshHighScores()
    {
        HighScoreSystem.Reload();

        IReadOnlyList<HighScoreEntry> entries =
            HighScoreSystem.HighScoreService.GetTop();

        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if (highScoreTexts[i] == null)
                continue;

            if (i < entries.Count)
            {
                highScoreTexts[i].text =
                    $"{i + 1}. {entries[i].name} - {entries[i].score:000000}";
            }
            else
            {
                highScoreTexts[i].text =
                    $"{i + 1}. {emptyHighScoreText}";
            }
        }
    }

    public void CloseHighScores()
    {
        if (highScoresPanel != null)
            highScoresPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        StartCoroutine(SelectNextFrame(soloButton));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator SelectNextFrame(Button button)
    {
        yield return null;

        if (button == null) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        button.Select();
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}