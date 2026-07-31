using UnityEngine;
using TMPro;

public class ContentLoader : MonoBehaviour
{
    [SerializeField ] private string contentFile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       LoadContent(contentFile);
    }

    public float LoadContent(string contentFile)
    {
        float startTime = Time.realtimeSinceStartup;

        TextAsset jsonFile = Resources.Load<TextAsset>($"PageContents/{contentFile}");
        if (jsonFile == null)
        {
            Debug.LogError($"Content file '{contentFile}' not found in Resources/PageContents.");
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ContentDefinition content = JsonUtility.FromJson<ContentDefinition>(jsonFile.text);
        if (content?.elements == null)
        {
            Debug.LogError($"Invalid content definition: '{contentFile}'.");
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ClearCurrentContent();
        foreach (ContentElement element in content.elements)
        {
            CreateElement(element);
        }
       return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
    }

    public float AddContent(string contentFile) // Just LoadContent, but it doesn't call ClearCurrentContent
    {
        float startTime = Time.realtimeSinceStartup;

        TextAsset jsonFile = Resources.Load<TextAsset>($"PageContents/{contentFile}");
        if (jsonFile == null)
        {
            Debug.LogError($"Content file '{contentFile}' not found in Resources/PageContents.");
            return (Time.realtimeSinceStartup - startTime) * 1000f; // Return elapsed time in milliseconds
        }

        ContentDefinition content = JsonUtility.FromJson<ContentDefinition>(jsonFile.text);
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
    private void CreateElement(ContentElement element)
    {
        GameObject obj = CreatePrimitive(element);

        if (obj == null)
        {
            Debug.LogWarning($"Failed to create object for element with id: {element.id}");
            return;
        }

        obj.name = element.id;
        ApplyTransform(obj, element);
        ApplyRenderer(obj, element);
        ApplyText(obj, element);
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

            default:
                Debug.LogWarning($"Unknown element type: {element.type}");
                break;
        }

        return obj;
    }

    private void ApplyTransform(GameObject obj, ContentElement element)
    {
        obj.transform.SetParent(transform, false);
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
            Texture2D texture = Resources.Load<Texture2D>($"Textures/{element.texture}");
            if (texture != null)
            {
                renderer.material.mainTexture = texture;
            }
        }
    }

    private void ApplyText(GameObject obj, ContentElement element)
    {
        TextMeshPro tmp = obj.GetComponent<TextMeshPro>();
        if (tmp != null)
        {
            tmp.text = element.text;
            tmp.fontSize = element.fontSize;
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>($"Fonts & Materials/{element.font}");
            if (font != null)
            {
                tmp.font = font;
            }
            else
            {
                Debug.LogWarning($"Font '{element.font}' not found. Using default font.");
            }
            if (ColorUtility.TryParseHtmlString(element.color, out Color parsedColor))
            {
                tmp.color = parsedColor;
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
