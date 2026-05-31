using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "Dialogue/Character Profile")]
public class DialogueCharacterProfile : ScriptableObject
{
    public string characterId;
    public string displayName;
    public Sprite portrait;
}
