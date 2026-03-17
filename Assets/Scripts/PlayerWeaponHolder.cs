using UnityEngine;
using System.Collections;

public class PlayerWeaponHolder : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private PlayerAttackHitbox attackHitbox;
    [SerializeField] private AudioSource audioSource;

    [Header("Default Attack Damage")]
    [SerializeField] private int defaultQuickDamage = 10;
    [SerializeField] private int defaultHeavyDamage = 20;

    [Header("Default Weapon Effects")]
    [SerializeField] private bool defaultEnableKnockback = false;
    [SerializeField] private float defaultKnockbackForce = 0f;
    [SerializeField] private bool defaultEnableBleed = true;

    private GameObject equippedWeaponVisualInstance;
    private EquippedWeaponVisual equippedWeaponVisual;
    private AudioClip currentAttackSfx;
    private Coroutine weaponTimerRoutine;

    public bool HasWeapon => equippedWeaponVisualInstance != null;

    private void Awake()
    {
        if (visual == null)
        {
            Transform found = transform.Find("Visual");
            if (found != null)
                visual = found;
        }

        if (weaponAnchor == null && visual != null)
        {
            Transform foundAnchor = visual.Find("WeaponAnchor");
            if (foundAnchor != null)
                weaponAnchor = foundAnchor;
        }

        if (weaponAnchor == null)
        {
            weaponAnchor = visual != null ? visual : transform;
        }

        if (attackHitbox == null)
            attackHitbox = GetComponentInChildren<PlayerAttackHitbox>();

        if (audioSource == null)
            audioSource = GetComponentInChildren<AudioSource>();

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(defaultQuickDamage, defaultHeavyDamage);
            attackHitbox.SetWeaponEffects(defaultEnableKnockback, defaultKnockbackForce, defaultEnableBleed);
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
    }

    public void EquipWeapon(
        string weaponName,
        GameObject equippedVisualPrefab,
        float duration,
        int quickDamage,
        int heavyDamage,
        bool enableKnockback,
        float knockbackForce,
        bool enableBleed,
        AudioClip pickupSfx,
        AudioClip attackSfx
    )
    {
        ClearCurrentWeaponVisual();

        if (equippedVisualPrefab != null)
        {
            equippedWeaponVisualInstance = Instantiate(equippedVisualPrefab, weaponAnchor);
            equippedWeaponVisualInstance.transform.localPosition = Vector3.zero;

            equippedWeaponVisual = equippedWeaponVisualInstance.GetComponent<EquippedWeaponVisual>();
        }

        currentAttackSfx = attackSfx;

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(quickDamage, heavyDamage);
            attackHitbox.SetWeaponEffects(enableKnockback, knockbackForce, enableBleed);
        }

        if (pickupSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSfx);
        }

        if (weaponTimerRoutine != null)
            StopCoroutine(weaponTimerRoutine);

        weaponTimerRoutine = StartCoroutine(WeaponTimerRoutine(duration));
    }

    public void PlayAttackSfx()
    {
        if (currentAttackSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentAttackSfx);
        }
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

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(defaultQuickDamage, defaultHeavyDamage);
            attackHitbox.SetWeaponEffects(defaultEnableKnockback, defaultKnockbackForce, defaultEnableBleed);
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
}