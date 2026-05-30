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

    public void AddJournalEntry(string entry)
    {
        State.journalEntries.Add(entry);
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

    public bool HasJournalEntry(string entry)
    {
        return State.journalEntries.Contains(entry);
    }

    public void SaveGame()
    {
        SaveSystem.Save(State);
    }

    public void LoadGame()
    {
        State = SaveSystem.Load();
    }
}