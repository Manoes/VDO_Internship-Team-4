using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class DigitalKeyboard : MonoBehaviour
{
  [SerializeField]
  private string[] keys =
   {
        "DEL", "SPACE",
        "A","B","C","D","E","F","G","H","I","J","K","L","M",
        "N","O","P","Q","R","S","T","U","V","W","X","Y","Z",

        "a","b","c","d","e","f","g","h","i","j","k","l","m",
        "n","o","p","q","r","s","t","u","v","w","x","y","z"
    };
  [SerializeField] private GameObject buttonPrefab;
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip soundEffect;
  [SerializeField] private float buttonSpawnDelay = 0.03f;

  private float previousPitch = 1;

  private Button deleteButton;

  private void Start()
  {
    StartCoroutine(SpawnButtons());
  }

  private IEnumerator SpawnButtons()
  {
    if (buttonPrefab == null)
    {
      Debug.LogError("[DigitalKeyboard] Button Prefab is not assigned!");
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
        if (key == "DEL")
          deleteButton = button;

        button.onClick.RemoveAllListeners();

        string capturedLetter = key;

        if (capturedLetter == "DEL")
        {
          button.onClick.AddListener(() =>
          {
            EndGameUIManager.Instance.DeleteCharacter();
          });
        }
        else if (capturedLetter == "SPACE")
        {
          button.onClick.AddListener(() =>
          {
            EndGameUIManager.Instance.AddSpace();
          });
        }
        else
        {
          button.onClick.AddListener(() =>
          {
            EndGameUIManager.Instance.AddCharacter(capturedLetter);
          });
        }
      }

      if (newButton.TryGetComponent<ButtonSelect>(out var select))
        select.SetAudioSource(audioSource);

      if (newButton.TryGetComponent<RectTransform>(out var rt))
      {
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;
      }

      previousPitch = Mathf.Clamp(previousPitch + 0.115f, -3f, 3f);

      if (audioSource != null && soundEffect != null && audioSource.isActiveAndEnabled)
        PlayRandomSound(previousPitch);

      yield return new WaitForSecondsRealtime(0.01f);
    }

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

  private void PlayRandomSound(float pitch)
  {
    audioSource.pitch = pitch;
    audioSource.PlayOneShot(soundEffect);
  }
}