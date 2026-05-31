using UnityEngine;
using TMPro;

public class TextInput : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private string JournalEntryPath = "test_note.txt";
    [SerializeField] private string ExpectedEntryContent = "This is a test journal entry collected from ArnavTestScene.";
    [SerializeField] private TMP_Text InputTextField;
    [SerializeField] string InputTextFieldContent = "Correct text";
 
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        
        if (!GameStateManager.Instance.HasJournalFile(JournalEntryPath))
        {
            InputText("Insufficient information in journal");
            return;
        }
        if (GameStateManager.Instance.GetJournalFileContent(JournalEntryPath).Contains(ExpectedEntryContent))
        {
            InputText(InputTextFieldContent);
        }
        else
        {
            InputText("Insufficient information in journal");
        }
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor;
    }
    
    private void InputText(string input = "")
    {
        InputTextField.text = input;
    }
}