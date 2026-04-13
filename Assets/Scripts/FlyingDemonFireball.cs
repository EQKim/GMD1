using UnityEngine;

public class FlyingDemonFireball : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 15;
    [SerializeField] private float lifeTime = 5f;

    [Header("Collision")]
    [SerializeField] private LayerMask playerLayers;

    private Vector2 moveDirection;
    private Transform target;

    public void Initialize(Vector2 direction, Transform assignedTarget)
    {
        moveDirection = direction.normalized;
        target = assignedTarget;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayers) == 0)
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null)
            health = other.GetComponentInParent<PlayerHealth>();

        if (health == null)
            return;

        if (target != null && health.transform != target)
            return;

        health.TakeDamage(damage);
        Destroy(gameObject);
    }
}