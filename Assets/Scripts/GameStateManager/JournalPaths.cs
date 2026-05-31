public static class JournalPaths
{
    public const string Root = "usr";

    public const string Joe = "joe";
    public const string Lily = "lily";
    public const string WebInspector = "web_inspector";
    public const string Araknyd = "araknyd";
    public const string CEO = "ceo";

    public static string Build(string characterOrTarget, string fileName)
    {
        return Root + "/" + characterOrTarget + "/" + fileName;
    }

    public static string Build(string characterOrTarget, string subFolder, string fileName)
    {
        return Root + "/" + characterOrTarget + "/" + subFolder + "/" + fileName;
    }
}