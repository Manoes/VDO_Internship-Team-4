using TMPro;
using UnityEngine;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.6f;

    private float timer;
    private Color startColor;

    void Awake()
    {
        if(text == null)
            text = GetComponent<TextMeshProUGUI>();
        
        startColor = text.color;
    }

    public void Initialize(int amount)
    {
        timer = lifetime;
        text.text = amount > 0 ? $"+{amount}" : amount.ToString();
        text.color = startColor;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        float alpa = Mathf.Clamp01(timer / lifetime);
        text.color = new Color(startColor.r, startColor.g, startColor.g, alpa);

        if(timer <= 0f)
            Destroy(gameObject);
    }
}
