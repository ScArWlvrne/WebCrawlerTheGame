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
        currentNode = ResolveNode(conversation.startNodeId);
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
        currentNode = ResolveNode(option.nextNodeId);
    }

    public void Continue()
    {
        if (currentNode == null)
            return;

        currentNode = ResolveNode(currentNode.nextNodeId);
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

    private DialogueNode ResolveNode(string nodeId)
    {
        const int maxRouteHops = 32;

        DialogueNode node = FindNode(nodeId);
        int routeHops = 0;

        while (node != null && node.kind == DialogueNodeKind.Router && routeHops < maxRouteHops)
        {
            string nextNodeId = null;

            foreach (DialogueRoute route in node.routes)
            {
                if (DialogueStateEvaluator.CanFollowRoute(route))
                {
                    nextNodeId = route.nextNodeId;
                    break;
                }
            }

            if (string.IsNullOrEmpty(nextNodeId))
                return null;

            node = FindNode(nextNodeId);
            routeHops++;
        }

        if (routeHops >= maxRouteHops)
            Debug.LogWarning("Dialogue router exceeded max hops starting at: " + nodeId);

        return node;
    }
}