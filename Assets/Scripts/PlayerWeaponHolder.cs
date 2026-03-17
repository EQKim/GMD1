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

    private GameObject equippedWeaponVisualInstance;
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
        AudioClip pickupSfx,
        AudioClip attackSfx
    )
    {
        ClearCurrentWeaponVisual();

        if (equippedVisualPrefab != null)
        {
            equippedWeaponVisualInstance = Instantiate(equippedVisualPrefab, weaponAnchor);
            equippedWeaponVisualInstance.transform.localPosition = Vector3.zero;
            equippedWeaponVisualInstance.transform.localRotation = Quaternion.identity;
        }

        currentAttackSfx = attackSfx;

        if (attackHitbox != null)
        {
            attackHitbox.SetDamageValues(quickDamage, heavyDamage);
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
    }
}