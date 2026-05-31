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
    public DialogueNodeKind kind = DialogueNodeKind.Npc;
    public string speaker;
    public string portraitCharacterId;
    public string message;
    public string nextNodeId;
    public List<DialogueRoute> routes = new List<DialogueRoute>();
    public List<DialogueOption> options = new List<DialogueOption>();
}

[Serializable]
public enum DialogueNodeKind
{
    Npc,
    System,
    Router
}

[Serializable]
public class DialogueRoute
{
    public DialogueConditionSet conditions = new DialogueConditionSet();
    public string nextNodeId;
}

[Serializable]
public class DialogueConditionSet
{
    public string requiredFlag;
    public string requiredNotFlag;

    public string trustCharacter;
    public bool hasMinTrust;
    public float minTrust;
    public bool hasMaxTrust;
    public float maxTrust = -1f;
}

[Serializable]
public class DialogueOption
{
    public string optionText;
    public string nextNodeId;

    public string requiredFlag;
    public string requiredNotFlag;
    public string requiredJournalFile;
    public string requiredUncommentedCodeBlock;
    public string requiredExhaustedInteractable;

    public string trustCharacter;
    public bool hasMinTrust;
    public float minTrust;
    public bool hasMaxTrust;
    public float maxTrust = -1f;

    public string flagToSet;

    public string journalFileToAddPath;
    public string journalFileToAddContent;

    public string trustChangeCharacter;
    public float trustChange;

    public List<DialogueEffect> effects = new List<DialogueEffect>();
}

[Serializable]
public class DialogueEffect
{
    public string trustChangeCharacter;
    public float trustChange;

    public string flagKey;
    public bool flagValue = true;

    public string journalEntry;
}