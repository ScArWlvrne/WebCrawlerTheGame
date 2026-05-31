using System.Collections.Generic;

public class DialogueDocumentDto
{
    public DialogueMetaDto meta;
    public string startNode;
    public Dictionary<string, DialogueNodeDto> nodes;
}

public class DialogueMetaDto
{
    public string id;
    public string title;
    public string platform;
    public string character;
    public string description;
}

public class DialogueNodeDto
{
    public string type;
    public string speaker;
    public string avatar;
    public string text;
    public string autoNext;
    public List<DialogueRouteDto> routes;
    public List<DialogueChoiceDto> choices;
}

public class DialogueRouteDto
{
    public DialogueConditionsDto conditions;
    public string nextNode;
}

public class DialogueChoiceDto
{
    public string id;
    public string text;
    public string nextNode;
    public DialogueConditionsDto conditions;
    public DialogueEffectsDto effects;
}

public class DialogueConditionsDto
{
    public string requiresFlag;
    public string requiresNotFlag;
    public DialogueTrustCheckDto minTrust;
    public DialogueTrustCheckDto maxTrust;
}

public class DialogueTrustCheckDto
{
    public string character;
    public float value;
}

public class DialogueEffectsDto
{
    public DialogueTrustChangeDto addTrust;
    public DialogueFlagSetDto setFlag;
    public string addJournalEntry;
}

public class DialogueTrustChangeDto
{
    public string character;
    public float amount;
}

public class DialogueFlagSetDto
{
    public string key;
    public bool value;
}
