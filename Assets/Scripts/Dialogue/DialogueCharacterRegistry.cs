using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacterRegistry", menuName = "Dialogue/Character Registry")]
public class DialogueCharacterRegistry : ScriptableObject
{
    [SerializeField] private List<DialogueCharacterProfile> profiles = new List<DialogueCharacterProfile>();

    public DialogueCharacterProfile ResolveProfile(DialogueNode node)
    {
        if (node == null)
            return null;

        if (!string.IsNullOrEmpty(node.portraitCharacterId))
        {
            DialogueCharacterProfile byId = FindByCharacterId(node.portraitCharacterId);
            if (byId != null)
                return byId;
        }

        if (!string.IsNullOrEmpty(node.speaker))
        {
            DialogueCharacterProfile byName = FindByDisplayName(node.speaker);
            if (byName != null)
                return byName;

            DialogueCharacterProfile bySpeakerId = FindByCharacterId(node.speaker);
            if (bySpeakerId != null)
                return bySpeakerId;
        }

        return null;
    }

    public DialogueCharacterProfile FindByCharacterId(string characterId)
    {
        if (string.IsNullOrEmpty(characterId))
            return null;

        foreach (DialogueCharacterProfile profile in profiles)
        {
            if (profile != null &&
                string.Equals(profile.characterId, characterId, System.StringComparison.OrdinalIgnoreCase))
            {
                return profile;
            }
        }

        return null;
    }

    public DialogueCharacterProfile FindByDisplayName(string displayName)
    {
        if (string.IsNullOrEmpty(displayName))
            return null;

        foreach (DialogueCharacterProfile profile in profiles)
        {
            if (profile != null &&
                string.Equals(profile.displayName, displayName, System.StringComparison.OrdinalIgnoreCase))
            {
                return profile;
            }
        }

        return null;
    }
}
