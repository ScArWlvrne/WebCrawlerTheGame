using System;
using System.Collections.Generic;

[Serializable]
public class DialogueConversation
{
    public string conversationId;
    public string startNodeId;
    public List<DialogueNode> nodes = new List<DialogueNode>();
}

[Serializable]
public class DialogueNode
{
    public string nodeId;
    public string speaker;
    public string message;
    public string nextNodeId;
    public List<DialogueOption> options = new List<DialogueOption>();
}

[Serializable]
public class DialogueOption
{
    public string optionText;
    public string nextNodeId;

    public string requiredFlag;
    public string requiredJournalFile;
    public string requiredUncommentedCodeBlock;
    public string requiredExhaustedInteractable;

    public string trustCharacter;
    public float minTrust;
    public float maxTrust = -1f;

    public string flagToSet;

    public string journalFileToAddPath;
    public string journalFileToAddContent;

    public string trustChangeCharacter;
    public float trustChange;
}