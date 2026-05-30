using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 270f;
    [SerializeField] private float maxRotationSpeedMultiplier = 2.5f;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Keyboard.current == null)
        //     {
        //         Debug.Log("No keyboard detected");
        //     }
        //     else if (Keyboard.current.wKey.wasPressedThisFrame)
        //     {
        //         Debug.Log("W pressed");
        //     }

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

        

        animator.SetBool("IsMoving", isMoving);

        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
