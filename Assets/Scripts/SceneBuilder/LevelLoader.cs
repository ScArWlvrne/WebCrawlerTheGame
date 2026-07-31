using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       TextAsset jsonFile = Resources.Load<TextAsset>("LevelJSON/LevelLoaderTest");
       LevelDefinition level = JsonUtility.FromJson<LevelDefinition>(jsonFile.text);
       foreach (LevelElement element in level.elements)
       {
           CreateElement(element);
       }
    }

    private void CreateElement(LevelElement element)
    {
        if (element.type == "cube")
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Renderer renderer = obj.GetComponent<Renderer>();
            

            obj.name = element.id;
            obj.transform.position = element.position.ToVector3();
            obj.transform.localScale = element.size.ToVector3();

            if (renderer != null)
            {
                if (ColorUtility.TryParseHtmlString(element.color, out Color parsedColor))
                {
                    renderer.material.color = parsedColor;
                }
            }
        }

        else if (element.type == "plane")
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Renderer renderer = obj.GetComponent<Renderer>();

            obj.name = element.id;
            obj.transform.position = element.position.ToVector3();
            obj.transform.localScale = element.size.ToVector3();

            if (renderer != null)
            {
                if (ColorUtility.TryParseHtmlString(element.color, out Color parsedColor))
                {
                    renderer.material.color = parsedColor;
                }
            }
        }
    }
}
