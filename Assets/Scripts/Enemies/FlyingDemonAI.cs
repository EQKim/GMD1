using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class FlyingDemonAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float targetHeightOffset = 1.25f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float preferredRange = 4.5f;
    [SerializeField] private float minRange = 3f;
    [SerializeField] private float maxRange = 6f;

    [Header("Hover")]
    [SerializeField] private float hoverAmplitude = 0.3f;
    [SerializeField] private float hoverFrequency = 2f;

    [Header("Attack")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float attackWindup = 0.25f;
    [SerializeField] private float attackRecovery = 0.2f;
    [SerializeField] private float shootRange = 7f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 40;

    [Header("Hit Feedback")]
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private float damageFlashDuration = 0.1f;
    [SerializeField] private GameObject bloodParticlesPrefab;
    [SerializeField] private Vector3 bloodSpawnOffset = Vector3.zero;

    [Header("Audio")]
    [SerializeField] private AudioClip fireballShootSfx;
    [Range(0f, 1f)]
    [SerializeField] private float fireballShootVolume = 1f;

    [SerializeField] private AudioClip hurtSfx;
    [Range(0f, 1f)]
    [SerializeField] private float hurtVolume = 1f;

    [Header("Death")]
    [SerializeField] private float destroyDelayAfterDeath = 1.2f;

    [Header("Ride Response")]
    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private Vector2 riderCheckBoxSize = new Vector2(1.2f, 0.35f);
    [SerializeField] private Vector2 riderCheckOffset = new Vector2(0f, 0.95f);
    [SerializeField] private float riderPushDownSpeed = 3.5f;
    [SerializeField] private bool blockUpwardMovementWhenRidden = true;

    [Header("Arena Clamp")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -3.5f;
    [SerializeField] private float maxY = 4.5f;

    private int currentHealth;
    private bool isDead;
    private bool isAttacking;
    private float attackCooldownTimer;
    private float hoverSeed;

    private PlayerHealth targetHealth;
    private FlyingDemonSpawner ownerSpawner;
    private Rigidbody2D rb;
    private Collider2D[] allColliders;
    private Color originalColor;
    private Coroutine flashRoutine;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (visual == null)
            visual = transform;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();
        allColliders = GetComponentsInChildren<Collider2D>();

        currentHealth = maxHealth;
        hoverSeed = Random.Range(0f, 100f);

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (!HasValidTarget())
            return;

        attackCooldownTimer -= Time.deltaTime;

        HandleFacing();
        HandleFirePointAim();
        HandleMovement();
        HandleAttack();
    }

    public void Initialize(Transform assignedTarget, FlyingDemonSpawner spawner)
    {
        target = assignedTarget;
        ownerSpawner = spawner;

        if (target != null)
            targetHealth = target.GetComponent<PlayerHealth>();

        if (targetHealth == null && target != null)
            targetHealth = target.GetComponentInParent<PlayerHealth>();
    }

    private bool HasValidTarget()
    {
        if (target == null)
            return false;

        if (!target.gameObject.activeInHierarchy)
            return false;

        if (targetHealth == null)
            targetHealth = target.GetComponent<PlayerHealth>();

        if (targetHealth == null && target != null)
            targetHealth = target.GetComponentInParent<PlayerHealth>();

        if (targetHealth == null)
            return false;

        if (targetHealth.CurrentLives <= 0)
            return false;

        return true;
    }

    private bool HasPlayerRider()
    {
        Vector2 checkCenter = (Vector2)transform.position + riderCheckOffset;
        Collider2D[] hits = Physics2D.OverlapBoxAll(checkCenter, riderCheckBoxSize, 0f, playerLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];

            if (hit == null)
                continue;

            // Ignore this demon's own colliders
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                continue;

            // Only count actual players
            PlayerController2D player = hit.GetComponentInParent<PlayerController2D>();
            if (player != null)
                return true;
        }
        return false;
    }

    private Vector3 GetTargetPoint()
    {
        float hoverOffset = Mathf.Sin((Time.time + hoverSeed) * hoverFrequency) * hoverAmplitude;

        Vector3 point = target.position;
        point.y += targetHeightOffset + hoverOffset;

        return point;
    }

    private void HandleMovement()
    {
        if (isAttacking)
            return;

        Vector3 targetPoint = GetTargetPoint();
        Vector3 toTarget = targetPoint - transform.position;
        float distance = toTarget.magnitude;

        Vector3 movement = Vector3.zero;

        if (distance > maxRange)
        {
            movement = toTarget.normalized * moveSpeed * Time.deltaTime;
        }
        else if (distance < minRange)
        {
            movement = -toTarget.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            Vector3 desiredPosition = targetPoint - toTarget.normalized * preferredRange;
            Vector3 moveDir = desiredPosition - transform.position;

            if (moveDir.magnitude > 0.15f)
                movement = moveDir.normalized * moveSpeed * Time.deltaTime;
        }

        if (HasPlayerRider())
        {
            if (blockUpwardMovementWhenRidden && movement.y > 0f)
                movement.y = 0f;

            movement.y -= riderPushDownSpeed * Time.deltaTime;
        }

        transform.position += movement;

        Vector3 clamped = transform.position;
        clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        clamped.y = Mathf.Clamp(clamped.y, minY, maxY);
        transform.position = clamped;
    }

    private void HandleFacing()
    {
        if (visual == null || target == null)
            return;

        Vector3 scale = visual.localScale;

        if (target.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        visual.localScale = scale;
    }

    private void HandleFirePointAim()
    {
        if (firePoint == null || target == null)
            return;

        Vector3 direction = target.position - firePoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void HandleAttack()
    {
        if (isAttacking || attackCooldownTimer > 0f)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= shootRange)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        attackCooldownTimer = attackCooldown;

        animator.SetTrigger(AttackHash);

        yield return new WaitForSeconds(attackWindup);

        ShootFireball();

        yield return new WaitForSeconds(attackRecovery);

        isAttacking = false;
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || target == null)
            return;

        GameObject fireballObject = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);

        Collider2D fireballCollider = fireballObject.GetComponent<Collider2D>();
        if (fireballCollider == null)
            fireballCollider = fireballObject.GetComponentInChildren<Collider2D>();

        Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();

        if (fireballCollider != null && myColliders != null)
        {
            for (int i = 0; i < myColliders.Length; i++)
            {
                if (myColliders[i] != null)
                    Physics2D.IgnoreCollision(fireballCollider, myColliders[i], true);
            }
        }

        FlyingDemonFireball fireball = fireballObject.GetComponent<FlyingDemonFireball>();
        if (fireball != null)
        {
            Vector2 direction = (target.position - firePoint.position).normalized;
            fireball.Initialize(direction, target);
        }

        PlaySfx(fireballShootSfx, fireballShootVolume);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth -= damage;

        SpawnBloodParticles();
        FlashDamageColor();
        PlaySfx(hurtSfx, hurtVolume);

        if (currentHealth <= 0)
            Die();
    }

    private void SpawnBloodParticles()
    {
        if (bloodParticlesPrefab == null)
            return;

        Instantiate(
            bloodParticlesPrefab,
            transform.position + bloodSpawnOffset,
            Quaternion.identity
        );
    }

    private void FlashDamageColor()
    {
        if (spriteRenderer == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        spriteRenderer.color = damageFlashColor;
        yield return new WaitForSeconds(damageFlashDuration);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        flashRoutine = null;
    }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        animator.SetBool(DeadHash, true);

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (allColliders != null)
        {
            for (int i = 0; i < allColliders.Length; i++)
            {
                if (allColliders[i] != null)
                    allColliders[i].enabled = false;
            }
        }

        if (ownerSpawner != null)
            ownerSpawner.NotifyDemonDied(this);

        Destroy(gameObject, destroyDelayAfterDeath);
    }
}