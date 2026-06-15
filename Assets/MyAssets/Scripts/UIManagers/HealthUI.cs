using DG.Tweening;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] heartIcons;

    [Header("Animations")]
    [SerializeField] private float popScale = 1.4f;
    [SerializeField] private float popDuration = 0.12f;

    private int previousHealth = -1;

    void OnEnable()
    {
        if(playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if(playerHealth == null) return;

        playerHealth.OnHealthChanged.AddListener(UpdateHealthUI);
        UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    void OnDisable()
    {
        if(playerHealth == null)
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
    }

    private void UpdateHealthUI(int CurrentHealth, int MaxHealth)
    {
        bool tookDamage = previousHealth != -1 && CurrentHealth < previousHealth;

        for (int i = 0; i < heartIcons.Length; i++)
        {
            if(heartIcons[i] == null) continue;

            if(i < CurrentHealth)
            {
                heartIcons[i].SetActive(true);
                heartIcons[i].transform.DOKill();
                heartIcons[i].transform.localScale = Vector3.one;
            }
            else
            {
                if(tookDamage && i >= CurrentHealth && i < previousHealth)
                    PopLostHeart(heartIcons[i]);
                else
                    heartIcons[i].SetActive(false);
            }
        }

        previousHealth = CurrentHealth;
    }

    private void PopLostHeart(GameObject heart)
    {
        heart.SetActive(true);

        Transform t = heart.transform;
        t.DOKill();
        t.localScale = Vector3.one;

        t.DOScale(popScale, popDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                t.DOScale(0f, popDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        heart.SetActive(false);
                        t.localScale = Vector3.one;
                    });
            });
    }
}
