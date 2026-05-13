using UnityEngine;

public class TimedEffectDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.08f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}