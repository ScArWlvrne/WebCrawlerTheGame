using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private float interactAnimationTimeout = 1.25f;
    [SerializeField] private PlayerInteractor playerInteractor;
    private IInteractable currentInteractable;

    private bool gameplayInputLocked = false;
    private bool usingGamepad = false;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetGameplayInputLocked(bool locked)
    {
        gameplayInputLocked = locked;

        if (locked)
        {
            HideInteractionPrompt();
            if (animator != null)
                animator.SetBool("IsMoving", false);
        }
    }

    public void SetDialogueLocked(bool locked)
    {
        SetGameplayInputLocked(locked);
    }

    void Update()
    {
        if (playerInteractor.IsInteracting || gameplayInputLocked)
            return;

        UpdateCurrentInteractable();
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            input.x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
            input.y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;

            if (Keyboard.current.anyKey.isPressed)
            {
                usingGamepad = false;
            }
        }

        if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().magnitude > 0.2f && input == Vector2.zero)
        {
            input = Gamepad.current.leftStick.ReadValue();
            usingGamepad = true;
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

        if (
            Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame ||
            Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame
        )
        {
            SceneManager.LoadScene("DesktopHub");
        }

        animator.SetBool("IsMoving", isMoving);

        controller.Move(moveSpeed * Time.deltaTime * move);
    }

    private void OnDrawGizmos()
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