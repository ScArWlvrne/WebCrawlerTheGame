using System;
using System.Collections.Generic;

[Serializable]
public class GameState
{
    public List<FlagData> flags = new List<FlagData>();
    public List<TrustData> trustValues = new List<TrustData>();
    public List<JournalFileData> journalFiles = new List<JournalFileData>();

    public List<string> exhaustedInteractables = new List<string>();
    public List<string> uncommentedCodeBlocks = new List<string>();
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

[Serializable]
public class JournalFileData
{
    public string path;
    public string content;
}