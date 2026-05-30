using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private CharacterController controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
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

       Vector3 move = new Vector3(input.x, 0, input.y);

       controller.Move(move * moveSpeed * Time.deltaTime);
    }
}
