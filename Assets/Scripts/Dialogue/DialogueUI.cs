using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    private const string ResourcesPrefabPath = "Dialogue/DialoguePanel";

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject worldDimmer;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Transform choicesContainer;
    [SerializeField] private TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Content")]
    [SerializeField] private DialogueCharacterRegistry characterRegistry;
    [SerializeField] private GameObject choiceRowPrefab;
    [SerializeField] private bool buildFallbackUiIfMissing = true;

    private DialogueRunner runner;
    private PlayerController playerController;
    private List<DialogueOption> currentVisibleOptions = new List<DialogueOption>();
    private readonly List<GameObject> spawnedChoiceRows = new List<GameObject>();
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

        EnsureRegistryLoaded();
        EnsureUIReferences();
        HidePanel();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static void CreateDefaultUI()
    {
        if (Instance != null)
            return;

        GameObject prefab = Resources.Load<GameObject>(ResourcesPrefabPath);
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab);
            instance.name = "DialogueSystem";

            if (Instance == null && instance.GetComponent<DialogueUI>() == null)
                instance.AddComponent<DialogueUI>();

            return;
        }

        new GameObject("DialogueSystem").AddComponent<DialogueUI>();
    }

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        CreateDefaultUI();

        if (Instance != null)
            return;

        DialogueUI existing = FindFirstObjectByType<DialogueUI>(FindObjectsInactive.Include);
        if (existing == null)
            new GameObject("DialogueSystem").AddComponent<DialogueUI>();
    }

#if UNITY_EDITOR
    public void ConfigureForEditor(
        GameObject panel,
        GameObject dimmer,
        Image portrait,
        TextMeshProUGUI speaker,
        TextMeshProUGUI message,
        Transform choices,
        TextMeshProUGUI options,
        TextMeshProUGUI hint,
        DialogueCharacterRegistry registry,
        GameObject choiceRow)
    {
        dialoguePanel = panel;
        worldDimmer = dimmer;
        portraitImage = portrait;
        speakerText = speaker;
        messageText = message;
        choicesContainer = choices;
        optionsText = options;
        hintText = hint;
        characterRegistry = registry;
        choiceRowPrefab = choiceRow;
        buildFallbackUiIfMissing = false;
    }
#endif

    public void StartDialogue(DialogueConversation conversation, string exhaustInteractableId = null)
    {
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
            if (currentVisibleOptions.Count == 1 &&
                (WasNumberPressed(1) || WasGamepadOptionPressed(0) || WasGamepadConfirmPressed()))
            {
                ChooseOption(currentVisibleOptions[0]);
                return;
            }

            for (int i = 0; i < currentVisibleOptions.Count && i < 9; i++)
            {
                if (WasNumberPressed(i + 1) || WasGamepadOptionPressed(i))
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
        ClearChoiceRows();

        if (portraitImage != null)
            portraitImage.enabled = false;

        if (speakerText != null)
            speakerText.text = "";

        if (messageText != null)
            messageText.text = "Conversation ended.";

        if (hintText != null)
            hintText.text = GetCloseHintText();
    }

    private void ShowDialogue(DialogueNode node, List<DialogueOption> visibleOptions)
    {
        ShowPanel();

        DialogueCharacterProfile profile = characterRegistry != null
            ? characterRegistry.ResolveProfile(node)
            : null;

        if (speakerText != null)
            speakerText.text = profile != null ? profile.displayName : node.speaker;

        if (messageText != null)
            messageText.text = node.message;

        UpdatePortrait(profile);
        RefreshChoices(visibleOptions);

        if (hintText != null)
        {
            if (visibleOptions.Count > 0)
                hintText.text = GetChoiceHintText(visibleOptions.Count);
            else
                hintText.text = GetContinueHintText();
        }
    }

    private void UpdatePortrait(DialogueCharacterProfile profile)
    {
        if (portraitImage == null)
            return;

        Sprite portrait = profile != null ? profile.portrait : null;
        portraitImage.sprite = portrait;
        portraitImage.enabled = portrait != null;
    }

    private void RefreshChoices(List<DialogueOption> visibleOptions)
    {
        ClearChoiceRows();

        if (choicesContainer != null && choiceRowPrefab != null)
        {
            for (int i = 0; i < visibleOptions.Count; i++)
            {
                GameObject row = Instantiate(choiceRowPrefab, choicesContainer);
                row.SetActive(true);
                spawnedChoiceRows.Add(row);

                TextMeshProUGUI label = row.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = (i + 1) + ". " + visibleOptions[i].optionText;
            }

            if (optionsText != null)
                optionsText.gameObject.SetActive(false);
            return;
        }

        if (optionsText != null)
        {
            optionsText.gameObject.SetActive(true);
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
    }

    private void ClearChoiceRows()
    {
        for (int i = spawnedChoiceRows.Count - 1; i >= 0; i--)
        {
            if (spawnedChoiceRows[i] != null)
                Destroy(spawnedChoiceRows[i]);
        }

        spawnedChoiceRows.Clear();
    }

    private void CloseDialogue()
    {
        awaitingClose = false;
        runner = null;
        currentVisibleOptions.Clear();
        ClearChoiceRows();
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
    }

    private void ExitDialogueMode()
    {
        isActive = false;
        playerController?.SetDialogueLocked(false);
    }

    private void ShowPanel()
    {
        if (worldDimmer != null)
            worldDimmer.SetActive(true);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    private void HidePanel()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (worldDimmer != null)
            worldDimmer.SetActive(false);
    }

    private void EnsureRegistryLoaded()
    {
        if (characterRegistry != null)
            return;

        characterRegistry = Resources.Load<DialogueCharacterRegistry>("Dialogue/DialogueCharacterRegistry");
    }

    private void EnsureUIReferences()
    {
        if (dialoguePanel != null && speakerText != null && messageText != null)
            return;

        if (!buildFallbackUiIfMissing)
        {
            Debug.LogError("DialogueUI: UI references are missing and fallback build is disabled.");
            return;
        }

        DialogueUILayoutBuilder.BuiltReferences built = DialogueUILayoutBuilder.Build(transform);
        dialoguePanel = built.dialoguePanel;
        worldDimmer = built.worldDimmer;
        portraitImage = built.portraitImage;
        speakerText = built.speakerText;
        messageText = built.messageText;
        choicesContainer = built.choicesContainer;
        optionsText = built.optionsText;
        hintText = built.hintText;

        if (choiceRowPrefab == null)
        {
            choiceRowPrefab = Resources.Load<GameObject>("Dialogue/ChoiceRow");
            if (choiceRowPrefab == null)
            {
                choiceRowPrefab = DialogueUILayoutBuilder.CreateChoiceRowPrefab();
                choiceRowPrefab.transform.SetParent(transform, false);
                choiceRowPrefab.SetActive(false);
            }
        }
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

    private static bool WasGamepadConfirmPressed()
    {
        return Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame;
    }

    private static bool WasContinuePressed()
    {
        if (Keyboard.current != null &&
            (Keyboard.current.spaceKey.wasPressedThisFrame ||
             Keyboard.current.enterKey.wasPressedThisFrame))
        {
            return true;
        }

        return WasGamepadConfirmPressed();
    }

    private static bool WasClosePressed()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            return true;

        return Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;
    }

    private static bool WasGamepadOptionPressed(int optionIndex)
    {
        if (Gamepad.current == null || optionIndex < 0 || optionIndex > 2)
            return false;

        switch (optionIndex)
        {
            case 0:
                return Gamepad.current.buttonNorth.wasPressedThisFrame;
            case 1:
                return Gamepad.current.buttonEast.wasPressedThisFrame;
            case 2:
                return Gamepad.current.buttonSouth.wasPressedThisFrame;
            default:
                return false;
        }
    }

    private static bool IsGamepadInUse()
    {
        if (Gamepad.current == null)
            return false;

        Gamepad pad = Gamepad.current;
        return pad.leftStick.ReadValue().sqrMagnitude > 0.04f ||
               pad.rightStick.ReadValue().sqrMagnitude > 0.04f ||
               pad.dpad.ReadValue().sqrMagnitude > 0.04f ||
               pad.buttonSouth.wasPressedThisFrame ||
               pad.buttonEast.wasPressedThisFrame ||
               pad.buttonWest.wasPressedThisFrame ||
               pad.buttonNorth.wasPressedThisFrame ||
               pad.startButton.wasPressedThisFrame ||
               pad.selectButton.wasPressedThisFrame;
    }

    private static string GetContinueHintText()
    {
        return IsGamepadInUse()
            ? "Press A to continue."
            : "Press Space or Enter to continue.";
    }

    private static string GetCloseHintText()
    {
        return IsGamepadInUse()
            ? "Press B to close and resume play."
            : "Press Escape to close and resume play.";
    }

    private static string GetChoiceHintText(int optionCount)
    {
        if (IsGamepadInUse())
        {
            if (optionCount >= 3)
                return "Y: option 1  |  B: option 2  |  A: option 3";
            if (optionCount == 2)
                return "Y: option 1  |  B: option 2";
            return "Press Y or A for option 1.";
        }

        return "Press 1-" + optionCount + " to choose.";
    }
}
