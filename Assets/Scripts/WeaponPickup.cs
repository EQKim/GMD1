using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Knife";
    [SerializeField] private GameObject equippedVisualPrefab;
    [SerializeField] private float duration = 10f;

    [Header("Weapon Type")]
    [SerializeField] private bool isRangedWeapon = false;

    [Tooltip("Shots per second for ranged weapons.")]
    [SerializeField] private float fireRate = 8f;

    [Header("Damage")]
    [SerializeField] private int quickAttackDamage = 20;
    [SerializeField] private int heavyAttackDamage = 35;

    [Header("Weapon Effects")]
    [Tooltip("If unchecked, knockback force will be treated as 0.")]
    [SerializeField] private bool enableKnockback = false;

    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private bool enableBleed = true;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [Range(0f, 1f)]
    [SerializeField] private float pickupSfxVolume = 1f;

    [SerializeField] private AudioClip attackSfx;
    [Range(0f, 1f)]
    [SerializeField] private float attackSfxVolume = 1f;

    private void OnValidate()
    {
        if (!enableKnockback)
            knockbackForce = 0f;
        else
            knockbackForce = Mathf.Max(0f, knockbackForce);

        fireRate = Mathf.Max(0.1f, fireRate);

        pickupSfxVolume = Mathf.Clamp01(pickupSfxVolume);
        attackSfxVolume = Mathf.Clamp01(attackSfxVolume);
    }

    private void Reset()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerWeaponHolder holder = other.GetComponentInParent<PlayerWeaponHolder>();
        if (holder == null)
            return;

        EquipTo(holder);
        Destroy(gameObject);
    }

    private void EquipTo(PlayerWeaponHolder holder)
    {
        holder.EquipWeapon(
            weaponName,
            equippedVisualPrefab,
            duration,
            isRangedWeapon,
            fireRate,
            quickAttackDamage,
            heavyAttackDamage,
            knockbackForce,
            enableBleed,
            pickupSfx,
            pickupSfxVolume,
            attackSfx,
            attackSfxVolume
        );
    }
}