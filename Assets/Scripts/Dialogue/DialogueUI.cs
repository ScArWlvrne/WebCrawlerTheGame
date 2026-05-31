using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI References (optional — built at runtime if missing)")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI hintText;

    private DialogueRunner runner;
    private PlayerController playerController;
    private List<DialogueOption> currentVisibleOptions = new List<DialogueOption>();
    private string pendingExhaustInteractableId;
    private bool awaitingClose;
    private bool isActive;

    public bool IsDialogueActive => isActive;

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
        HidePanel();
    }

    public static void CreateDefaultUI()
    {
        if (Instance != null)
            return;

        new GameObject("DialogueSystem").AddComponent<DialogueUI>();
    }

    public void StartDialogue(DialogueConversation conversation, string exhaustInteractableId = null)
    {
        Debug.Log("DialogueUI.StartDialogue: " + conversation.conversationId);

        pendingExhaustInteractableId = exhaustInteractableId;
        awaitingClose = false;

        runner = new DialogueRunner();
        runner.StartConversation(conversation);

        EnterDialogueMode();
        RefreshUI();
    }

    public void ContinueDialogue()
    {
        if (runner == null || awaitingClose)
            return;

        runner.Continue();
        RefreshUI();
    }

    public void ChooseOption(DialogueOption option)
    {
        if (runner == null || awaitingClose)
            return;

        runner.ChooseOption(option);
        GameStateManager.Instance?.SaveGame();
        RefreshUI();
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (awaitingClose)
        {
            if (WasClosePressed())
                CloseDialogue();
            return;
        }

        HandleDialogueInput();
    }

    private void HandleDialogueInput()
    {
        if (runner == null)
            return;

        if (currentVisibleOptions.Count > 0)
        {
            for (int i = 0; i < currentVisibleOptions.Count && i < 9; i++)
            {
                if (WasNumberPressed(i + 1))
                {
                    ChooseOption(currentVisibleOptions[i]);
                    return;
                }
            }
        }
        else if (WasContinuePressed())
        {
            ContinueDialogue();
        }
    }

    private void RefreshUI()
    {
        if (runner == null)
        {
            CloseDialogue();
            return;
        }

        if (runner.IsConversationOver())
        {
            OnConversationFinished();
            return;
        }

        DialogueNode node = runner.CurrentNode;
        currentVisibleOptions = runner.GetVisibleOptions();
        ShowDialogue(node, currentVisibleOptions);
    }

    private void OnConversationFinished()
    {
        if (!string.IsNullOrEmpty(pendingExhaustInteractableId) &&
            GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ExhaustInteractable(pendingExhaustInteractableId);
            GameStateManager.Instance.SaveGame();
            pendingExhaustInteractableId = null;
        }

        awaitingClose = true;
        currentVisibleOptions.Clear();

        if (messageText != null)
            messageText.text = "Conversation ended.";

        if (optionsText != null)
            optionsText.text = "";

        if (hintText != null)
            hintText.text = "Press Escape to close and resume play.";
    }

    private void ShowDialogue(DialogueNode node, List<DialogueOption> visibleOptions)
    {
        ShowPanel();

        if (speakerText != null)
            speakerText.text = node.speaker;

        if (messageText != null)
            messageText.text = node.message;

        if (optionsText != null)
        {
            if (visibleOptions.Count == 0)
            {
                optionsText.text = "";
            }
            else
            {
                string optionsDisplay = "";
                for (int i = 0; i < visibleOptions.Count; i++)
                {
                    optionsDisplay += (i + 1) + ". " + visibleOptions[i].optionText + "\n";
                }
                optionsText.text = optionsDisplay.TrimEnd();
            }
        }

        if (hintText != null)
        {
            if (visibleOptions.Count > 0)
                hintText.text = "Press 1-" + visibleOptions.Count + " to choose. Trust filters options.";
            else
                hintText.text = "Press Space or Enter to continue.";
        }

        Debug.Log("SHOW DIALOGUE UI — Speaker: " + node.speaker);
    }

    private void CloseDialogue()
    {
        awaitingClose = false;
        runner = null;
        currentVisibleOptions.Clear();
        HideDialogue();
    }

    private void HideDialogue()
    {
        HidePanel();
        ExitDialogueMode();
    }

    private void EnterDialogueMode()
    {
        isActive = true;

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        playerController?.SetDialogueLocked(true);

        Debug.Log("ENTER DIALOGUE MODE");
    }

    private void ExitDialogueMode()
    {
        isActive = false;
        playerController?.SetDialogueLocked(false);
        Debug.Log("EXIT DIALOGUE MODE");
    }

    private void ShowPanel()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            Debug.Log("DialogueUI: panel shown");
        }
        else
        {
            Debug.LogError("DialogueUI: dialoguePanel is null — UI was not built.");
        }
    }

    private void HidePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void EnsureUIReferences()
    {
        if (dialoguePanel == null)
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
        canvas.sortingOrder = 1000;

        if (dialoguePanel == null)
        {
            dialoguePanel = new GameObject("DialoguePanel");
            dialoguePanel.transform.SetParent(transform, false);

            RectTransform panelRect = dialoguePanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image panelImage = dialoguePanel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.92f);
        }

        speakerText = CreateText("SpeakerText", dialoguePanel.transform, 48, FontStyles.Bold,
            new Vector2(0.08f, 0.82f), new Vector2(0.92f, 0.95f), TextAlignmentOptions.TopLeft);

        messageText = CreateText("MessageText", dialoguePanel.transform, 32, FontStyles.Normal,
            new Vector2(0.08f, 0.42f), new Vector2(0.92f, 0.8f), TextAlignmentOptions.TopLeft);

        optionsText = CreateText("OptionsText", dialoguePanel.transform, 26, FontStyles.Normal,
            new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.4f), TextAlignmentOptions.TopLeft);

        hintText = CreateText("HintText", dialoguePanel.transform, 22, FontStyles.Italic,
            new Vector2(0.08f, 0.02f), new Vector2(0.92f, 0.1f), TextAlignmentOptions.BottomLeft);
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

    private static bool WasNumberPressed(int number)
    {
        if (Keyboard.current == null)
            return false;

        switch (number)
        {
            case 1: return Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame;
            case 2: return Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame;
            case 3: return Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame;
            case 4: return Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame;
            case 5: return Keyboard.current.digit5Key.wasPressedThisFrame || Keyboard.current.numpad5Key.wasPressedThisFrame;
            case 6: return Keyboard.current.digit6Key.wasPressedThisFrame || Keyboard.current.numpad6Key.wasPressedThisFrame;
            case 7: return Keyboard.current.digit7Key.wasPressedThisFrame || Keyboard.current.numpad7Key.wasPressedThisFrame;
            case 8: return Keyboard.current.digit8Key.wasPressedThisFrame || Keyboard.current.numpad8Key.wasPressedThisFrame;
            case 9: return Keyboard.current.digit9Key.wasPressedThisFrame || Keyboard.current.numpad9Key.wasPressedThisFrame;
            default: return false;
        }
    }

    private static bool WasContinuePressed()
    {
        return Keyboard.current != null &&
               (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame);
    }

    private static bool WasClosePressed()
    {
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
    }
}
