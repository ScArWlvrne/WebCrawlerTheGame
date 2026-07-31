using System;
using UnityEngine;

[Serializable]
public class ContentElement
{
    public string id;

    public ElementVector3 position;
    public ElementVector3 rotation;
    public ElementVector3 size;

    public string type;
    public string color;
    public string texture;
    public string text;
    public string font = "LiberationSans";
    public int fontSize;
}
