using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerController;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponentInParent<PlayerController2D>();
    }

    public void AE_EnableQuickAttackHitbox()
    {
        if (playerController != null)
            playerController.AE_EnableQuickAttackHitbox();
    }

    public void AE_EnableHeavyAttackHitbox()
    {
        if (playerController != null)
            playerController.AE_EnableHeavyAttackHitbox();
    }

    public void AE_DisableAttackHitbox()
    {
        if (playerController != null)
            playerController.AE_DisableAttackHitbox();
    }
}