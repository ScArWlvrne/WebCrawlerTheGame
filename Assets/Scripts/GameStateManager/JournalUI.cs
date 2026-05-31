using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JournalUI : MonoBehaviour
{
    public static JournalUI Instance { get; private set; }

    private static readonly Color WindowBackground = new Color(0.96f, 0.96f, 0.96f, 1f);
    private static readonly Color TitleBarBackground = new Color(0.88f, 0.88f, 0.88f, 1f);
    private static readonly Color BodyTextColor = new Color(0.12f, 0.12f, 0.12f, 1f);
    private static readonly Color HintTextColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    [Header("UI References (optional — built at runtime if missing)")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Input")]
    [SerializeField] private Key toggleKey = Key.J;

    private PlayerController playerController;
    private bool isVisible;

    public bool IsJournalOpen => isVisible;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureUIReferences();
        Hide();
    }

    public static void CreateDefaultUI()
    {
        if (Instance != null)
            return;

        new GameObject("JournalSystem").AddComponent<JournalUI>();
    }

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        CreateDefaultUI();

        if (Instance == null)
        {
            JournalUI existing = FindFirstObjectByType<JournalUI>();
            if (existing != null)
                Debug.LogWarning("JournalUI exists in scene but Instance was unset.");
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (IsDialogueBlockingJournal())
            return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
            Toggle();

        if (isVisible && Keyboard.current.escapeKey.wasPressedThisFrame)
            Hide();
    }

    public void Refresh()
    {
        if (bodyText == null)
            return;

        if (GameStateManager.Instance == null)
        {
            bodyText.text = "No game state loaded.";
            return;
        }

        StringBuilder builder = new StringBuilder();
        builder.Append("usr/\n");
        builder.Append("  araknyd/\n");
        builder.Append("  ceo/\n");
        builder.Append("  xbank/\n");
        builder.Append("\n");

        if (GameStateManager.Instance.State.journalFiles.Count == 0)
        {
            builder.Append("  (no files collected yet)");
        }
        else
        {
            foreach (JournalFileData file in GameStateManager.Instance.State.journalFiles)
            {
                builder.Append("  ");
                builder.Append(file.path);
                builder.Append("\n\n");
                builder.Append(file.content);
                builder.Append("\n\n---\n\n");
            }
        }

        bodyText.text = builder.ToString().TrimEnd();

        if (hintText != null)
            hintText.text = GetObjectiveHint();

        Debug.Log("JournalUI refreshed — file count: " + GameStateManager.Instance.State.journalFiles.Count);
    }

    public void Show()
    {
        if (IsDialogueBlockingJournal())
            return;

        Refresh();
        isVisible = true;

        if (journalPanel != null)
            journalPanel.SetActive(true);

        EnterJournalMode();
        Debug.Log("JournalUI: panel shown");
    }

    public void Hide()
    {
        isVisible = false;

        if (journalPanel != null)
            journalPanel.SetActive(false);

        ExitJournalMode();
        Debug.Log("JournalUI: panel hidden");
    }

    public void Toggle()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    private void EnterJournalMode()
    {
        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        playerController?.SetGameplayInputLocked(true);
        Debug.Log("ENTER JOURNAL MODE");
    }

    private void ExitJournalMode()
    {
        playerController?.SetGameplayInputLocked(false);
        Debug.Log("EXIT JOURNAL MODE");
    }

    private static bool IsDialogueBlockingJournal()
    {
        return DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive;
    }

    private void EnsureUIReferences()
    {
        if (journalPanel == null)
            BuildDefaultUIHierarchy();
    }

    private void BuildDefaultUIHierarchy()
    {
        Canvas canvas = gameObject.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = 950;

        if (journalPanel == null)
        {
            journalPanel = new GameObject("JournalPanel");
            journalPanel.transform.SetParent(transform, false);

            RectTransform panelRect = journalPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = journalPanel.AddComponent<Image>();
            panelImage.color = WindowBackground;
        }

        CreateTitleBar(journalPanel.transform);

        headerText = CreateText("HeaderText", journalPanel.transform, 28, FontStyles.Bold,
            new Vector2(0.04f, 0.92f), new Vector2(0.96f, 0.98f), TextAlignmentOptions.MidlineLeft);
        headerText.text = "Journal — File Explorer";
        headerText.color = BodyTextColor;

        bodyText = CreateText("BodyText", journalPanel.transform, 22, FontStyles.Normal,
            new Vector2(0.04f, 0.08f), new Vector2(0.96f, 0.9f), TextAlignmentOptions.TopLeft);
        bodyText.color = BodyTextColor;

        hintText = CreateText("HintText", journalPanel.transform, 20, FontStyles.Italic,
            new Vector2(0.04f, 0.02f), new Vector2(0.96f, 0.06f), TextAlignmentOptions.BottomLeft);
        hintText.text = GetObjectiveHint();
        hintText.color = HintTextColor;
    }

    private static string GetObjectiveHint()
    {
        if (GameStateManager.Instance == null)
            return "Press Escape to close and resume play. Press J to reopen later.";

        if (GameStateManager.Instance.GetFlag(GameFlags.GameWon))
            return "Objective complete: X Bank transfer initiated and Meridian wires exposed.";

        if (!GameStateManager.Instance.GetFlag(GameFlags.AraknydFinaleUnlocked))
            return GetAraknydObjectiveHint();

        if (!GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.XBank, "urls.txt")))
            return "Objective: Open the X Bank desktop site.";

        if (!GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.XBank, "username_hint.txt")))
            return "Objective: Read Donald's Email for the X Bank username.";

        if (!GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "temp_password.txt")))
            return "Objective: Use Venom to get Haley's password reset clue.";

        if (!GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "security_mother.txt")) ||
            !GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.CEO, "security_pet.txt")))
            return "Objective: Inspect Donald's FaceWeb clues in Spider Edge.";

        if (!GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.XBankMfaValidationBlock))
            return "Objective: Ask Web Inspector to unlock X Bank source and uncomment the MFA block.";

        if (!GameStateManager.Instance.GetFlag(GameFlags.XBankAccountAccessed))
            return "Objective: Use the X Bank login panel in Spider Edge.";

        return "Objective: Uncomment transfer confirmation and initiate the finale.";
    }

    private static string GetAraknydObjectiveHint()
    {
        if (!GameStateManager.Instance.HasJournalFile(JournalPaths.Build(JournalPaths.Araknyd, "urls.txt")))
            return "Objective: Use Terminal to discover Araknyd's admin page.";

        if (!GameStateManager.Instance.GetFlag(GameFlags.AdminDashboardUnlocked))
            return "Objective: Use Venom to get Lily's admin dashboard intel.";

        if (!GameStateManager.Instance.IsCodeBlockUncommented(GameCodeBlocks.AdminDownloadDatabaseButton))
            return "Objective: Open Spider Edge and uncomment Araknyd's database export.";

        return "Objective: Return to Desktop and open the X Bank site.";
    }

    private static void CreateTitleBar(Transform parent)
    {
        GameObject titleBar = new GameObject("TitleBar");
        titleBar.transform.SetParent(parent, false);

        RectTransform rect = titleBar.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.94f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = titleBar.AddComponent<Image>();
        image.color = TitleBarBackground;
    }

    private static TextMeshProUGUI CreateText(
        string objectName,
        Transform parent,
        int fontSize,
        FontStyles fontStyle,
        Vector2 anchorMin,
        Vector2 anchorMax,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        if (TMP_Settings.defaultFontAsset != null)
            text.font = TMP_Settings.defaultFontAsset;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }
}
