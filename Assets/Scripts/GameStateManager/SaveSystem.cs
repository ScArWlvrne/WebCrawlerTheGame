using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SaveFolder =>
        Path.Combine(Application.persistentDataPath, "Saves");

    private static string SavePath =>
        Path.Combine(SaveFolder, "save.json");

    public static void Save(GameState state)
    {
        Directory.CreateDirectory(SaveFolder);

        string json = JsonUtility.ToJson(state, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("Saved game to: " + SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save deleted.");
        }
    }

    public static GameState Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found.");
            return new GameState();
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameState>(json);
    }
}