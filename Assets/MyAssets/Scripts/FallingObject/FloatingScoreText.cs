using TMPro;
using UnityEngine;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.6f;

    [Header("Colors")]
    [SerializeField] private Color positiveColor = new Color(1f, 0.85f, 0f); // Banana Yellow
    [SerializeField] private Color negativeColor = Color.red;

    private float timer;
    private Color currentColor;

    void Awake()
    {
        if(text == null)
            text = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int amount)
    {
        timer = lifetime;

        text.text = amount > 0 
            ? $"+{amount}" 
            : amount.ToString();

        currentColor = amount > 0
            ? positiveColor
            : negativeColor;

        text.color = currentColor;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer -= Time.deltaTime;

        float alpa = Mathf.Clamp01(timer / lifetime);
        text.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpa);

        if(timer <= 0f)
            Destroy(gameObject);
    }
}
