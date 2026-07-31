using UnityEngine;

public class ContentLoader : MonoBehaviour
{
    [SerializeField ] private string contentFile = "LevelLoaderTest";
    [SerializeField] private Transform contentRoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       LoadContent(contentFile);
    }

    public void LoadContent(string contentFile)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"PageContents/{contentFile}");
        if (jsonFile == null)
        {
            Debug.LogError($"Content file '{contentFile}' not found in Resources/PageContents.");
            return;
        }

        ContentDefinition content = JsonUtility.FromJson<ContentDefinition>(jsonFile.text);
        if (content?.elements == null)
        {
            Debug.LogError($"Invalid content definition: '{contentFile}'.");
            return;
        }

        ClearCurrentContent();
        foreach (ContentElement element in content.elements)
        {
            CreateElement(element);
       }
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
        ApplyMaterial(obj, element);
    }

    private GameObject CreatePrimitive(ContentElement element)
    {
        GameObject obj = null;

        switch (element.type)
        {
            case "cube":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;

            case "panel":
                obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                break;

            case "sphere":
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;

            case "capsule":
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                break;

            case "cylinder":
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;

            default:
                Debug.LogWarning($"Unknown element type: {element.type}");
                break;
        }

        return obj;
    }

    private void ApplyTransform(GameObject obj, ContentElement element)
    {
        obj.transform.SetParent(contentRoot, false);
        obj.transform.localScale = element.size?.ToVector3() ?? Vector3.one;
        obj.transform.SetLocalPositionAndRotation(
                                                  element.position?.ToVector3() ?? Vector3.zero,
                                                  Quaternion.Euler(element.rotation?.ToVector3() ?? Vector3.zero)
                                                  );
    }

    private void ApplyMaterial(GameObject obj, ContentElement element)
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

    private void ClearCurrentContent()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
    }
}
