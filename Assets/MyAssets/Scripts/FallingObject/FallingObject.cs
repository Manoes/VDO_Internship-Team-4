using UnityEngine;

public enum FallingObjectType
{
    Banana,
    GoldenBanana,
    Coconut
}

public class FallingObject : MonoBehaviour
{
    [Header("FallingObject Data")]
    [SerializeField] private FallingObjectType type;
    [SerializeField] private int scoreAmount = 10;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float fallSpeed = 4f;
    [SerializeField] private float destroyBelowY = -8f;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] randomSprites;

    [Header("Rotation")]
    [SerializeField] private float minRotationSpeed = 45f;
    [SerializeField] private float maxRotationSpeed = 120f;

    [Header("Sway")]
    [SerializeField] private float swayAmplitude = 0.4f;
    [SerializeField] private float swaySpeed = 2f;

    private float swayOffset;
    private Vector3 startPosition;

    private float rotationSpeed;

    void Awake()
    {
        AssignRandomSprite();

        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        swayOffset = Random.Range(0f, Mathf.PI * 2f);

        if (Random.value > 0.5f)
            rotationSpeed *= -1f;
    }

    void AssignRandomSprite()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null) return;

        if (randomSprites == null || randomSprites.Length == 0) return;

        spriteRenderer.sprite = randomSprites[Random.Range(0, randomSprites.Length)];
    }

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (!float.IsNaN(rotationSpeed))
        {
            transform.Rotate(
                0f,
                0f,
                rotationSpeed * Time.deltaTime
            );
        }

        if (transform.position.y < destroyBelowY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.TryGetComponent(out MonkeyPlayerController player))
        return;

    int playerIndex = player.PlayerIndex;

    if (type == FallingObjectType.Banana || type == FallingObjectType.GoldenBanana)
    {
        AudioManager.Instance?.PlayBananaCollect();
        ScoreManager.Instance?.AddScore(playerIndex, scoreAmount, transform.position);
    }
    else if (type == FallingObjectType.Coconut)
    {
        AudioManager.Instance?.PlayCoconutHit();

        if (ModeManager.Instance != null && ModeManager.Instance.IsSolo)
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            health?.TakeDamage(damageAmount);
        }
        else
        {
            ScoreManager.Instance?.AddScore(playerIndex, -scoreAmount, transform.position);
        }
    }

    Destroy(gameObject);
}
}
