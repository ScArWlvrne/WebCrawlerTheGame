using System;

[Serializable]
public class InteractableDefinition
{
    public string type; // Can be: file, scene, content, or flag
    public bool exhaustible = false; // If true, the interactable can only be used once

    public ContentOperation[] operations; // For content interactables, defines the operations to perform on the content when activated
    public string sceneName; // For scene to be loaded when a scene interactable is activated
    
    public string filePath; // For file to be created/edited when a file interactable is activated
    public string fileContent; // For file content to be appended when a file interactable is activated

    public string flagName; // For flag to be set when a flag interactable is activated
}
