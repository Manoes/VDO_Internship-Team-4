using UnityEngine;
using TMPro; // Required for TextMeshPro input fields

public class SaveSystem : MonoBehaviour
{
    // Singleton instance
    public static SaveSystem instance { get; private set; }

    [Header("UI References")]
    [SerializeField]
    public TMP_InputField inputField; // The input field to interact with

    private void Awake()
    {
        // Implement the singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}