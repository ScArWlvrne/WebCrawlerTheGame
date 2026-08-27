using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(CharacterController))]

[RequireComponent(typeof(PlayerInteractor))]
public class PlayerController : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 270f;
    [SerializeField] private float maxRotationSpeedMultiplier = 2.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private InteractionPromptUI interactionPromptUI;
    [SerializeField] private float interactAnimationTimeout = 1.25f;

    private PlayerInteractor playerInteractor;
    private bool gameplayInputLocked = false;
    public bool GameplayInputLocked => gameplayInputLocked;
    private bool usingGamepad = false; // Move this to a HUD script eventually
    public bool UsingGamepad => usingGamepad; // "

    private CharacterController controller;

    void Awake()
    {
        playerInteractor = GetComponent<PlayerInteractor>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (playerInteractor != null && playerInteractor.IsInteracting || gameplayInputLocked)
            return;

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
            if (playerInteractor != null)
                playerInteractor.TryInteract();
        }

        if (
            Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame ||
            Gamepad.current != null && Gamepad.current.dpad.down.wasPressedThisFrame
        )
        {
            SceneManager.LoadScene("DesktopHub");
        }

        animator.SetBool(IsMovingHash, isMoving);

        controller.Move(moveSpeed * Time.deltaTime * move);
    }

    public void SetGameplayInputLocked(bool locked)
    {
        gameplayInputLocked = locked;

        if (locked)
        {
            if (animator != null)
                animator.SetBool(IsMovingHash, false);
        }
    }
}