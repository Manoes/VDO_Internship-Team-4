using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonZoom : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Vector3 originalSize;
    Vector3 originalRotation;
    [SerializeField] float duration = 0.2f;
    [SerializeField] bool customSpin = false;
    [SerializeField] float spinRange = 10f;
    [SerializeField] float zoom = 1.25f;

    void Start()
    {
        originalSize = transform.localScale;
        originalRotation = transform.localRotation.eulerAngles;
    }
    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(originalSize * zoom, duration);
        float randomRotation = customSpin ? Random.Range(-spinRange, spinRange) : Random.Range(-6f, 6f);
        transform.DORotate(originalRotation + new Vector3(0, 0, randomRotation), duration);



    }
    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(originalSize, duration);
        transform.DORotate(originalRotation, duration);
    }

    void OnDisable()
    {
        transform.DOScale(originalSize, duration);
        transform.DORotate(originalRotation, duration);
    }



}
