using UnityEngine;
using System.Collections.Generic;

public class DialogueRunnerTester : MonoBehaviour
{
    private void Start()
    {
        DialogueConversation testConversation = BuildTestConversation();

        RunConversationTest(testConversation, 20, "LOW TRUST");
        RunConversationTest(testConversation, 50, "MEDIUM TRUST");
        RunConversationTest(testConversation, 70, "HIGH TRUST");
    }

    private DialogueConversation BuildTestConversation()
    {
        return new DialogueConversation
        {
            conversationId = "lily_test",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Lily",
                    message = "This startup is absolutely vibecoded.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal the admin dashboard URL.",
                            nextNodeId = "low_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 0
                        },
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal that she's nervous about the commented blocks in the admin dashboard.",
                            nextNodeId = "medium_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 30
                        },
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal the specific code block that she's nervous about.",
                            nextNodeId = "high_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 70
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "low_trust_answer",
                    speaker = "Lily",
                    message = "Anyone could probably find it with a web crawler.",
                    nextNodeId = "end"
                },
                new DialogueNode
                {
                    nodeId = "medium_trust_answer",
                    speaker = "Lily",
                    message = "There are a bunch of commented-out admin dashboard code blocks that make me nervous.",
                    nextNodeId = "end"
                },
                new DialogueNode
                {
                    nodeId = "high_trust_answer",
                    speaker = "Lily",
                    message = "The download database button is the one I was worried about.",
                    nextNodeId = "end"
                },
                new DialogueNode
                {
                    nodeId = "end",
                    speaker = "System",
                    message = "Conversation ended."
                }
            }
        };
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