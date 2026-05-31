using UnityEngine;

public class SpiderAnimationEvents : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    public void EndInteractionAnimation()
    {
        playerController.EndInteractionAnimation();
    }
}