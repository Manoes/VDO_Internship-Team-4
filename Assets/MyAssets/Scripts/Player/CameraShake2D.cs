using System.Collections;
using UnityEngine;

public class CameraShake2D : Singleton<CameraShake2D>
{
    [SerializeField] private float defaultIntensity = 0.12f;
    [SerializeField] private float defaultDuration = 0.15f;

    private Vector3 startPosition;
    private Coroutine shakeRoutine;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.localPosition;
    }

    public void Shake()
    {
        Shake(defaultIntensity, defaultDuration);
    }

    public void Shake(float intensity, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(intensity, duration));
    }

    private IEnumerator ShakeRoutine(float intensity, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            transform.localPosition = startPosition + (Vector3)Random.insideUnitCircle * intensity;

            yield return null;
        }

        transform.localPosition = startPosition;
    }
}