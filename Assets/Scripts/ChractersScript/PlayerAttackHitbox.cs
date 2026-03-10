using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int quickAttackDamage = 10;
    [SerializeField] private int heavyAttackDamage = 20;

    [Header("Target Filtering")]
    [SerializeField] private LayerMask targetLayers;

    private Collider2D hitboxCollider;
    private PlayerHealth ownerHealth;
    private int currentDamage;
    private bool attackActive;

    // Prevent hitting the same target multiple times in one punch
    private readonly HashSet<PlayerHealth> hitTargets = new HashSet<PlayerHealth>();

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        hitboxCollider.enabled = false;

        ownerHealth = GetComponentInParent<PlayerHealth>();
    }

    public void EnableQuickAttack()
    {
        currentDamage = quickAttackDamage;
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    public void EnableHeavyAttack()
    {
        currentDamage = heavyAttackDamage;
        attackActive = true;
        hitTargets.Clear();
        hitboxCollider.enabled = true;
    }

    public void DisableAttack()
    {
        attackActive = false;
        hitboxCollider.enabled = false;
        hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (!attackActive)
            return;

        if (((1 << other.gameObject.layer) & targetLayers) == 0)
            return;

        PlayerHealth targetHealth = other.GetComponentInParent<PlayerHealth>();
        if (targetHealth == null)
            return;

        if (targetHealth == ownerHealth)
            return;

        if (hitTargets.Contains(targetHealth))
            return;

        hitTargets.Add(targetHealth);
        targetHealth.TakeDamage(currentDamage);
    }
}