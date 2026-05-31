public static class JournalPaths
{
    public enum JournalOwner
    {
        Joe,
        Lily,
        Donald,
        CEO,
        XBank,
        WebDirector,
        Test
    }

    public const string Root = "usr";

    public const string Joe = "joe";
    public const string Lily = "lily";
    public const string Donald = "donald";
    public const string WebDirector = "web_director";
    public const string WebInspector = "web_inspector";
    public const string Araknyd = "araknyd";
    public const string CEO = "ceo";
    public const string XBank = "xbank";
    public const string Test = "test";

    public static string Build(string characterOrTarget, string fileName)
    {
        return Root + "/" + characterOrTarget + "/" + fileName;
    }

    public static string Build(string characterOrTarget, string subFolder, string fileName)
    {
        return Root + "/" + characterOrTarget + "/" + subFolder + "/" + fileName;
    }

    public static string GetOwnerFolder(JournalOwner owner)
    {
        switch (owner)
        {
            case JournalOwner.Joe:
                return Joe;
            case JournalOwner.Lily:
                return Lily;
            case JournalOwner.Donald:
                return Donald;
            case JournalOwner.CEO:
                return CEO;
            case JournalOwner.XBank:
                return XBank;
            case JournalOwner.WebDirector:
                return WebDirector;
            case JournalOwner.Test:
            default:
                return Test;
        }
    }
}
