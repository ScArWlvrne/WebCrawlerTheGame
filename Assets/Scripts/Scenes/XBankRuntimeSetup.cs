using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class XBankRuntimeSetup : MonoBehaviour
{
    public static XBankRuntimeSetup Instance { get; private set; }

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        new GameObject("XBankRuntimeSetup").AddComponent<XBankRuntimeSetup>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupScene(scene.name);
    }

    public void SetupScene(string sceneName)
    {
        EndgameController.EnsureExists();

        switch (sceneName)
        {
            case "DesktopHub":
                SetupDesktopHub();
                break;
            case "Browser":
                SetupBrowser();
                break;
            case "XBank":
                SetupXBank();
                break;
            case "Venom":
                SetupVenom();
                break;
            case "Email":
                SetupEmail();
                break;
        }
    }

    private void SetupDesktopHub()
    {
        bool xBankUnlocked = IsXBankUnlocked();
        if (xBankUnlocked)
        {
            if (GameObject.Find("X Bank Site Shortcut") == null)
                CreateShortcut("X Bank Site Shortcut", "XBank", new Vector3(5.2f, 0.7f, 0.5f), "X Bank", GameFlags.AraknydFinaleUnlocked);

            if (GameObject.Find("X Bank Email Shortcut") == null)
                CreateShortcut("X Bank Email Shortcut", "Email", new Vector3(5.2f, 0.7f, -0.1f), "Email", GameFlags.AraknydFinaleUnlocked);
        }
        else
        {
            DestroyIfExists("X Bank Site Shortcut");
            DestroyIfExists("X Bank Email Shortcut");
        }

        GameObject journalObject = GameObject.Find("Journal");
        if (journalObject != null && journalObject.GetComponent<JournalOpenInteractable>() == null)
        {
            EnsureCollider(journalObject);
            journalObject.layer = 3;
            journalObject.AddComponent<JournalOpenInteractable>();
        }
    }

    private void SetupBrowser()
    {
        if (IsXBankUnlocked())
        {
            if (GameObject.Find("Donald Security Mother Pickup") == null)
                CreateJournalPickup("Donald Security Mother Pickup", JournalPaths.JournalOwner.CEO, "security_mother.txt", "Donald's profile references Marla Maples. X Bank security profile: mother's maiden name answer is Maples.", new Vector3(-2.2f, 1f, 1.6f));

            if (GameObject.Find("Donald Security Pet Pickup") == null)
                CreateJournalPickup("Donald Security Pet Pickup", JournalPaths.JournalOwner.CEO, "security_pet.txt", "Donald keeps posting about his childhood dog, Titan. X Bank first-pet answer: Titan.", new Vector3(-2.2f, 1f, 2.0f));

            if (GameObject.Find("Donald Birthday Phone Pickup") == null)
                CreateJournalPickup("Donald Birthday Phone Pickup", JournalPaths.JournalOwner.CEO, "profile_osint.txt", "Donald Musk: birthday 6/28/1971, phone (956)-123-4567, email donald@fakegmail.com.", new Vector3(-2.2f, 1f, 2.4f));
        }

        AddExitShortcut(new Vector3(2.4f, 1f, -1.6f));
    }

    private void SetupXBank()
    {
        EnsureXBankDirectPlayState();
        EnsureXBankRoom();
        EnsureRuntimePlayer();

        if (GameObject.Find("X Bank Browser Gate") == null)
        {
            GameObject gate = new GameObject("X Bank Browser Gate");
            gate.transform.position = new Vector3(0f, 1f, 0f);
            gate.AddComponent<BrowserXBankGate>();
            CreateWorldText("X Bank Executive Portal", gate.transform, new Vector3(-1.6f, 1.55f, 2.85f), 7f);
        }

        if (GameObject.Find("X Bank Web Inspector") == null)
        {
            GameObject inspector = CreateInteractableCube("X Bank Web Inspector", new Vector3(0f, 1f, 2.75f), new Vector3(0.55f, 0.15f, 0.25f));
            inspector.AddComponent<DialogueTrigger>().Configure(DialogueTrigger.ConversationPreset.WebInspectorXBank, "xbank_web_inspector", false);
            CreateWorldText("Web Inspector\nX Bank source", inspector.transform, new Vector3(-0.2f, 0.25f, 0f), 4f);
        }

        AddExitShortcut(new Vector3(2.4f, 1f, -1.6f));
    }

    private void EnsureXBankDirectPlayState()
    {
        if (GameStateManager.Instance == null)
            return;

        if (!GameStateManager.Instance.GetFlag(GameFlags.AraknydFinaleUnlocked))
        {
            GameStateManager.Instance.SetFlag(GameFlags.AraknydFinaleUnlocked, true);
            SeedDirectPlayXBankIntel();
        }

        GameStateManager.Instance.SaveGame();
    }

    private void SeedDirectPlayXBankIntel()
    {
        AddJournalFileIfMissing(JournalPaths.Build(JournalPaths.XBank, "username_hint.txt"),
            "Donald's X Bank executive username: dmusk1971. Haley Delgado is handling the reset.");

        AddJournalFileIfMissing(JournalPaths.Build(JournalPaths.CEO, "temp_password.txt"),
            "Haley's password reset clue points to Donald's reused temporary password: Araknyd628!");

        AddJournalFileIfMissing(JournalPaths.Build(JournalPaths.CEO, "security_mother.txt"),
            "Donald's profile references Marla Maples. X Bank security profile: mother's maiden name answer is Maples.");

        AddJournalFileIfMissing(JournalPaths.Build(JournalPaths.CEO, "security_pet.txt"),
            "Donald keeps posting about his childhood dog, Titan. X Bank first-pet answer: Titan.");
    }

    private void AddJournalFileIfMissing(string path, string content)
    {
        if (GameStateManager.Instance != null &&
            !GameStateManager.Instance.HasJournalFile(path))
        {
            GameStateManager.Instance.AddJournalFile(path, content);
        }
    }

    private void SetupVenom()
    {
        if (!IsXBankUnlocked())
        {
            DestroyIfExists("Haley Venom Conversation");
            AddExitShortcut(new Vector3(1.6f, 1f, -1.2f));
            return;
        }

        if (GameObject.Find("Haley Venom Conversation") == null)
        {
            GameObject haley = CreateInteractableCube("Haley Venom Conversation", new Vector3(0f, 1f, 1.4f), new Vector3(0.75f, 0.12f, 0.25f));
            haley.AddComponent<DialogueTrigger>().Configure(DialogueTrigger.ConversationPreset.HaleyXBank, "haley_xbank_venom", false);
            CreateWorldText("Venom DM\nHaley Delgado", haley.transform, new Vector3(-0.25f, 0.25f, 0f), 4f);
        }

        AddExitShortcut(new Vector3(1.6f, 1f, -1.2f));
    }

    private void SetupEmail()
    {
        SetSceneObjectActive("Emails", false);

        if (!IsXBankUnlocked())
        {
            if (GameObject.Find("X Bank Email Locked Runtime") == null)
            {
                GameObject lockedRoot = new GameObject("X Bank Email Locked Runtime");
                CreateWorldText("Email is locked until the Araknyd investigation is complete.", lockedRoot.transform, new Vector3(-1.4f, 1.5f, 1.8f), 4f);
            }

            AddExitShortcut(new Vector3(1.8f, 1f, -1.2f));
            return;
        }

        DestroyIfExists("X Bank Email Locked Runtime");

        if (GameObject.Find("X Bank Email Runtime") != null)
            return;

        GameObject root = new GameObject("X Bank Email Runtime");
        TMP_Text bodyText = CreateWorldText("Select an email.", root.transform, new Vector3(-1.4f, 1.5f, 1.8f), 4f);
        bodyText.rectTransform.sizeDelta = new Vector2(40f, 24f);

        CreateEmail(root.transform, bodyText, 0, "Lily Pad - SOC2 audit failed",
            "Donald,\n\nX Bank's audit team arrives next week. They will ask about data security, password reuse, and the Meridian export work. Please finish the questionnaire before Haley has to do it for you.",
            JournalPaths.Build(JournalPaths.CEO, "audit_memo.txt"),
            "Lily warned Donald that X Bank's audit team is coming and will ask about data security, password reuse, and Meridian export work.");

        CreateEmail(root.transform, bodyText, 1, "IT ticket #123456 - password leaked",
            "Boss,\n\nWe got another ticket from your assistant. A company password leaked again. Please stop using the same one you use for everything.",
            JournalPaths.Build(JournalPaths.CEO, "password_habit.txt"),
            "Donald reuses passwords across important systems. IT ticket #123456 says a company password leaked and he uses the same one everywhere.");

        CreateEmail(root.transform, bodyText, 2, "Haley Delgado - X Bank username",
            "Sir,\n\nX Bank says your executive username is still dmusk1971. You have two days to complete the reset before I do it for you.",
            JournalPaths.Build(JournalPaths.XBank, "username_hint.txt"),
            "Donald's X Bank executive username: dmusk1971. Haley Delgado is handling the reset.");

        CreateEmail(root.transform, bodyText, 3, "X Bank security alert",
            "Donald Musk,\n\nWe detected a failed login to online.xbank.com/executive. Your reset link expired. Security questions remain active.",
            JournalPaths.Build(JournalPaths.XBank, "security_reset.txt"),
            "X Bank executive portal uses active security questions even when the reset link expires.");

        CreateEmail(root.transform, bodyText, 4, "Forwarded: Meridian wire confirmation",
            "Forwarded receipt: $2.4M recurring wire to DataVault Solutions / Meridian Corp. Memo: Araknyd consulting fees.",
            JournalPaths.Build(JournalPaths.CEO, "meridian_wire.txt"),
            "Forwarded wire confirmation: $2.4M recurring payment to DataVault Solutions / Meridian Corp for Araknyd consulting fees.",
            JournalPaths.Build(JournalPaths.XBank, "username_hint.txt"));

        AddExitShortcut(new Vector3(1.8f, 1f, -1.2f));
    }

    private bool IsXBankUnlocked()
    {
        return GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(GameFlags.AraknydFinaleUnlocked);
    }

    private void EnsureRuntimePlayer()
    {
        if (GameObject.Find("Runtime Player") == null)
            CreateRuntimePlayer();

        if (Camera.main == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            GameObject player = GameObject.Find("Runtime Player");
            cameraObject.transform.position = new Vector3(0f, 12f, -9.8f);
            cameraObject.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            CameraFollow follow = cameraObject.AddComponent<CameraFollow>();
            if (player != null)
                follow.target = player.transform;
        }

        if (GameObject.Find("X Bank Runtime Light") == null)
        {
            GameObject lightObject = new GameObject("X Bank Runtime Light");
            Light sceneLight = lightObject.AddComponent<Light>();
            sceneLight.type = LightType.Directional;
            sceneLight.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }

    private void CreateRuntimePlayer()
    {
        GameObject player = InstantiatePlayerPrefab();
        if (player == null)
            player = CreateFallbackSpiderPlayer();

        player.name = "Runtime Player";
        player.transform.position = new Vector3(0f, 0.1f, -1.8f);
        player.transform.rotation = Quaternion.identity;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller == null)
            controller = player.AddComponent<PlayerController>();

        controller.ConfigureRuntime(1 << 3);
    }

    private GameObject InstantiatePlayerPrefab()
    {
#if UNITY_EDITOR
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        if (prefab != null)
            return Instantiate(prefab);
#endif
        return null;
    }

    private GameObject CreateFallbackSpiderPlayer()
    {
        GameObject player = new GameObject("Runtime Player");
        CharacterController characterController = player.AddComponent<CharacterController>();
        characterController.center = new Vector3(0f, 0.45f, 0f);
        characterController.height = 0.9f;
        characterController.radius = 0.35f;

        GameObject body = CreatePrimitiveChild(player.transform, "Fallback Spider Body", PrimitiveType.Sphere, new Vector3(0f, 0.42f, 0f), new Vector3(0.9f, 0.35f, 1.15f));
        GameObject head = CreatePrimitiveChild(player.transform, "Fallback Spider Head", PrimitiveType.Sphere, new Vector3(0f, 0.45f, 0.62f), new Vector3(0.52f, 0.34f, 0.42f));
        body.layer = 0;
        head.layer = 0;

        for (int i = 0; i < 4; i++)
        {
            float z = -0.35f + (i * 0.24f);
            CreatePrimitiveChild(player.transform, "Fallback Spider Leg L" + i, PrimitiveType.Cube, new Vector3(-0.55f, 0.35f, z), new Vector3(0.85f, 0.08f, 0.08f));
            CreatePrimitiveChild(player.transform, "Fallback Spider Leg R" + i, PrimitiveType.Cube, new Vector3(0.55f, 0.35f, z), new Vector3(0.85f, 0.08f, 0.08f));
        }

        return player;
    }

    private GameObject CreatePrimitiveChild(Transform parent, string objectName, PrimitiveType primitiveType, Vector3 localPosition, Vector3 localScale)
    {
        GameObject child = GameObject.CreatePrimitive(primitiveType);
        child.name = objectName;
        child.transform.SetParent(parent, false);
        child.transform.localPosition = localPosition;
        child.transform.localScale = localScale;

        Collider collider = child.GetComponent<Collider>();
        if (collider != null)
            Destroy(collider);

        return child;
    }

    private void EnsureXBankRoom()
    {
        if (GameObject.Find("X Bank Runtime Floor") == null)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "X Bank Runtime Floor";
            floor.transform.position = new Vector3(0f, -0.08f, 1.4f);
            floor.transform.localScale = new Vector3(7f, 0.1f, 7f);
        }

        if (GameObject.Find("X Bank Runtime Back Wall") == null)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "X Bank Runtime Back Wall";
            wall.transform.position = new Vector3(0f, 1.5f, 3.25f);
            wall.transform.localScale = new Vector3(6f, 3f, 0.1f);
        }
    }

    private void CreateShortcut(string objectName, string sceneName, Vector3 position, string label, string requiredFlag = null)
    {
        GameObject shortcut = CreateInteractableCube(objectName, position, new Vector3(0.7f, 0.12f, 0.45f));
        shortcut.AddComponent<SceneLoadInteractable>().Configure(sceneName, requiredFlag);
        CreateWorldText(label, shortcut.transform, new Vector3(-0.22f, 0.22f, 0f), 5f);
    }

    private void AddExitShortcut(Vector3 position)
    {
        if (GameObject.Find("Exit To Desktop Runtime") != null)
            return;

        CreateShortcut("Exit To Desktop Runtime", "DesktopHub", position, "Exit To Desktop");
    }

    private void CreateEmail(Transform parent, TMP_Text bodyText, int index, string subject, string body, string journalPath, string journalContent, string requiredFile = null)
    {
        GameObject row = CreateInteractableCube("Email - " + subject, new Vector3(-2f, 1.2f - (index * 0.32f), 1.2f), new Vector3(1.4f, 0.08f, 0.22f));
        row.transform.SetParent(parent, true);
        row.AddComponent<EmailMessageInteractable>().Configure(bodyText, subject, body, journalPath, journalContent, requiredFile);
        CreateWorldText(subject, row.transform, new Vector3(-0.45f, 0.16f, 0f), 3f);
    }

    private void CreateJournalPickup(string objectName, JournalPaths.JournalOwner owner, string fileName, string content, Vector3 position)
    {
        GameObject pickup = CreateInteractableCube(objectName, position, new Vector3(0.6f, 0.1f, 0.2f));
        pickup.AddComponent<JournalEntryInteractable>().Configure(owner, fileName, content, objectName, false);
        CreateWorldText(fileName, pickup.transform, new Vector3(-0.25f, 0.18f, 0f), 3f);
    }

    private GameObject CreateInteractableCube(string objectName, Vector3 position, Vector3 scale)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = objectName;
        obj.layer = 3;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        return obj;
    }

    private TMP_Text CreateWorldText(string text, Transform parent, Vector3 localPosition, float fontSize)
    {
        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(parent, false);
        textObject.transform.localPosition = localPosition;
        textObject.transform.localScale = Vector3.one * 0.04f;
        TextMeshPro tmp = textObject.AddComponent<TextMeshPro>();
        tmp.fontSize = fontSize;
        tmp.text = text;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.rectTransform.sizeDelta = new Vector2(30f, 10f);
        return tmp;
    }

    private void EnsureCollider(GameObject obj)
    {
        if (obj.GetComponent<Collider>() == null)
            obj.AddComponent<BoxCollider>();
    }

    private void DestroyIfExists(string objectName)
    {
        GameObject existing = GameObject.Find(objectName);
        if (existing != null)
            Destroy(existing);
    }

    private void SetSceneObjectActive(string objectName, bool active)
    {
        GameObject existing = GameObject.Find(objectName);
        if (existing != null)
            existing.SetActive(active);
    }
}
