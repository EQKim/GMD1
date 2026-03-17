using UnityEngine;

public class EquippedWeaponVisual : MonoBehaviour
{
    [Header("Idle")]
    [SerializeField] private float idleZRotation = 60f;

    [Header("Quick Attack")]
    [SerializeField] private float quickSwingZRotation = -20f;
    [SerializeField] private float quickSwingDuration = 0.08f;
    [SerializeField] private float quickReturnDuration = 0.12f;

    [Header("Heavy Attack")]
    [SerializeField] private float heavySwingZRotation = -60f;
    [SerializeField] private float heavySwingDuration = 0.12f;
    [SerializeField] private float heavyReturnDuration = 0.18f;

    private Transform visualRoot;
    private bool facingRight = true;
    private Coroutine swingRoutine;

    private void Start()
    {
        visualRoot = GetComponentInParent<PlayerController2D>()?.transform.Find("Visual");

        ApplyIdleRotation();
    }

    private void Update()
    {
        if (visualRoot == null)
            return;

        bool newFacingRight = visualRoot.localScale.x > 0f;

        if (newFacingRight != facingRight)
        {
            facingRight = newFacingRight;
            ApplyIdleRotation();
        }
    }

    private void ApplyIdleRotation()
    {
        float z = facingRight ? idleZRotation : -idleZRotation;
        transform.localRotation = Quaternion.Euler(0f, 0f, z);
    }

    public void PlayQuickSwing()
    {
        StartSwing(quickSwingZRotation, quickSwingDuration, quickReturnDuration);
    }

    public void PlayHeavySwing()
    {
        StartSwing(heavySwingZRotation, heavySwingDuration, heavyReturnDuration);
    }

    public void ReturnToIdle()
    {
        ApplyIdleRotation();
    }

    private void StartSwing(float targetZ, float swingTime, float returnTime)
    {
        if (swingRoutine != null)
            StopCoroutine(swingRoutine);

        swingRoutine = StartCoroutine(SwingRoutine(targetZ, swingTime, returnTime));
    }

    private System.Collections.IEnumerator SwingRoutine(float targetZ, float swingTime, float returnTime)
    {
        float startZ = transform.localEulerAngles.z;
        float endZ = facingRight ? targetZ : -targetZ;

        float t = 0f;

        // swing
        while (t < swingTime)
        {
            t += Time.deltaTime;
            float z = Mathf.LerpAngle(startZ, endZ, t / swingTime);
            transform.localRotation = Quaternion.Euler(0f, 0f, z);
            yield return null;
        }

        t = 0f;

        float idleZ = facingRight ? idleZRotation : -idleZRotation;

        // return
        while (t < returnTime)
        {
            t += Time.deltaTime;
            float z = Mathf.LerpAngle(endZ, idleZ, t / returnTime);
            transform.localRotation = Quaternion.Euler(0f, 0f, z);
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, idleZ);
        swingRoutine = null;
    }
}