using UnityEngine;

public enum FallingObjectType
{
    Banana,
    Coconut
}

public class FallingObject : MonoBehaviour
{
    [SerializeField] private FallingObjectType type;
    [SerializeField] private int scoreAmount = 10;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float fallSpeed = 4f;
    [SerializeField] private float destroyBelowY = -8f;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if(transform.position.y < destroyBelowY)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        MonkeyPlayerController player = collision.GetComponent<MonkeyPlayerController>();
        if(player == null) return;

        int playerIndex = player.PlayerIndex;

        if(type == FallingObjectType.Banana)
        {
            ScoreManager.Instance?.AddScore(playerIndex, scoreAmount);
        }
        else if(type == FallingObjectType.Coconut)
        {
            if(ModeManager.Instance != null && ModeManager.Instance.IsSolo)
            {
                PlayerHealth health = collision.GetComponent<PlayerHealth>();
                health?.TakeDamage(damageAmount);
            }
            else
            {
                ScoreManager.Instance?.AddScore(playerIndex, -scoreAmount);
            }
        }

        Destroy(gameObject);
    }
}
