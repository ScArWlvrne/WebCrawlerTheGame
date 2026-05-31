using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    private DialogueRunner runner;

    public void StartDialogue(DialogueConversation conversation)
    {
        runner = new DialogueRunner();
        runner.StartConversation(conversation);

        EnterDialogueMode();
        RefreshUI();
    }

    public void ContinueDialogue()
    {
        if (runner == null)
            return;

        runner.Continue();

        RefreshUI();
    }

    public void ChooseOption(DialogueOption option)
    {
        if (runner == null)
            return;

        runner.ChooseOption(option);

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (runner == null || runner.IsConversationOver())
        {
            HideDialogue();
            return;
        }

        DialogueNode node = runner.CurrentNode;
        List<DialogueOption> visibleOptions = runner.GetVisibleOptions();

        ShowDialogue(node, visibleOptions);
    }

    private void ShowDialogue(DialogueNode node, List<DialogueOption> visibleOptions)
    {
        Debug.Log("SHOW DIALOGUE UI");
        Debug.Log("Speaker: " + node.speaker);
        Debug.Log("Message: " + node.message);
        Debug.Log("Visible option count: " + visibleOptions.Count);

        for (int i = 0; i < visibleOptions.Count; i++)
        {
            Debug.Log("UI Option " + i + ": " + visibleOptions[i].optionText);
        }

        // Future UI hookup:
        // speakerText.text = node.speaker;
        // messageText.text = node.message;
        // create one button per visible option;
        // if visibleOptions.Count == 0, show Continue button instead.
    }

    private void HideDialogue()
    {
        Debug.Log("HIDE DIALOGUE UI");
        ExitDialogueMode();

        // Future UI hookup:
        // dialoguePanel.SetActive(false);
    }
}