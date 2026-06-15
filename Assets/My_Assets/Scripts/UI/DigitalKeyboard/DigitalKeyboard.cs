using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DigitalKeyboard : MonoBehaviour
{
  [Header("Keys")]
  [SerializeField]
  private string[] keys =
  {
        "DEL", "SPACE", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
        "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
         "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"
    };

  [Header("Default Selection")]
  [SerializeField] private bool selectDeleteByDefault = true;

  [Header("References")]
  [SerializeField] private GameObject buttonPrefab;
  [SerializeField] private TMP_InputField nameInput;

  [Header("Audio")]
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip soundEffect;
  [SerializeField] private float buttonSpawnDelay = 0.03f;

  [Header("Name Settings")]
  [SerializeField] private int maxNameLength = 12;

  private float previousPitch = 1f;

  private Button deleteButton;

  private void Start()
  {
    StartCoroutine(SpawnButtons());
  }

  private IEnumerator SpawnButtons()
  {
    if (buttonPrefab == null)
    {
      Debug.LogError("[DigitalKeyboard] Button Prefab is not assigned.");
      yield break;
    }

    yield return new WaitForSecondsRealtime(buttonSpawnDelay);

    previousPitch = 1f;

    for (int i = 0; i < keys.Length; i++)
    {
      string key = keys[i];

      GameObject newButton = Instantiate(buttonPrefab, transform);
      newButton.name = key;

      TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
      if (text != null)
        text.SetText(key == "SPACE" ? "_" : key);

      if (newButton.TryGetComponent<Button>(out Button button))
      {
        button.onClick.RemoveAllListeners();

        string capturedKey = key;

        button.onClick.AddListener(() =>
        {
          HandleKeyPress(capturedKey);
        });

        if (key == "DEL")
          deleteButton = button;
      }

      if (newButton.TryGetComponent<ButtonSelect>(out var select))
        select.SetAudioSource(audioSource);

      if (newButton.TryGetComponent<RectTransform>(out RectTransform rt))
      {
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
      }

      previousPitch = Mathf.Clamp(previousPitch + 0.115f, -3f, 3f);

      if (audioSource != null && soundEffect != null && audioSource.isActiveAndEnabled)
        PlayRandomSound(previousPitch);

      yield return new WaitForSecondsRealtime(0.01f);

    }

    if (selectDeleteByDefault)
      StartCoroutine(SelectDeleteButton());
  }

  private IEnumerator SelectDeleteButton()
  {
    yield return null;

    if (deleteButton == null)
      yield break;

    EventSystem.current.SetSelectedGameObject(null);
    deleteButton.Select();
    EventSystem.current.SetSelectedGameObject(deleteButton.gameObject);
  }

  private void HandleKeyPress(string key)
  {
    if (nameInput == null)
      return;

    if (key == "DEL")
    {
      DeleteCharacter();
      return;
    }

    if (key == "SPACE")
    {
      AddCharacter(" ");
      return;
    }

    AddCharacter(key);
  }

  private void AddCharacter(string character)
  {
    if (nameInput == null)
      return;

    if (nameInput.text.Length >= maxNameLength)
      return;

    nameInput.text += character;
  }

  private void DeleteCharacter()
  {
    if (nameInput == null)
      return;

    if (string.IsNullOrEmpty(nameInput.text))
      return;

    nameInput.text = nameInput.text[..^1];
  }

  private void PlayRandomSound(float pitch)
  {
    if (audioSource == null || soundEffect == null)
      return;

    audioSource.pitch = pitch;
    audioSource.PlayOneShot(soundEffect);
  }
}