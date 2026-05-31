#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DialogueUISetup
{
    private const string PrefabPath = "Assets/UI/Dialogue/DialoguePanel.prefab";
    private const string ChoiceRowPrefabPath = "Assets/UI/Dialogue/ChoiceRow.prefab";
    private const string RegistryPath = "Assets/Dialogue/Characters/DialogueCharacterRegistry.asset";
    private const string ResourcesPrefabPath = "Assets/Resources/Dialogue/DialoguePanel.prefab";

    [MenuItem("Tools/Dialogue/Setup Dialogue UI")]
    public static void SetupDialogueUI()
    {
        EnsureSpriteImport("Assets/Dialogue/Characters/joe_schmo_fosho.png");
        EnsureSpriteImport("Assets/Dialogue/Characters/donald_musk.png");

        Sprite lilySprite = null;
        Sprite lilyPadSprite = null;
        Sprite joeSprite = LoadSprite("Assets/Dialogue/Characters/joe_schmo_fosho.png");
        Sprite donSprite = LoadSprite("Assets/Dialogue/Characters/donald_musk.png");

        Directory.CreateDirectory("Assets/Dialogue/Characters");
        Directory.CreateDirectory("Assets/UI/Dialogue");
        Directory.CreateDirectory("Assets/Resources/Dialogue");

        DialogueCharacterProfile lily = CreateOrUpdateProfile(
            "Assets/Dialogue/Characters/Lily.asset", GameCharacters.Lily, "Lily Chen", lilySprite);
        DialogueCharacterProfile lilyPad = CreateOrUpdateProfile(
            "Assets/Dialogue/Characters/LilyPad.asset", GameCharacters.LilyPad, "Lily Pad", lilyPadSprite);
        DialogueCharacterProfile joe = CreateOrUpdateProfile(
            "Assets/Dialogue/Characters/Joe.asset", GameCharacters.Joe, "Joe Schmo FoSho", joeSprite);
        DialogueCharacterProfile don = CreateOrUpdateProfile(
            "Assets/Dialogue/Characters/Don.asset", "don", "Don", donSprite);

        DialogueCharacterRegistry registry = AssetDatabase.LoadAssetAtPath<DialogueCharacterRegistry>(RegistryPath);
        if (registry == null)
        {
            registry = ScriptableObject.CreateInstance<DialogueCharacterRegistry>();
            AssetDatabase.CreateAsset(registry, RegistryPath);
        }

        SerializedObject registryObject = new SerializedObject(registry);
        SerializedProperty profilesProperty = registryObject.FindProperty("profiles");
        profilesProperty.ClearArray();
        AddProfile(profilesProperty, lily);
        AddProfile(profilesProperty, lilyPad);
        AddProfile(profilesProperty, joe);
        AddProfile(profilesProperty, don);
        registryObject.ApplyModifiedPropertiesWithoutUndo();

        GameObject choiceRowPrefab = BuildChoiceRowPrefab();
        SavePrefab(choiceRowPrefab, ChoiceRowPrefabPath);
        Object.DestroyImmediate(choiceRowPrefab);

        GameObject dialogueRoot = BuildDialogueRoot(registry, ChoiceRowPrefabPath);
        SavePrefab(dialogueRoot, PrefabPath);
        SavePrefab(Object.Instantiate(dialogueRoot), ResourcesPrefabPath);
        Object.DestroyImmediate(dialogueRoot);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Dialogue UI setup complete. Prefabs at " + PrefabPath + " and " + ResourcesPrefabPath);
    }

    private static GameObject BuildChoiceRowPrefab()
    {
        return DialogueUILayoutBuilder.CreateChoiceRowPrefab();
    }

    private static GameObject BuildDialogueRoot(DialogueCharacterRegistry registry, string choiceRowPath)
    {
        GameObject root = new GameObject("DialogueSystem");
        DialogueUILayoutBuilder.BuiltReferences built = DialogueUILayoutBuilder.Build(root.transform);

        DialogueUI dialogueUI = root.AddComponent<DialogueUI>();
        GameObject choiceRowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(choiceRowPath);

        dialogueUI.ConfigureForEditor(
            built.dialoguePanel,
            built.worldDimmer,
            built.portraitImage,
            built.speakerText,
            built.messageText,
            built.choicesContainer,
            built.optionsText,
            built.hintText,
            registry,
            choiceRowPrefab);

        return root;
    }

    private static void SavePrefab(GameObject root, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(root, path);
    }

    private static DialogueCharacterProfile CreateOrUpdateProfile(
        string path,
        string characterId,
        string displayName,
        Sprite portrait)
    {
        DialogueCharacterProfile profile = AssetDatabase.LoadAssetAtPath<DialogueCharacterProfile>(path);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<DialogueCharacterProfile>();
            AssetDatabase.CreateAsset(profile, path);
        }

        profile.characterId = characterId;
        profile.displayName = displayName;
        profile.portrait = portrait;
        EditorUtility.SetDirty(profile);
        return profile;
    }

    private static void AddProfile(SerializedProperty profilesProperty, DialogueCharacterProfile profile)
    {
        int index = profilesProperty.arraySize;
        profilesProperty.InsertArrayElementAtIndex(index);
        profilesProperty.GetArrayElementAtIndex(index).objectReferenceValue = profile;
    }

    private static void EnsureSpriteImport(string texturePath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
            return;

        if (importer.textureType != TextureImporterType.Sprite || importer.spriteImportMode != SpriteImportMode.Single)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
    }

    private static Sprite LoadSprite(string texturePath)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
    }
}
#endif
