using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Highlight")]
    [SerializeField] protected float outlineThickness = 0.03f;
    [SerializeField] private Material outlineMaterial;

    private GameObject outlineObject;
    private MeshRenderer outlineRenderer;
    private MeshFilter outlineMeshFilter;
    private bool isHighlighted;

    protected virtual void Awake()
    {
        MeshFilter sourceMeshFilter = GetComponent<MeshFilter>();
        if (sourceMeshFilter == null)
        {
            Debug.LogWarning($"{gameObject.name} has no MeshFilter.");
            return;
        }

        outlineObject = new GameObject($"{gameObject.name}_Outline");
        outlineObject.transform.SetParent(transform, false);
        outlineObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        outlineObject.transform.localScale = Vector3.one;

        outlineMeshFilter = outlineObject.AddComponent<MeshFilter>();
        outlineMeshFilter.sharedMesh = sourceMeshFilter.sharedMesh;

        outlineRenderer = outlineObject.AddComponent<MeshRenderer>();

        if (outlineMaterial == null)
        {
            Debug.LogWarning($"{gameObject.name} has no outline material assigned. Using default material.");
            outlineMaterial = Resources.Load<Material>("Materials/InteractableOutline");
        }

        outlineRenderer.material = outlineMaterial;
    }

    protected virtual void Start()
    {
        if (outlineObject == null)
        {
            Debug.LogWarning($"{gameObject.name} has no outline object.");
            return;
        }

        Unhighlight(true); // Ensure the outline is hidden at the start, true argument forces the unhighlighting even if it was already unhighlighted
        outlineObject.transform.localScale = Vector3.one * (1f + outlineThickness);
    }
    
    public abstract void Interact();

    public virtual void Highlight(bool force = false)
    {
        if (isHighlighted && !force || outlineObject == null)
            return;

        outlineObject.SetActive(true);
        isHighlighted = true;
    }

    public virtual void Unhighlight(bool force = false)
    {
        if (!isHighlighted && !force || outlineObject == null)
            return;

        outlineObject.SetActive(false);
        isHighlighted = false;
    }
}

