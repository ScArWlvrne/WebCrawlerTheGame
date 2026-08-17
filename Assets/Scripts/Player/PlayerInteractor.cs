using UnityEngine;
using System.Collections;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float interactAngle = 60f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private Animator animator;
    [SerializeField] private float interactAnimationTimeout = 1.25f;
    [SerializeField] private PlayerController playerController;

    private IInteractable currentInteractable;
    private bool isInteracting;
    public bool IsInteracting => isInteracting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateCurrentInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            interactableLayer
        );

        IInteractable bestInteractable = null;
        currentInteractable = null;
        float bestDistance = float.PositiveInfinity;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            Vector3 targetPoint = hit.bounds.center;
            Vector3 directionToTarget = targetPoint - transform.position;
            directionToTarget.y = 0f;

            float distance = directionToTarget.magnitude;

            if (distance <= 0.001f)
                continue;

            float angle = Vector3.Angle(transform.forward, directionToTarget.normalized);

            if (angle > interactAngle * 0.5f)
                continue;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestInteractable = interactable;
            }
        }

        currentInteractable = bestInteractable;

        if (currentInteractable != null)
        {
            // Highlight interactable and change HUD
        }
        else
        {
            // Remove highlight and reset HUD;
        }
    }

    public void TryInteract()
    {
        if (isInteracting || playerController.GameplayInputLocked)
            return;

        if (currentInteractable != null)
        {
            MonoBehaviour target = currentInteractable as MonoBehaviour;
            string targetName = target != null ? target.gameObject.name : "unknown";
            Debug.Log("Player interacting with: " + targetName);
            StartCoroutine(PlayInteractAnimationThenInteract(currentInteractable));
        }
        else
        {
            Debug.Log("Nothing to interact with.");
        }
    }

    private IEnumerator PlayInteractAnimationThenInteract(IInteractable interactable)
    {
        isInteracting = true;

        bool waitForAnimationEvent = animator != null;
        if (waitForAnimationEvent)
        {
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Interact");
        }

        float elapsed = 0f;
        while (isInteracting && waitForAnimationEvent && elapsed < interactAnimationTimeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (isInteracting)
            Debug.LogWarning("Interact animation timed out — calling Interact() anyway.");

        isInteracting = false;
        interactable.Interact();
    }

    public void EndInteractionAnimation()
    {
        isInteracting = false;
    }
}
