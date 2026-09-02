using System;
using YamlDotNet.Serialization;

[Serializable]
public class ContentDefinition
{
    public string contentId;
    public ContentElement[] elements;

    [YamlIgnore]
    public bool isLoaded = false; // This field is not serialized to YAML and is used to track if the content has been loaded
}