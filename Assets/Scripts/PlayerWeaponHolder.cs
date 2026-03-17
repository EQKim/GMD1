using UnityEngine;
using System.Collections;

public class PlayerWeaponHolder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform weaponAnchorRight;
    [SerializeField] private Transform weaponAnchorLeft;
    [SerializeField] private PlayerAttackHitbox attackHitbox;
    [SerializeField] private AudioSource audioSource;

    [Header("Default Attack Damage")]
    [SerializeField] private int defaultQuickDamage = 10;
    [SerializeField] private int defaultHeavyDamage = 20;

    [Header("Default Weapon Effects")]
    [Tooltip("If enabled, this component will override the hitbox's knockback/bleed values on start and when the weapon expires.")]
    [SerializeField] private bool overrideHitboxDefaults = false;

    [SerializeField] private float defaultKnockbackForce = 0f;
    [SerializeField] private bool defaultEnableBleed = true;

    [Header("Hit SFX Protection")]
    [Tooltip("Prevents the same hit sound from being triggered multiple times too quickly.")]
    [SerializeField] private float minAttackSfxInterval = 0.08f;

    private GameObject equippedWeaponVisualInstance;
    private EquippedWeaponVisual equippedWeaponVisual;
    private AudioClip currentAttackSfx;
    private float currentAttackSfxVolume = 1f;
    private Coroutine weaponTimerRoutine;
    private bool lastFacingRight = true;

    private float lastAttackSfxTime = -999f;
    private int lastAttackSfxFrame = -1;

    public bool HasWeapon => equippedWeaponVisualInstance != null;

    private void Awake()
    {
        if (visual == null)
        {
            Transform found = transform.Find("Visual");
            if (found != null)
                visual = found;
        }

        if (weaponAnchorRight == null && visual != null)
        {
            Transform foundRight = visual.Find("WeaponAnchorRight");
            if (foundRight != null)
                weaponAnchorRight = foundRight;
        }

        if (weaponAnchorLeft == null && visual != null)
        {
            Transform foundLeft = visual.Find("WeaponAnchorLeft");
            if (foundLeft != null)
                weaponAnchorLeft = foundLeft;
        }

        if (attackHitbox == null)
            attackHitbox = GetComponentInChildren<PlayerAttackHitbox>();

        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(defaultQuickDamage, defaultHeavyDamage);

            if (overrideHitboxDefaults)
                attackHitbox.SetWeaponEffects(defaultKnockbackForce, defaultEnableBleed);
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
        }
        else
        {
            Debug.LogWarning($"{name}: No AudioSource assigned or found for PlayerWeaponHolder.");
        }

        lastFacingRight = IsFacingRight();
    }

    private void Update()
    {
        bool facingRight = IsFacingRight();

        if (facingRight != lastFacingRight)
        {
            lastFacingRight = facingRight;
            RefreshWeaponParent();
        }
    }

    public void EquipWeapon(
        string weaponName,
        GameObject equippedVisualPrefab,
        float duration,
        int quickDamage,
        int heavyDamage,
        float knockbackForce,
        bool enableBleed,
        AudioClip pickupSfx,
        float pickupSfxVolume,
        AudioClip attackSfx,
        float attackSfxVolume
    )
    {
        ClearCurrentWeaponVisual();

        if (equippedVisualPrefab != null)
        {
            Transform targetAnchor = GetCurrentWeaponAnchor();

            equippedWeaponVisualInstance = Instantiate(
                equippedVisualPrefab,
                targetAnchor != null ? targetAnchor : transform
            );

            equippedWeaponVisualInstance.transform.localPosition = Vector3.zero;
            equippedWeaponVisualInstance.transform.localRotation = Quaternion.identity;

            ApplyFacingScale(equippedWeaponVisualInstance.transform);

            equippedWeaponVisual = equippedWeaponVisualInstance.GetComponent<EquippedWeaponVisual>();

            if (equippedWeaponVisual != null)
                equippedWeaponVisual.RefreshIdlePose();
        }

        currentAttackSfx = attackSfx;
        currentAttackSfxVolume = Mathf.Clamp01(attackSfxVolume);

        if (pickupSfx != null && audioSource != null)
            audioSource.PlayOneShot(pickupSfx, Mathf.Clamp01(pickupSfxVolume));

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(quickDamage, heavyDamage);
            attackHitbox.SetWeaponEffects(knockbackForce, enableBleed);
        }

        if (weaponTimerRoutine != null)
            StopCoroutine(weaponTimerRoutine);

        weaponTimerRoutine = StartCoroutine(WeaponTimerRoutine(duration));
    }

    public void PlayAttackSfx()
    {
        if (currentAttackSfx == null || audioSource == null)
            return;

        if (Time.frameCount == lastAttackSfxFrame)
            return;

        if (Time.time - lastAttackSfxTime < minAttackSfxInterval)
            return;

        lastAttackSfxFrame = Time.frameCount;
        lastAttackSfxTime = Time.time;

        audioSource.PlayOneShot(currentAttackSfx, currentAttackSfxVolume);
    }

    public void ResetAttackSfxGate()
    {
        lastAttackSfxFrame = -1;
        lastAttackSfxTime = -999f;
    }

    public void PlayQuickWeaponSwing()
    {
        if (equippedWeaponVisual != null)
            equippedWeaponVisual.PlayQuickSwing();
    }

    public void PlayHeavyWeaponSwing()
    {
        if (equippedWeaponVisual != null)
            equippedWeaponVisual.PlayHeavySwing();
    }

    public void ReturnWeaponToIdle()
    {
        if (equippedWeaponVisual != null)
            equippedWeaponVisual.ReturnToIdle();
    }

    public void RemoveWeapon()
    {
        if (weaponTimerRoutine != null)
        {
            StopCoroutine(weaponTimerRoutine);
            weaponTimerRoutine = null;
        }

        ClearCurrentWeaponVisual();
        currentAttackSfx = null;
        ResetAttackSfxGate();

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(defaultQuickDamage, defaultHeavyDamage);

            if (overrideHitboxDefaults)
                attackHitbox.SetWeaponEffects(defaultKnockbackForce, defaultEnableBleed);
        }
    }

    private IEnumerator WeaponTimerRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        weaponTimerRoutine = null;
        RemoveWeapon();
    }

    private void ClearCurrentWeaponVisual()
    {
        if (equippedWeaponVisualInstance != null)
        {
            Destroy(equippedWeaponVisualInstance);
            equippedWeaponVisualInstance = null;
        }

        equippedWeaponVisual = null;
    }

    private bool IsFacingRight()
    {
        if (visual == null)
            return true;

        return visual.localScale.x > 0f;
    }

    private Transform GetCurrentWeaponAnchor()
    {
        if (IsFacingRight())
            return weaponAnchorRight != null ? weaponAnchorRight : transform;

        return weaponAnchorLeft != null ? weaponAnchorLeft : transform;
    }

    private void RefreshWeaponParent()
    {
        if (equippedWeaponVisualInstance == null)
            return;

        Transform targetAnchor = GetCurrentWeaponAnchor();
        if (targetAnchor == null)
            return;

        equippedWeaponVisualInstance.transform.SetParent(targetAnchor, false);
        equippedWeaponVisualInstance.transform.localPosition = Vector3.zero;
        equippedWeaponVisualInstance.transform.localRotation = Quaternion.identity;

        ApplyFacingScale(equippedWeaponVisualInstance.transform);

        if (equippedWeaponVisual != null)
            equippedWeaponVisual.RefreshIdlePose();
    }

    private void ApplyFacingScale(Transform weaponTransform)
    {
        Vector3 scale = weaponTransform.localScale;
        scale.x = IsFacingRight() ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        weaponTransform.localScale = scale;
    }
}