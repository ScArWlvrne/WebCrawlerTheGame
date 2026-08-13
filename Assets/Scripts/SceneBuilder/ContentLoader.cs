using UnityEngine;
using TMPro;

public class ContentLoader : MonoBehaviour
{
    [SerializeField ] private string contentFile;

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
        ClearCurrentContent();
        AddContent(contentFile);
    }

    public float AddContent(string contentFile)
    {
        float startTime = Time.realtimeSinceStartup;

        TextAsset jsonFile = LoadJson(contentFile);
        if (jsonFile == null)
        {
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ContentDefinition content = ParseContent(jsonFile.text);
        if (content?.elements == null)
        {
            Debug.LogError($"Invalid content definition: '{contentFile}'.");
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        foreach (ContentElement element in content.elements)
        {
            CreateElement(element);
        }
       return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    private TextAsset LoadJson(string contentFile)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"PageContents/{contentFile}");
        if (jsonFile == null)
        {
            Debug.LogError($"Content file '{contentFile}' not found in Resources/PageContents.");
            return null;
        }
        return jsonFile;
    }

    private ContentDefinition ParseContent(string json)
    {
        try
        {
            return JsonUtility.FromJson<ContentDefinition>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse content JSON: {ex.Message}");
            return null;
        }
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
        obj.transform.localScale = element.size?.ToVector3() ?? Vector3.one;
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

    private void ClearCurrentContent()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
