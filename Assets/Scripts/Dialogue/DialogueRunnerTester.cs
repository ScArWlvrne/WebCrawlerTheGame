using UnityEngine;
using System.Collections.Generic;

public class DialogueRunnerTester : MonoBehaviour
{
    private void Start()
    {
        DialogueConversation testConversation = DialogueConversationFactory.GetLilyTestConversation();

        RunConversationTest(testConversation, 20, "LOW TRUST");
        RunConversationTest(testConversation, 50, "MEDIUM TRUST");
        RunConversationTest(testConversation, 70, "HIGH TRUST");
    }

    private void RunConversationTest(DialogueConversation conversation, int trustValue, string label)
    {
        Debug.Log("==============================");
        Debug.Log("DIALOGUE TEST: " + label);
        Debug.Log("Starting Lily trust: " + trustValue);

        GameStateManager.Instance.SetTrust(GameCharacters.Lily, trustValue);

        DialogueRunner runner = new DialogueRunner();
        runner.StartConversation(conversation);

        LogCurrentNode(runner);

        List<DialogueOption> visibleOptions = runner.GetVisibleOptions();

        Debug.Log("Visible option count: " + visibleOptions.Count);

        for (int i = 0; i < visibleOptions.Count; i++)
        {
            Debug.Log("Option " + i + ": " + visibleOptions[i].optionText);
        }

        if (visibleOptions.Count == 0)
        {
            Debug.LogWarning("No visible options. Ending test early.");
            return;
        }

        DialogueOption chosenOption = visibleOptions[visibleOptions.Count - 1];

        Debug.Log("Chosen option: " + chosenOption.optionText);

        runner.ChooseOption(chosenOption);

        LogCurrentNode(runner);

        runner.Continue();

        LogCurrentNode(runner);

        runner.Continue();

        Debug.Log("Conversation over: " + runner.IsConversationOver());
        Debug.Log("END TEST: " + label);
        Debug.Log("==============================");
    }

    private void LogCurrentNode(DialogueRunner runner)
    {
        if (runner.CurrentNode == null)
        {
            Debug.Log("Current node: null");
            return;
        }

        Debug.Log("Current speaker: " + runner.CurrentNode.speaker);
        Debug.Log("Current message: " + runner.CurrentNode.message);
    }
}