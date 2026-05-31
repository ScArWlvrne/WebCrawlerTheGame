using UnityEngine;

/// <summary>
/// Drives IInteractable clicks via mouse raycasts. Used in view-only scenes (Email) with no Player.
/// </summary>
public class ViewCursorController : MonoBehaviour
{
    [SerializeField] private Camera viewCamera;
    [SerializeField] private LayerMask interactableLayers = ~0;
    [SerializeField] private float maxRayDistance = 100f;

    private void Awake()
    {
        if (viewCamera == null)
            viewCamera = Camera.main;
    }

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (viewCamera == null || !Input.GetMouseButtonDown(0))
            return;

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, interactableLayers, QueryTriggerInteraction.Collide))
            return;

        IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
            return;

        interactable.Interact();
    }
}
