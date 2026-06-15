using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonSelect : MonoBehaviour, ISelectHandler
{

    [SerializeField] AudioSource asS;
    [SerializeField] AudioClip[] selectSound;

    public void OnSelect(BaseEventData eventData)
    {
        asS.PlayOneShot(selectSound[Random.Range(0, selectSound.Length)]);

    }

    public void SetAudioSource(AudioSource audioSource)
    {
        asS = audioSource;
    }
}
