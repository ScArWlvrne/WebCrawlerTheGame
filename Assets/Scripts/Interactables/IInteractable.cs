using UnityEngine;
using UnityEngine.UI;
using TMPro;
public interface IInteractable
{
    void Interact();
    Transform GetPromptAnchor();
}
