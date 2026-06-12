using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MatchTimer : MonoBehaviour
{
    [SerializeField] private float matchDuration = 90f;
    [SerializeField] private TextMeshProUGUI timerText;

    public UnityEvent OnTimerFinished;

    private float timer;
    private bool running;

    void Start()
    {
        if(ModeManager.Instance != null && ModeManager.Instance.IsTwoPlayer)
            StartTimer();
        else
            gameObject.SetActive(false);
    }

    void Update()
    {
        if(!running) return;

        timer -= Time.deltaTime;

        if(timer <= 0f)
        {
            timer = 0f;
            running = false;
            OnTimerFinished?.Invoke();
        }

        UpdateUI();
    }

    public void StartTimer()
    {
        timer = matchDuration;
        running = true;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(timerText == null) return;

        int seconds = Mathf.CeilToInt(timer);
        timerText.text = seconds.ToString("00");
    }
}
