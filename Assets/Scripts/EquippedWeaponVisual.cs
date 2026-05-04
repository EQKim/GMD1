using UnityEngine;
using System.Collections;

public class EquippedWeaponVisual : MonoBehaviour
{
    [Header("Swing")]
    [SerializeField] private float swingZDelta = 20f;
    [SerializeField] private float swingDuration = 0.08f;
    [SerializeField] private float returnDuration = 0.12f;

    private Quaternion idleLocalRotation;
    private Coroutine swingRoutine;

    private void OnEnable()
    {
        RefreshIdlePose();
    }

    public void RefreshIdlePose()
    {
        idleLocalRotation = transform.localRotation;
    }

    public void PlaySwing()
    {
        StartSwing(swingZDelta, swingDuration, returnDuration);
    }

    public void ReturnToIdle()
    {
        if (swingRoutine != null)
        {
            StopCoroutine(swingRoutine);
            swingRoutine = null;
        }

        transform.localRotation = idleLocalRotation;
    }

    private void StartSwing(float zDelta, float swingTime, float returnTime)
    {
        if (swingRoutine != null)
            StopCoroutine(swingRoutine);

        float signedDelta = GetFacingAdjustedDelta(zDelta);
        swingRoutine = StartCoroutine(SwingRoutine(signedDelta, swingTime, returnTime));
    }

    private float GetFacingAdjustedDelta(float zDelta)
    {
        return transform.localScale.x >= 0f ? zDelta : -zDelta;
    }

    private IEnumerator SwingRoutine(float zDelta, float swingTime, float returnTime)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = idleLocalRotation * Quaternion.Euler(0f, 0f, zDelta);

        float t = 0f;

        while (t < swingTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t / swingTime);
            yield return null;
        }

        t = 0f;

        while (t < returnTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, idleLocalRotation, t / returnTime);
            yield return null;
        }

        transform.localRotation = idleLocalRotation;
        swingRoutine = null;
    }
}