using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Knife";
    [SerializeField] private GameObject equippedVisualPrefab;
    [SerializeField] private float duration = 10f;

    [Header("Damage Override")]
    [SerializeField] private int quickAttackDamage = 20;
    [SerializeField] private int heavyAttackDamage = 35;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField] private AudioClip attackSfx;

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

        holder.EquipWeapon(
            weaponName,
            equippedVisualPrefab,
            duration,
            quickAttackDamage,
            heavyAttackDamage,
            pickupSfx,
            attackSfx
        );

        Destroy(gameObject);
    }
}