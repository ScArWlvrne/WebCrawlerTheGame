using UnityEngine;

public static class DialogueDocumentLoader
{
    public static DialogueConversation Load(TextAsset dialogueAsset)
    {
        return DialogueLoader.FromJson(dialogueAsset);
    }
}
