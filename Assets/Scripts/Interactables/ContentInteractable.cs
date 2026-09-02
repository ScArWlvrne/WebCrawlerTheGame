using UnityEngine;

public class ContentInteractable : Interactable
{
    public ContentLoader contentLoader;
    public ContentOperation[] operations;
    public override void Interact()
    {
        if (operations == null || operations.Length == 0)
        {
            Debug.LogWarning($"No operations defined for {gameObject.name}.");
            return;
        }

        foreach (ContentOperation operation in operations)
        {
            if (operation == null || operation.content == null)
            {
                Debug.LogWarning($"Operation or content is null for {gameObject.name}.");
                continue;
            }

            if ((operation.action == "load" || operation.action == "toggle") && !operation.content.isLoaded)
            {
                float timeToLoad = contentLoader.AddContent(operation.content);
                operation.content.isLoaded = true;
                Debug.Log($"Content loaded with action '{operation.action}'. Time taken: {timeToLoad} milliseconds.");
            }
            else if (operation.action == "unload" || (operation.action == "toggle" && operation.content.isLoaded))
            {
                float timeToUnload = contentLoader.UnloadContent(operation.content);
                operation.content.isLoaded = false;
                Debug.Log($"Content unloaded with action '{operation.action}'. Time taken: {timeToUnload} milliseconds.");
            }
            else
            {
                Debug.LogWarning($"Invalid action '{operation.action}' or content already in the desired state (isLoaded: {operation.content.isLoaded}).");
            }
        }
    }
}
