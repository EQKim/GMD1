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

    [Header("Ranged")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Ranged FX")]
    [SerializeField] private GameObject muzzleFlashPrefab;

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
    private Transform muzzlePoint;
    private Transform muzzleFlashPoint;
    private ParticleSystem smokeParticles;

    private AudioClip currentAttackSfx;
    private float currentAttackSfxVolume = 1f;
    private Coroutine weaponTimerRoutine;
    private Coroutine rangedAttackRoutine;
    private bool lastFacingRight = true;

    private float lastAttackSfxTime = -999f;
    private int lastAttackSfxFrame = -1;

    private bool currentWeaponIsRanged;
    private float currentWeaponFireRate = 0f;
    private int currentQuickDamage;
    private int currentHeavyDamage;

    public bool HasWeapon => equippedWeaponVisualInstance != null;
    public bool CurrentWeaponIsRanged => HasWeapon && currentWeaponIsRanged;
    public float CurrentWeaponFireRate => currentWeaponFireRate;

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

        currentQuickDamage = defaultQuickDamage;
        currentHeavyDamage = defaultHeavyDamage;

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
        bool isRangedWeapon,
        float fireRate,
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
        StopRangedAttack();
        StopCurrentWeaponAudio();
        ClearCurrentWeaponVisual();

        currentWeaponIsRanged = isRangedWeapon;
        currentWeaponFireRate = Mathf.Max(0.1f, fireRate);
        currentQuickDamage = quickDamage;
        currentHeavyDamage = heavyDamage;

        muzzlePoint = null;
        muzzleFlashPoint = null;
        smokeParticles = null;

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

            Transform foundMuzzle = equippedWeaponVisualInstance.transform.Find("Visual/MuzzlePoint");
            if (foundMuzzle != null)
            {
                muzzlePoint = foundMuzzle;
                muzzleFlashPoint = foundMuzzle.Find("MuzzleFlashPoint");

                Transform smokeParticlesTransform = foundMuzzle.Find("SmokeParticles");
                if (smokeParticlesTransform != null)
                    smokeParticles = smokeParticlesTransform.GetComponent<ParticleSystem>();
            }
        }

        currentAttackSfx = attackSfx;
        currentAttackSfxVolume = Mathf.Clamp01(attackSfxVolume);

        ConfigureAudioForCurrentWeapon();

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

    private void ConfigureAudioForCurrentWeapon()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.volume = 1f;

        if (currentWeaponIsRanged && currentAttackSfx != null)
        {
            audioSource.clip = currentAttackSfx;
            audioSource.loop = true;
            audioSource.volume = currentAttackSfxVolume;
        }
    }

    private void StopCurrentWeaponAudio()
    {
        if (audioSource == null)
            return;

        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }

    public void PlayAttackSfx()
    {
        if (currentWeaponIsRanged)
            return;

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

    public void StartRangedAttack()
    {
        if (!CurrentWeaponIsRanged)
            return;

        if (rangedAttackRoutine != null)
            return;

        StartRangedLoopAudio();
        StartBarrelSmoke();
        rangedAttackRoutine = StartCoroutine(RangedAttackRoutine());
    }

    public void StopRangedAttack()
    {
        if (rangedAttackRoutine != null)
        {
            StopCoroutine(rangedAttackRoutine);
            rangedAttackRoutine = null;
        }

        PauseRangedLoopAudio();
        StopBarrelSmoke();
        ReturnWeaponToIdle();
    }

    private void StartRangedLoopAudio()
    {
        if (audioSource == null || !currentWeaponIsRanged || currentAttackSfx == null)
            return;

        audioSource.clip = currentAttackSfx;
        audioSource.loop = true;
        audioSource.volume = currentAttackSfxVolume;

        if (audioSource.time > 0f)
            audioSource.UnPause();
        else if (!audioSource.isPlaying)
            audioSource.Play();
    }

    private void PauseRangedLoopAudio()
    {
        if (audioSource == null || !currentWeaponIsRanged)
            return;

        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    private IEnumerator RangedAttackRoutine()
    {
        float secondsPerShot = 1f / Mathf.Max(0.1f, currentWeaponFireRate);

        while (true)
        {
            FireRangedShot();
            yield return new WaitForSeconds(secondsPerShot);
        }
    }

    private void FireRangedShot()
    {
        PlayQuickWeaponSwing();
        SpawnMuzzleFlash();

        if (bulletPrefab == null)
        {
            Debug.LogWarning($"{name}: No bulletPrefab assigned on PlayerWeaponHolder.");
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogWarning($"{name}: No MuzzlePoint found on equipped weapon prefab.");
            return;
        }

        Vector2 direction = IsFacingRight() ? Vector2.right : Vector2.left;

        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzlePoint.position,
            Quaternion.identity
        );

        BulletProjectile projectile = bullet.GetComponent<BulletProjectile>();
        if (projectile != null)
        {
            PlayerHealth owner = GetComponent<PlayerHealth>();
            projectile.Initialize(direction, currentQuickDamage, owner);
        }
    }

    private void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzleFlashPoint == null)
            return;

        GameObject fx = Instantiate(
            muzzleFlashPrefab,
            muzzleFlashPoint.position,
            Quaternion.identity
        );

        Vector3 scale = fx.transform.localScale;
        scale.x = IsFacingRight() ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        fx.transform.localScale = scale;
    }

    private void StartBarrelSmoke()
    {
        if (smokeParticles == null)
            return;

        var emission = smokeParticles.emission;
        emission.enabled = true;

        if (!smokeParticles.isPlaying)
            smokeParticles.Play();
    }

    private void StopBarrelSmoke()
    {
        if (smokeParticles == null)
            return;

        var emission = smokeParticles.emission;
        emission.enabled = false;
    }

    public void RemoveWeapon()
    {
        if (weaponTimerRoutine != null)
        {
            StopCoroutine(weaponTimerRoutine);
            weaponTimerRoutine = null;
        }

        StopRangedAttack();
        StopCurrentWeaponAudio();
        ClearCurrentWeaponVisual();
        currentAttackSfx = null;
        ResetAttackSfxGate();

        currentWeaponIsRanged = false;
        currentWeaponFireRate = 0f;
        currentQuickDamage = defaultQuickDamage;
        currentHeavyDamage = defaultHeavyDamage;

        muzzlePoint = null;
        muzzleFlashPoint = null;
        smokeParticles = null;

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
        muzzlePoint = null;
        muzzleFlashPoint = null;
        smokeParticles = null;
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