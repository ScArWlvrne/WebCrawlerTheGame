using UnityEngine;

public class TestInteractable : Interactable
{
    private bool highlighted = false;

    public override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (!highlighted)
        {
            Highlight();
        }
        else
        {
            Unhighlight();
        }
        highlighted = !highlighted;
    }
}
