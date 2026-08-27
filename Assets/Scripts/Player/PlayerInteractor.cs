using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    private static readonly int InteractHash = Animator.StringToHash("Interact");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float interactAngle = 60f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private Animator animator;
    [SerializeField] private float interactAnimationTimeout = 1.25f;

    private PlayerController playerController;
    public IInteractable currentInteractable;
    private bool isInteracting;
    public bool IsInteracting => isInteracting;


    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentInteractable();
    }

    private void UpdateCurrentInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            interactableLayer
        );

        Debug.Log($"Hits: {hits.Length}");

        IInteractable bestInteractable = null;
        currentInteractable = null;
        float bestDistance = float.PositiveInfinity;

        foreach (Collider hit in hits)
        {
            Debug.Log($"Hit: {hit.name}");
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

            Debug.Log($"Interactable: {interactable}");
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
        if ( playerController != null && (IsInteracting || playerController.GameplayInputLocked))
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
            animator.SetBool(IsMovingHash, false);
            animator.SetTrigger(InteractHash);
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

    private void VisualizeInteractRange()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Vector3 leftDir = Quaternion.Euler(0f, -interactAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, interactAngle * 0.5f, 0f) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftDir * interactRange);
        Gizmos.DrawRay(transform.position, rightDir * interactRange);
    }

    private void OnDrawGizmos()
    {
        VisualizeInteractRange();
    }
}
