using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 270f;
    [SerializeField] private float maxRotationSpeedMultiplier = 2.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private float interactAngle = 60f;
    [SerializeField] private LayerMask interactableLayer;

    private bool isInteracting = false;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (isInteracting)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            input.x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
            input.y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
        }

        if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0.2f && input == Vector2.zero)
        {
            input = Gamepad.current.leftStick.ReadValue();
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 move = new Vector3(input.x, 0f, input.y);

        bool isMoving = move.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move.normalized);

            float angle = Quaternion.Angle(transform.rotation, targetRotation);

            float angleScalar = Mathf.InverseLerp(0f, 180f, angle);

            float adjustedTurnSpeed = rotationSpeed * Mathf.Lerp(1f, maxRotationSpeedMultiplier, angleScalar);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                adjustedTurnSpeed * Time.deltaTime
            );
        }

        if (
            Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame ||
            Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame
        )
        {
            TryInteract();
        }

        animator.SetBool("IsMoving", isMoving);

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void TryInteract()
    {
        if (isInteracting)
            return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            interactableLayer
        );

        IInteractable bestInteractable = null;
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

        if (bestInteractable != null)
        {
            StartCoroutine(PlayInteractAnimationThenInteract(bestInteractable));
            return;
        }

        Debug.Log("Nothing to interact with.");
    }

    private IEnumerator PlayInteractAnimationThenInteract(IInteractable interactable)
    {
        isInteracting = true;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
            animator.SetTrigger("Interact");
        }

        while (isInteracting)
        {
            yield return null;
        }

        interactable.Interact();
    }

    public void EndInteractionAnimation()
    {
        isInteracting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        Vector3 leftDir = Quaternion.Euler(0f, -interactAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0f, interactAngle * 0.5f, 0f) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, leftDir * interactRange);
        Gizmos.DrawRay(transform.position, rightDir * interactRange);
    }
}