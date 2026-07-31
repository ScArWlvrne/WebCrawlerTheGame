using System;
using UnityEngine;

[Serializable]
public class LevelElement
{
    public string id;

    public LevelVector3 position;
    public LevelVector3 size;

    public string type;
    public string color;
}
