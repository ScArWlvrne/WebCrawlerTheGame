using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public GameState State = new GameState();

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

    public void SetFlag(string flagName, bool value)
    {
        foreach (FlagData flag in State.flags)
        {
            if (flag.key == flagName)
            {
                flag.value = value;
                return;
            }
        }

        State.flags.Add(new FlagData
        {
            key = flagName,
            value = value
        });
    }

    public bool GetFlag(string flagName)
    {
        foreach (FlagData flag in State.flags)
        {
            if (flag.key == flagName)
                return flag.value;
        }

        return false;
    }

    public void AddJournalFile(string path, string content)
    {
        foreach (JournalFileData file in State.journalFiles)
        {
            if (file.path == path)
            {
                file.content = content;
                return;
            }
        }

        State.journalFiles.Add(new JournalFileData
        {
            path = path,
            content = content
        });
    }

    public bool HasJournalFile(string path)
    {
        foreach (JournalFileData file in State.journalFiles)
        {
            if (file.path == path)
                return true;
        }

        return false;
    }

    public string GetJournalFileContent(string path)
    {
        foreach (JournalFileData file in State.journalFiles)
        {
            if (file.path == path)
                return file.content;
        }

        return "";
    }

    public void RemoveJournalFile(string path)
    {
        for (int i = State.journalFiles.Count - 1; i >= 0; i--)
        {
            if (State.journalFiles[i].path == path)
            {
                State.journalFiles.RemoveAt(i);
                return;
            }
        }
    }

    public void SetTrust(string character, float value)
    {
        foreach (TrustData trust in State.trustValues)
        {
            if (trust.character == character)
            {
                trust.value = value;
                return;
            }
        }

        State.trustValues.Add(new TrustData
        {
            character = character,
            value = value
        });
    }

    public float GetTrust(string character)
    {
        foreach (TrustData trust in State.trustValues)
        {
            if (trust.character == character)
                return trust.value;
        }

        return 0f;
    }

    public void AddTrust(string character, float amount)
    {
        SetTrust(character, GetTrust(character) + amount);
    }

    public void SaveGame()
    {
        SaveSystem.Save(State);
    }

    public void LoadGame()
    {
        State = SaveSystem.Load();
    }

    public void ExhaustInteractable(string interactableId)
    {
        if (!State.exhaustedInteractables.Contains(interactableId))
        {
            State.exhaustedInteractables.Add(interactableId);
        }
    }

    public bool IsInteractableExhausted(string interactableId)
    {
        return State.exhaustedInteractables.Contains(interactableId);
    }

    public void UncommentCodeBlock(string codeBlockId)
    {
        if (!State.uncommentedCodeBlocks.Contains(codeBlockId))
        {
            State.uncommentedCodeBlocks.Add(codeBlockId);
        }
    }

    public bool IsCodeBlockUncommented(string codeBlockId)
    {
        return State.uncommentedCodeBlocks.Contains(codeBlockId);
    }

    public bool HasSave()
    {
        return SaveSystem.HasSave();
    }

    public void DeleteSave()
    {
        SaveSystem.DeleteSave();
    }

    public void NewGame()
    {
        State = new GameState();
        DeleteSave();
    }
}