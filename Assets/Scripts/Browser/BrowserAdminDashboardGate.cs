using TMPro;
using UnityEngine;

public class BrowserAdminDashboardGate : MonoBehaviour
{
    private const string AdminBetaUrl = "https://www.araknyd.io/admin-beta";
    private const string AdminUrlsPath = "usr/araknyd/urls.txt";
    private const string AdminDownloadCodeBlockId = "admin_download_database_button";

    [SerializeField] private TMP_Text urlText;
    [SerializeField] private TMP_Text sourceText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TextAsset adminDashboardSource;
    [SerializeField] private GameObject lockedStateRoot;
    [SerializeField] private GameObject unlockedStateRoot;
    [SerializeField] private bool createSourceDisplayWhenMissing = true;

    private GameObject exportCodeBlockObject;

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool discovered = IsAdminBetaDiscovered();
        bool dashboardUnlocked = GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(GameFlags.AdminDashboardUnlocked);

        ResolveReferences();

        if (urlText != null)
            urlText.text = discovered ? AdminBetaUrl : "|";

        if (sourceText != null)
            sourceText.text = discovered ? GetAdminSourceText(dashboardUnlocked) : "Run spider-crawl in Terminal to discover /admin-beta.";

        if (statusText != null)
        {
            statusText.text = dashboardUnlocked
                ? "View-source clue unlocked. Uncomment exportCustomerDatabase()."
                : "Dashboard located. Lily's intel is still needed for the export clue.";
        }

        if (lockedStateRoot != null)
            lockedStateRoot.SetActive(!discovered);

        if (unlockedStateRoot != null)
            unlockedStateRoot.SetActive(discovered);

        EnsureExportCodeBlock(discovered && dashboardUnlocked);
    }

    private bool IsAdminBetaDiscovered()
    {
        if (GameStateManager.Instance == null)
            return false;

        return GameStateManager.Instance.GetFlag(GameFlags.AraknydAdminBetaDiscovered) ||
            GameStateManager.Instance.HasJournalFile(AdminUrlsPath);
    }

    private string GetAdminSourceText(bool dashboardUnlocked)
    {
        if (adminDashboardSource == null)
            return "Admin dashboard source asset is not assigned.";

        if (!dashboardUnlocked)
        {
            return "Admin dashboard shell located.\n" +
                "Lily's intel is still needed before the exportCustomerDatabase source block is actionable.";
        }

        string source = adminDashboardSource.text;

        if (GameStateManager.Instance != null &&
            GameStateManager.Instance.IsCodeBlockUncommented(AdminDownloadCodeBlockId))
        {
            source = source.Replace("/* [code-block-id: admin_download_database_button]\n", "");
            source = source.Replace("\n*/\n// --- END admin_download_database_button ---", "\n// --- END admin_download_database_button ---");
        }

        return source;
    }

    private void ResolveReferences()
    {
        if (urlText == null)
            urlText = FindTextByGameObjectName("URL");

        if (sourceText == null && createSourceDisplayWhenMissing)
            sourceText = CreateSourceDisplay();
    }

    private TMP_Text FindTextByGameObjectName(string objectName)
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TMP_Text text in texts)
        {
            if (text.gameObject.name == objectName)
                return text;
        }

        return null;
    }

    private TMP_Text CreateSourceDisplay()
    {
        GameObject displayObject = new GameObject("Admin Source Preview");
        displayObject.transform.SetParent(transform, false);
        displayObject.transform.localPosition = new Vector3(0f, -1.2f, 0f);
        displayObject.transform.localRotation = Quaternion.identity;
        displayObject.transform.localScale = Vector3.one * 0.025f;

        TextMeshPro display = displayObject.AddComponent<TextMeshPro>();
        display.fontSize = 8f;
        display.textWrappingMode = TextWrappingModes.Normal;
        display.alignment = TextAlignmentOptions.TopLeft;
        display.rectTransform.sizeDelta = new Vector2(30f, 20f);

        return display;
    }

    private void EnsureExportCodeBlock(bool discovered)
    {
        if (exportCodeBlockObject == null)
        {
            exportCodeBlockObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            exportCodeBlockObject.name = "Admin Export Code Block";
            exportCodeBlockObject.layer = 3;
            exportCodeBlockObject.transform.SetParent(transform, false);
            exportCodeBlockObject.transform.localPosition = new Vector3(0f, -0.35f, 0.65f);
            exportCodeBlockObject.transform.localRotation = Quaternion.identity;
            exportCodeBlockObject.transform.localScale = new Vector3(0.6f, 0.08f, 0.18f);
            exportCodeBlockObject.AddComponent<CodeBlockInteractable>();
        }

        exportCodeBlockObject.SetActive(discovered);
    }
}
