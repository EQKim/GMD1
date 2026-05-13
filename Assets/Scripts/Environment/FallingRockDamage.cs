using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FallingRockDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 20;

    [Header("Impact FX")]
    [SerializeField] private GameObject impactEffect;

    [Header("Audio")]
    [SerializeField] private AudioClip impactSfx;
    [SerializeField, Range(0f, 1f)] private float impactVolume = 1f;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(damage);

        Vector2 hitPoint = other.ClosestPoint(transform.position);

        if (impactEffect != null)
        {
            Instantiate(impactEffect, hitPoint, Quaternion.identity);
        }

        if (impactSfx != null)
        {
            AudioSource.PlayClipAtPoint(impactSfx, hitPoint, impactVolume);
        }

        Destroy(gameObject);
    }
}