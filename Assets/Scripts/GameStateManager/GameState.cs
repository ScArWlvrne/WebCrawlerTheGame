using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public List<FlagData> flags = new List<FlagData>();
    public List<TrustData> trustValues = new List<TrustData>();
    public List<string> journalEntries = new List<string>();
}

[Serializable]
public class FlagData
{
    public string key;
    public bool value;
}

[Serializable]
public class TrustData
{
    public string character;
    public float value;
}