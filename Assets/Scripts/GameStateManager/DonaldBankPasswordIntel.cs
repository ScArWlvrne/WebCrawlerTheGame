public static class DonaldBankPasswordIntel
{
    public const string Password = "tempPword123!";
    private const string LegacyPassword = "Araknyd628!";

    public static readonly string PasswordsJournalPath = JournalPaths.Build(JournalPaths.CEO, "passwords.txt");
    private static readonly string TempPasswordJournalPath = JournalPaths.Build(JournalPaths.CEO, "temp_password.txt");

    public const string PasswordsJournalEntry =
        "online.xbank.com/executive: " + Password + " (synced from admin dashboard File Explorer)";

    public const string TempPasswordJournalHint =
        "Donald Musk temporary password: " + Password +
        " (standard IT reset template from email; Haley says he reuses it everywhere).";

    public static void SyncToPasswordsFile()
    {
        GameStateManager.Instance.AddJournalFile(PasswordsJournalPath, PasswordsJournalEntry);
    }

    public static bool IsKnown()
    {
        if (GameStateManager.Instance == null)
            return false;

        return JournalFileContains(TempPasswordJournalPath) || JournalFileContains(PasswordsJournalPath);
    }

    private static bool JournalFileContains(string path)
    {
        if (!GameStateManager.Instance.HasJournalFile(path))
            return false;

        string content = GameStateManager.Instance.GetJournalFileContent(path);
        return content.Contains(Password) || content.Contains(LegacyPassword);
    }
}
