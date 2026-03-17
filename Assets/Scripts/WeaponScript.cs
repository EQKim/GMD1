using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Picked up weapon!");

        // TODO: give weapon to player here

        Destroy(gameObject);
    }
}