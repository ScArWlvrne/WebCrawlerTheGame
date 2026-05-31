using TMPro;
using UnityEngine;

public class XBankTransferInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private TMP_Text outputText;

    public void Configure(TMP_Text targetOutputText)
    {
        outputText = targetOutputText;
    }

    public void Interact()
    {
        if (GameStateManager.Instance == null)
            return;

        if (!GameStateManager.Instance.GetFlag(GameFlags.XBankAccountAccessed))
        {
            SetOutput("X Bank transfer controls are locked until Donald's account is open.");
            return;
        }

        if (!GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.XBankTransferConfirmBlock))
        {
            SetOutput("Transfer confirmation script is still commented out in the fake X Bank page.");
            return;
        }

        GameStateManager.Instance.SetFlag(GameFlags.XBankWireInitiated, true);
        GameStateManager.Instance.SetFlag(GameFlags.XBankMeridianExposed, true);
        GameStateManager.Instance.SetFlag(GameFlags.GameWon, true);
        GameStateManager.Instance.SaveGame();

        SetOutput("Transfer of $847,293.00 initiated.\n\nTransaction history exposed: recurring wires to DataVault Solutions, Meridian Corp, and Araknyd consulting fees.");
    }

    public Transform GetPromptAnchor()
    {
        return promptAnchor != null ? promptAnchor : transform;
    }

    private void SetOutput(string text)
    {
        if (outputText != null)
            outputText.text = text;
    }
}
