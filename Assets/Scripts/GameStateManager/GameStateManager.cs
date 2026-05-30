using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private Dictionary<string, bool> flags = 
        new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetFlag (string flagName, bool value) 
    {
        flags[flagName] = value; 
    }

    public bool GetFlag(string flagName)
    {
        if (flags.ContainsKey(flagName))
            return flags[flagName];

        return false;
    }
}