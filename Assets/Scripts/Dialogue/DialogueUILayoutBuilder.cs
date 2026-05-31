using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the dialogue bottom-sheet hierarchy. Used at runtime when prefab refs are missing,
/// and by the Editor setup menu when generating DialoguePanel.prefab.
/// </summary>
public static class DialogueUILayoutBuilder
{
    public struct BuiltReferences
    {
        public GameObject dialoguePanel;
        public GameObject worldDimmer;
        public Image portraitImage;
        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI messageText;
        public Transform choicesContainer;
        public TextMeshProUGUI optionsText;
        public TextMeshProUGUI hintText;
    }

    public static BuiltReferences Build(Transform root)
    {
        BuiltReferences refs = new BuiltReferences();

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = root.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            root.gameObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);
            root.gameObject.AddComponent<GraphicRaycaster>();
        }

        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;

        refs.worldDimmer = CreatePanel("WorldDimmer", root, Vector2.zero, Vector2.one,
            new Color(0f, 0f, 0f, 0.35f));

        refs.dialoguePanel = CreatePanel("DialoguePanel", root,
            new Vector2(0f, 0f), new Vector2(1f, 0.34f),
            new Color(0.08f, 0.09f, 0.12f, 0.94f));

        GameObject portraitFrame = CreatePanel("PortraitFrame", refs.dialoguePanel.transform,
            new Vector2(0.02f, 0.1f), new Vector2(0.16f, 0.9f),
            new Color(0.15f, 0.16f, 0.2f, 1f));

        refs.portraitImage = CreateImage("PortraitImage", portraitFrame.transform,
            new Vector2(0.06f, 0.06f), new Vector2(0.94f, 0.94f));
        refs.portraitImage.preserveAspect = true;
        refs.portraitImage.color = Color.white;

        refs.speakerText = CreateText("SpeakerName", refs.dialoguePanel.transform, 34, FontStyles.Bold,
            new Vector2(0.18f, 0.78f), new Vector2(0.96f, 0.95f), TextAlignmentOptions.BottomLeft);

        refs.messageText = CreateText("MessageBody", refs.dialoguePanel.transform, 28, FontStyles.Normal,
            new Vector2(0.18f, 0.38f), new Vector2(0.96f, 0.76f), TextAlignmentOptions.TopLeft);

        GameObject choicesHost = new GameObject("ChoicesContainer");
        choicesHost.transform.SetParent(refs.dialoguePanel.transform, false);
        RectTransform choicesRect = choicesHost.AddComponent<RectTransform>();
        choicesRect.anchorMin = new Vector2(0.18f, 0.08f);
        choicesRect.anchorMax = new Vector2(0.96f, 0.36f);
        choicesRect.offsetMin = Vector2.zero;
        choicesRect.offsetMax = Vector2.zero;
        VerticalLayoutGroup layout = choicesHost.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.spacing = 6f;
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        refs.choicesContainer = choicesHost.transform;

        refs.optionsText = CreateText("OptionsText", refs.dialoguePanel.transform, 24, FontStyles.Normal,
            new Vector2(0.18f, 0.08f), new Vector2(0.96f, 0.36f), TextAlignmentOptions.TopLeft);
        refs.optionsText.gameObject.SetActive(false);

        refs.hintText = CreateText("Hint", refs.dialoguePanel.transform, 20, FontStyles.Italic,
            new Vector2(0.18f, 0.02f), new Vector2(0.96f, 0.08f), TextAlignmentOptions.BottomLeft);
        refs.hintText.color = new Color(0.75f, 0.78f, 0.85f, 1f);

        refs.dialoguePanel.SetActive(false);
        refs.worldDimmer.SetActive(false);

        return refs;
    }

    public static GameObject CreateChoiceRowPrefab()
    {
        GameObject row = new GameObject("ChoiceRow");
        RectTransform rowRect = row.AddComponent<RectTransform>();
        rowRect.sizeDelta = new Vector2(0f, 32f);

        LayoutElement layoutElement = row.AddComponent<LayoutElement>();
        layoutElement.minHeight = 28f;
        layoutElement.preferredHeight = 32f;

        TextMeshProUGUI label = CreateText("Label", row.transform, 22, FontStyles.Normal,
            Vector2.zero, Vector2.one, TextAlignmentOptions.MidlineLeft);
        label.margin = new Vector4(4f, 0f, 0f, 0f);
        label.color = new Color(0.9f, 0.93f, 1f, 1f);

        return row;
    }

    private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static Image CreateImage(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        return imageObject.AddComponent<Image>();
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
        text.color = Color.white;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }
}
