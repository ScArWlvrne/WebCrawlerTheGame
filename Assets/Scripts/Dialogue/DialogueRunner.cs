using System.Collections.Generic;
using UnityEngine;

public class DialogueRunner
{
    private DialogueConversation conversation;
    private DialogueNode currentNode;

    public DialogueNode CurrentNode => currentNode;

    public void StartConversation(DialogueConversation newConversation)
    {
        conversation = newConversation;
        currentNode = FindNode(conversation.startNodeId);
    }

    public List<DialogueOption> GetVisibleOptions()
    {
        List<DialogueOption> visibleOptions = new List<DialogueOption>();

        if (currentNode == null)
            return visibleOptions;

        foreach (DialogueOption option in currentNode.options)
        {
            if (DialogueStateEvaluator.CanShowOption(option))
            {
                visibleOptions.Add(option);
            }
        }

        return visibleOptions;
    }

    public void ChooseOption(DialogueOption option)
    {
        DialogueStateEvaluator.ApplyOptionEffects(option);
        currentNode = FindNode(option.nextNodeId);
    }

    public void Continue()
    {
        if (currentNode == null)
            return;

        currentNode = FindNode(currentNode.nextNodeId);
    }

    public bool IsConversationOver()
    {
        return currentNode == null;
    }

    private DialogueNode FindNode(string nodeId)
    {
        if (conversation == null || string.IsNullOrEmpty(nodeId))
            return null;

        foreach (DialogueNode node in conversation.nodes)
        {
            if (node.nodeId == nodeId)
                return node;
        }

        Debug.LogWarning("Dialogue node not found: " + nodeId);
        return null;
    }
}