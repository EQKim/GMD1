using UnityEngine;

public class FallingObject : MonoBehaviour
{
    private float fallSpeed;
    private float despawnY;
    private bool isInitialized;

    public void Initialize(float speed, float destroyY)
    {
        fallSpeed = speed;
        despawnY = destroyY;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < despawnY)
            Destroy(gameObject);
    }
}