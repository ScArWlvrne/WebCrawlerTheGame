using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void EndInteractionAnimation()
    {
        playerController.EndInteractionAnimation();
    }
}