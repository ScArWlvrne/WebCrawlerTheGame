using UnityEngine;
using TMPro;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;

public class ContentLoader : MonoBehaviour
{
    [SerializeField ] private string contentFile;

    public Dictionary<string, ContentDefinition> contentDefinitions = new Dictionary<string, ContentDefinition>();
    
    private readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new ElementVector3YamlConverter())
        .Build();
    private readonly ISerializer serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new ElementVector3YamlConverter())
        .Build();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (string.IsNullOrEmpty(contentFile))
        {
            Debug.Log("Content file name is not set. No load on start.");
            return;
        }
       LoadPage(contentFile);
    }

    public void LoadPage(string contentFile)
    {
        ClearAllContent();
        AddContent(contentFile);
    }
    public void LoadPage(ContentDefinition content)
    {
        ClearAllContent();
        AddContent(content);
    }

    public void UnloadPage(string contentFile)
    {
        UnloadContent(contentFile);
    }

    public void UnloadPage(ContentDefinition content)
    {
        UnloadContent(content);
    }

    

    public float AddContent(ContentDefinition content)
    {
        float startTime = Time.realtimeSinceStartup;



        if (content?.elements == null)
        {
            Debug.LogError($"Invalid content definition: '{contentFile}'.");
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        foreach (ContentElement element in content.elements)
        {
            CreateElement(element);
        }

        content.isLoaded = true; // Mark the content as loaded

       return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    public float UnloadContent(ContentDefinition content)
    {
        float startTime = Time.realtimeSinceStartup;
        foreach (Transform child in transform)
        {
            foreach (ContentElement element in content.elements)
            {
                if (child.name == element.id)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }

        content.isLoaded = false; // Mark the content as unloaded

        return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    public float AddContent(string contentFile)
    {
        float startTime = Time.realtimeSinceStartup;

        TextAsset yamlFile = LoadYaml(contentFile);
        if (yamlFile == null)
        {
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ContentDefinition content = ParseContent(yamlFile.text);
        AddContent(content);

        return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    public float UnloadContent(string contentFile)
    {
        float startTime = Time.realtimeSinceStartup;

        TextAsset yamlFile = LoadYaml(contentFile);
        if (yamlFile == null)
        {
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ContentDefinition content = ParseContent(yamlFile.text);
        UnloadContent(content);

        return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    private void ClearAllContent()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private TextAsset LoadYaml(string contentFile)
    {
        TextAsset yamlFile = Resources.Load<TextAsset>($"PageContents/{contentFile}");
        if (yamlFile == null)
        {
            Debug.LogError($"Content file '{contentFile}' not found in Resources/PageContents.");
            return null;
        }
        return yamlFile;
    }

    private ContentDefinition ParseContent(string yaml)
    {
        ContentDefinition parsedContent = null;

        try
        {    
            parsedContent = deserializer.Deserialize<ContentDefinition>(yaml);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse content YAML: {ex}");
            return null;
        }

        if (parsedContent == null)
        {
            Debug.LogError("Parsed content is null.");
            return null;
        }

        RegisterContent(parsedContent); // Register the content for future reference
        return parsedContent;
    }
    private void CreateElement(ContentElement element, Transform parent = null)
    {
        parent = parent != null ? parent : transform; // Use the provided parent or default to this transform

        GameObject obj = CreatePrimitive(element);

        if (obj == null)
        {
            Debug.LogWarning($"Failed to create object for element with id: {element.id}");
            return;
        }

        obj.name = element.id;
        ApplyTransform(obj, element, parent);
        ApplyRenderer(obj, element);
        ApplyText(obj, element);
        ApplyInteractable(obj, element.interactable);

        if (element.elements?.elements != null)
        {
            foreach (ContentElement child in element.elements.elements)
            {
                CreateElement(child, obj.transform);
            }
        }
    }

    private GameObject CreatePrimitive(ContentElement element)
    {
        GameObject obj = null;

        switch (element.type)
        {
            case "cube":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Debug.Log($"Created cube for element with id: {element.id}");
                break;

            case "panel":
                obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Debug.Log($"Created panel for element with id: {element.id}");
                break;

            case "sphere":
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Debug.Log($"Created sphere for element with id: {element.id}");
                break;

            case "capsule":
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                Debug.Log($"Created capsule for element with id: {element.id}");
                break;

            case "cylinder":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Debug.Log($"Created cylinder for element with id: {element.id}");
                break;

            case "text":
                obj = new GameObject("Text");
                obj.AddComponent<TextMeshPro>();
                Debug.Log($"Created text for element with id: {element.id}");
                break;

            case "container":
                obj = new GameObject("Container");
                Debug.Log($"Created container for element with id: {element.id}");
                break;

            default:
                Debug.LogWarning($"Unknown element type: {element.type}");
                break;
        }

        return obj;
    }

    private void ApplyTransform(GameObject obj, ContentElement element, Transform parent = null)
    {
        parent = parent != null ? parent : transform; // Use the provided parent or default to this transform

        obj.transform.SetParent(parent, false);
        obj.transform.localScale =  element.type is "container" or "text"
                                    ? Vector3.one
                                    : element.size?.ToVector3() ?? Vector3.one;
        obj.transform.SetLocalPositionAndRotation(
                                                  element.position?.ToVector3() ?? Vector3.zero,
                                                  Quaternion.Euler(element.rotation?.ToVector3() ?? Vector3.zero)
                                                  );
    }

    private void ApplyRenderer(GameObject obj, ContentElement element)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (ColorUtility.TryParseHtmlString(element.color, out Color parsedColor))
            {
                renderer.material.color = parsedColor;
            }
            
            if (!string.IsNullOrEmpty(element.texture))
            {
                Texture2D texture =
                    Resources.Load<Texture2D>($"Textures/{element.texture}");

                if (texture != null)
                {
                    renderer.material.mainTexture = texture;
                }
                else
                {
                    Debug.LogWarning($"Texture '{element.texture}' not found.");
                }
            }
        }
    }

    private void ApplyText(GameObject obj, ContentElement element)
    {
        TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            Debug.Log($"Applying text settings for element with id: {element.id}");

            tmp.alignment = TextAlignmentOptions.Center;
            Debug.Log($"Set alignment to center.");
            tmp.text = element.text;
            Debug.Log($"Set text: {element.text}");
            tmp.fontSize = element.fontSize;
            Debug.Log($"Set font size: {element.fontSize}");
            
            if (!string.IsNullOrEmpty(element.font))
            {
                TMP_FontAsset font =
                    Resources.Load<TMP_FontAsset>($"Fonts & Materials/{element.font}");

                if (font != null)
                {
                    tmp.font = font;
                    Debug.Log($"Set font: {element.font}");
                }
                else
                {
                    Debug.LogWarning($"Font '{element.font}' not found. Using default font.");
                }
            }

            if (ColorUtility.TryParseHtmlString(element.color, out Color parsedColor))
            {
                tmp.color = parsedColor;
                Debug.Log($"Set text color: {element.color}");
            }
        }
    }

    private void ApplyInteractable(GameObject obj, InteractableDefinition interactable)
    {
        Debug.Log($"Attempting to apply interactable settings for element with id: {obj.name}");

        if (interactable != null)
        {
            Debug.Log($"Applying interactable of type '{interactable.type}' to object '{obj.name}'.");
            Interactable component = null;

            switch (interactable.type)
            {
                case "file": // TODO: Implement FileInteractable class and uncomment the lines below
                    // component = obj.AddComponent<FileInteractable>();
                    // ((FileInteractable)component).filePath = interactable.filePath;
                    // ((FileInteractable)component).fileContent = interactable.fileContent;
                    break;
                case "scene":
                    component = obj.AddComponent<SceneLoadInteractable>();
                    ((SceneLoadInteractable)component).sceneName = interactable.sceneName;
                    break;
                case "content":
                    component = obj.AddComponent<ContentInteractable>();
                    ((ContentInteractable)component).operations = interactable.operations;;
                    ((ContentInteractable)component).contentLoader = this;
                    break;
                case "flag": // TODO: Implement FlagInteractable class and uncomment the lines below
                    // component = obj.AddComponent<FlagInteractable>();
                    // ((FlagInteractable)component).flagName = interactable.flagName;
                    break;
                default:
                    Debug.LogWarning($"Unknown interactable type: {interactable.type}");
                    break;
            }

            if (component != null)
            {
                component.exhaustible = interactable.exhaustible;
                obj.layer = LayerMask.NameToLayer("Interactable");

                Debug.Log($"Added Interactable component to object '{obj.name}' with type '{interactable.type}'.");
                return;
            }
            else
            {
                Debug.LogWarning($"Failed to add Interactable component to object '{obj.name}' for type '{interactable.type}'.");
                return;
            }
        }
        Debug.Log($"No interactable settings to apply for element with id: {obj.name}");
    }

    private ContentDefinition RegisterContent(ContentDefinition content, string parentId = null)
    {
        if (content == null)
            return null;

        if (string.IsNullOrEmpty(content.contentId) && string.IsNullOrEmpty(parentId))
        {
            Debug.LogWarning("Attempted to register root content with no contentId. Root content must have a unique contentId to be registered.");
            return null;
        }

        bool isReferenceOnly =
            !string.IsNullOrEmpty(content.contentId)
            && content.elements == null;

        if (isReferenceOnly)
        {
            if (!contentDefinitions.ContainsKey(content.contentId))
            {
                Debug.LogWarning($"Content reference with id '{content.contentId}' does not exist in the registry.");
                return null;
            }
            return contentDefinitions[content.contentId];
        }

        string registryId = !string.IsNullOrEmpty(content.contentId)
            ? content.contentId
            : $"{parentId}.{Guid.NewGuid()}";

        if (contentDefinitions.TryGetValue(registryId, out ContentDefinition existingContent))
        {
            Debug.LogWarning($"Content with id '{registryId}' is already registered.");
            return existingContent;
        }

        contentDefinitions.Add(registryId, content);

        if (content.elements == null)
            return content;

        foreach (ContentElement element in content.elements)
        {
            if (element.elements != null)
                element.elements = RegisterContent(element.elements, registryId);

            if (element.interactable?.operations != null)
            {       
                foreach (ContentOperation operation in element.interactable.operations)
                {
                    if (operation.content != null)
                        operation.content = RegisterContent(operation.content, registryId);
                }
            }
        }

        return content;
    }
}
