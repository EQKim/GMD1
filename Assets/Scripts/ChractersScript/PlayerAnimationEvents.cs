using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerController;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController2D>();
    }

    public void AE_EnableAttackHitbox()
    {
        if (playerController != null)
            playerController.AE_EnableAttackHitbox();
    }

    public void AE_DisableAttackHitbox()
    {
        if (playerController != null)
            playerController.AE_DisableAttackHitbox();
    }
}