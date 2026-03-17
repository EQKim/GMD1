using UnityEngine;

public class FloatingPickup : MonoBehaviour
{
    [SerializeField] private float floatAmplitude = 0.5f; // was 0.15f
    [SerializeField] private float floatSpeed = 2.5f;     // tweak as needed

    private Vector3 baseLocalPosition;
    private float randomOffset;

    private void Start()
    {
        baseLocalPosition = transform.localPosition;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        Vector3 p = baseLocalPosition;
        p.y += Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatAmplitude;
        transform.localPosition = p;
    }
}