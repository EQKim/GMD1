using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    [SerializeField] private float fallbackDestroyTime = 1f;

    private ParticleSystem particleSystemToWatch;

    private void Awake()
    {
        particleSystemToWatch = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        if (particleSystemToWatch == null)
        {
            Destroy(gameObject, fallbackDestroyTime);
            return;
        }

        float totalLifetime =
            particleSystemToWatch.main.duration +
            particleSystemToWatch.main.startLifetime.constantMax;

        Destroy(gameObject, totalLifetime + 0.1f);
    }
}