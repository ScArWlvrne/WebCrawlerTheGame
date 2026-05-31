using UnityEngine;
using TMPro;

public class TextInput : MonoBehaviour, IInteractable
{
    private enum InputMode
    {
        JournalEcho,
        XBankLogin
    }

    [SerializeField] private InputMode inputMode = InputMode.JournalEcho;
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private string JournalEntryPath = "test_note.txt";
    [SerializeField] private string ExpectedEntryContent = "This is a test journal entry collected from ArnavTestScene.";
    [SerializeField] private TMP_Text InputTextField;
    [SerializeField] string InputTextFieldContent = "Correct text";
    [SerializeField] private string successText = "Access granted. X Bank executive dashboard unlocked.";
    [SerializeField] private string failureText = "Insufficient information in journal";

    public void ConfigureXBankLogin(TMP_Text outputText)
    {
        inputMode = InputMode.XBankLogin;
        InputTextField = outputText;
    }
 
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);

        if (inputMode == InputMode.XBankLogin)
        {
            TryXBankLogin();
            return;
        }
        
        if (!GameStateManager.Instance.HasJournalFile(JournalEntryPath))
        {
            InputText(failureText);
            return;
        }
        if (GameStateManager.Instance.GetJournalFileContent(JournalEntryPath).Contains(ExpectedEntryContent))
        {
            InputText(InputTextFieldContent);
        }
        else
        {
            InputText(failureText);
        }
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor;
    }
    
    private void InputText(string input = "")
    {
        if (InputTextField != null)
            InputTextField.text = input;
    }

    private void TryXBankLogin()
    {
        if (GameStateManager.Instance == null)
        {
            InputText(failureText);
            return;
        }

        GameStateManager.Instance.SetFlag(GameFlags.XBankLoginAttempted, true);

        if (!HasRequiredXBankFiles())
        {
            InputText("Missing X Bank login intel. Check Journal for usr/xbank and usr/ceo files.");
            GameStateManager.Instance.SaveGame();
            return;
        }

        if (!GameStateManager.Instance.GetFlag(GameFlags.WebInspectorXBankSourceGranted) ||
            !GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.XBankMfaValidationBlock))
        {
            InputText("Credentials accepted, but X Bank MFA blocks the session. Web Inspector needs the source proof.");
            GameStateManager.Instance.SaveGame();
            return;
        }

        GameStateManager.Instance.SetFlag(GameFlags.XBankAccountAccessed, true);
        GameStateManager.Instance.SaveGame();
        InputText(successText);
        FindFirstObjectByType<BrowserXBankGate>()?.Refresh();
    }

    private bool HasRequiredXBankFiles()
    {
        return GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.XBank, "urls.txt")) &&
               GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.XBank, "username_hint.txt")) &&
               GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "temp_password.txt")) &&
               GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "security_mother.txt")) &&
               GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "security_pet.txt"));
    }
}