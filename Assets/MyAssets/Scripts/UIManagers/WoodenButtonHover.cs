using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class WoodenButtonHover : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private RectTransform target;

    [Header("Rock")]
    [SerializeField] private float rockAngle = 4f;
    [SerializeField] private float rockDuration = 0.18f;

    [Header("Pop")]
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float scaleDuration = 0.15f;

    private Vector3 startScale;
    private Quaternion startRotation;
    private Sequence rockSequence;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        startScale = target.localScale;
        startRotation = target.localRotation;
    }

    public void OnSelect(BaseEventData eventData)
    {
        target.DOKill();

        target
            .DOScale(startScale * selectedScale, scaleDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        rockSequence = DOTween.Sequence();
        rockSequence.SetUpdate(true);

        rockSequence.Append(target.DOLocalRotate(new Vector3(0f, 0f, rockAngle), rockDuration));
        rockSequence.Append(target.DOLocalRotate(new Vector3(0f, 0f, -rockAngle), rockDuration));
        rockSequence.SetLoops(-1, LoopType.Yoyo);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rockSequence?.Kill();
        target.DOKill();

        target
            .DOScale(startScale, scaleDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        target
            .DOLocalRotateQuaternion(startRotation, scaleDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }
}