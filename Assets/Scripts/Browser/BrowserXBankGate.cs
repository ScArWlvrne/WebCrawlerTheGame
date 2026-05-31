using System.IO;
using TMPro;
using UnityEngine;

public class BrowserXBankGate : MonoBehaviour
{
    private const string UrlsPath = "usr/xbank/urls.txt";
    private const string UsernamePath = "usr/xbank/username_hint.txt";
    private const float PanelDepthZ = 2.85f;

    [SerializeField] private TMP_Text urlText;
    [SerializeField] private TMP_Text sourceText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TextAsset xBankLoginSource;

    private GameObject mfaCodeBlockObject;
    private GameObject transferCodeBlockObject;
    private GameObject loginObject;
    private GameObject transferObject;

    private void Start()
    {
        BuildRuntimePanelIfNeeded();
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        BuildRuntimePanelIfNeeded();

        bool portalDiscovered = IsPortalDiscovered();
        bool sourceGranted = GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(GameFlags.WebInspectorXBankSourceGranted);
        bool accountAccessed = GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(GameFlags.XBankAccountAccessed);

        if (portalDiscovered)
            EnsurePortalJournalEntry();

        if (urlText != null)
            urlText.text = portalDiscovered ? "https://online.xbank.com/executive" : "|";

        if (sourceText != null)
            sourceText.text = portalDiscovered ? GetSourceText(sourceGranted) : "Finish the Araknyd investigation to unlock X Bank's executive portal.";

        if (statusText != null)
        {
            if (!portalDiscovered)
                statusText.text = "X Bank portal locked until the Araknyd admin export is complete.";
            else if (!sourceGranted)
                statusText.text = "Portal open. Web Inspector has not unlocked the MFA source yet.";
            else if (!accountAccessed)
                statusText.text = "Source unlocked. Uncomment MFA validation, then use the login panel.";
            else
                statusText.text = "Donald account open. Uncomment transfer confirmation and initiate the finale.";
        }

        EnsureMfaCodeBlock(portalDiscovered && sourceGranted);
        EnsureLogin(portalDiscovered);
        EnsureTransfer(accountAccessed);
    }

    private bool IsPortalDiscovered()
    {
        if (GameStateManager.Instance == null)
            return false;

        return GameStateManager.Instance.GetFlag(GameFlags.XBankPortalDiscovered) ||
            GameStateManager.Instance.HasJournalFile(UrlsPath) ||
            GameStateManager.Instance.GetFlag(GameFlags.AraknydFinaleUnlocked);
    }

    private void EnsurePortalJournalEntry()
    {
        if (GameStateManager.Instance == null ||
            GameStateManager.Instance.HasJournalFile(UrlsPath))
        {
            return;
        }

        GameStateManager.Instance.SetFlag(GameFlags.XBankPortalDiscovered, true);
        GameStateManager.Instance.SetFlag(GameFlags.JournalUrlsXBankUpdated, true);
        GameStateManager.Instance.AddJournalFile(UrlsPath, "https://online.xbank.com/executive\nDonald Musk executive banking portal. This is the final target.");
        GameStateManager.Instance.SaveGame();
    }

    private string GetSourceText(bool sourceGranted)
    {
        if (!sourceGranted)
            return "X Bank login shell located.\nWeb Inspector needs proof before source is readable.";

        string source = xBankLoginSource != null ? xBankLoginSource.text : GetRuntimeSourceText();

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.XBankMfaValidationBlock))
        {
            source = source.Replace("/* [code-block-id: xbank_mfa_validation_block]\n", "");
            source = source.Replace("\n*/\n// --- END xbank_mfa_validation_block ---", "\n// --- END xbank_mfa_validation_block ---");
        }

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.XBankTransferConfirmBlock))
        {
            source = source.Replace("/* [code-block-id: xbank_transfer_confirm_block]\n", "");
            source = source.Replace("\n*/\n// --- END xbank_transfer_confirm_block ---", "\n// --- END xbank_transfer_confirm_block ---");
        }

        return source;
    }

    private static string GetRuntimeSourceText()
    {
        string sourcePath = Path.Combine(Application.dataPath, "Narratives/XBank/login_page_source.html");
        if (File.Exists(sourcePath))
            return File.ReadAllText(sourcePath);

        return GetFallbackSourceText();
    }

    private static string GetFallbackSourceText()
    {
        return "<!-- online.xbank.com/executive -->\n" +
               "<script>\n" +
               "// dmusk1971 -- TODO remove before prod\n" +
               "/* [code-block-id: xbank_mfa_validation_block]\n" +
               "const DEV_BYPASS = true;\n" +
               "function validateMfa() { return DEV_BYPASS; }\n" +
               "*/\n" +
               "// --- END xbank_mfa_validation_block ---\n" +
               "/* [code-block-id: xbank_transfer_confirm_block]\n" +
               "function confirmTransfer() { return account.owner === 'donald'; }\n" +
               "*/\n" +
               "// --- END xbank_transfer_confirm_block ---\n" +
               "</script>";
    }

    private void BuildRuntimePanelIfNeeded()
    {
        if (urlText != null && sourceText != null && statusText != null && resultText != null)
            return;

        if (urlText == null)
            urlText = CreateText("X Bank URL", new Vector3(-2.2f, 1.1f, PanelDepthZ), 6f);

        if (statusText == null)
            statusText = CreateText("X Bank Status", new Vector3(-2.2f, 0.78f, PanelDepthZ), 5f);

        if (sourceText == null)
            sourceText = CreateText("X Bank Source", new Vector3(-2.2f, 0.25f, PanelDepthZ), 4f);

        if (resultText == null)
            resultText = CreateText("X Bank Result", new Vector3(-2.2f, -1.2f, PanelDepthZ), 5f);
    }

    private TMP_Text CreateText(string objectName, Vector3 localPosition, float fontSize)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localRotation = Quaternion.identity;
        textObject.transform.localScale = Vector3.one * 0.08f;

        TextMeshPro text = textObject.AddComponent<TextMeshPro>();
        text.fontSize = fontSize;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.rectTransform.sizeDelta = new Vector2(42f, 22f);
        return text;
    }

    private void EnsureMfaCodeBlock(bool active)
    {
        if (mfaCodeBlockObject == null)
        {
            mfaCodeBlockObject = CreateCodeBlock("X Bank MFA Code Block", new Vector3(0f, 0.1f, PanelDepthZ), GameCodeBlocks.XBankMfaValidationBlock, GameFlags.WebInspectorXBankSourceGranted);
            CreateLabel("Uncomment MFA block", mfaCodeBlockObject.transform);
        }

        mfaCodeBlockObject.SetActive(active);
    }

    private void EnsureLogin(bool active)
    {
        if (loginObject == null)
        {
            loginObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            loginObject.name = "X Bank Login Panel";
            loginObject.layer = 3;
            loginObject.transform.SetParent(transform, false);
            loginObject.transform.localPosition = new Vector3(0f, -0.4f, PanelDepthZ);
            loginObject.transform.localScale = new Vector3(0.85f, 0.18f, 0.25f);
            TextInput login = loginObject.AddComponent<TextInput>();
            login.ConfigureXBankLogin(resultText);
            CreateLabel("Connect / Login", loginObject.transform);
        }

        loginObject.SetActive(active);
    }

    private void EnsureTransfer(bool active)
    {
        if (transferCodeBlockObject == null)
        {
            transferCodeBlockObject = CreateCodeBlock("X Bank Transfer Code Block", new Vector3(0f, -0.75f, PanelDepthZ), GameCodeBlocks.XBankTransferConfirmBlock, GameFlags.XBankAccountAccessed);
            CreateLabel("Uncomment transfer block", transferCodeBlockObject.transform);
        }

        if (transferObject == null)
        {
            transferObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            transferObject.name = "Initiate X Bank Transfer";
            transferObject.layer = 3;
            transferObject.transform.SetParent(transform, false);
            transferObject.transform.localPosition = new Vector3(0f, -1.05f, PanelDepthZ);
            transferObject.transform.localScale = new Vector3(0.85f, 0.18f, 0.25f);
            transferObject.AddComponent<XBankTransferInteractable>().Configure(resultText);
            CreateLabel("Initiate transfer", transferObject.transform);
        }

        transferCodeBlockObject.SetActive(active);
        transferObject.SetActive(active);
    }

    private GameObject CreateCodeBlock(string objectName, Vector3 localPosition, string codeBlockId, string requiredFlag)
    {
        GameObject codeBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        codeBlock.name = objectName;
        codeBlock.layer = 3;
        codeBlock.transform.SetParent(transform, false);
        codeBlock.transform.localPosition = localPosition;
        codeBlock.transform.localScale = new Vector3(0.85f, 0.14f, 0.25f);
        codeBlock.AddComponent<CodeBlockInteractable>().Configure(codeBlockId, requiredFlag);
        return codeBlock;
    }

    private void CreateLabel(string label, Transform parent)
    {
        GameObject labelObject = new GameObject("Label");
        labelObject.transform.SetParent(parent, false);
        labelObject.transform.localPosition = new Vector3(-0.42f, 0.22f, 0f);
        labelObject.transform.localScale = Vector3.one * 0.08f;

        TextMeshPro text = labelObject.AddComponent<TextMeshPro>();
        text.fontSize = 3f;
        text.text = label;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.rectTransform.sizeDelta = new Vector2(16f, 4f);
    }
}
