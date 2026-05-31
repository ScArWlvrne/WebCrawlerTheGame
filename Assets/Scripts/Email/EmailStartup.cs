using TMPro;
using UnityEngine;

/// <summary>
/// Bootstraps the Email view scene: persistent services, cursor input, and runtime wiring of interactables.
/// </summary>
[DefaultExecutionOrder(-50)]
public class EmailStartup : MonoBehaviour
{
    [SerializeField] private string closeButtonName = "Close";
    [SerializeField] private string venomInboxRowName = "From venom";
    [SerializeField] private string venomBodyPanelName = "page (1)";
    [SerializeField] private string returnSceneName = "DesktopHub";
    [SerializeField] private GameObject emailBodyPanelOverride;
    [SerializeField] private TMP_Text emailBodyTextOverride;

    private void Awake()
    {
        EnsureServices();
        EnsureCursorController();
        EnsureCircleTransition();
        WireCloseButton();
        WireVenomInboxEmail();
        HideEmailBodyUntilOpened();
    }

    private void EnsureServices()
    {
        if (GetComponent<GameBootstrap>() == null)
            gameObject.AddComponent<GameBootstrap>();
        else
            GetComponent<GameBootstrap>().EnsureAllSystems();
    }

    private void EnsureCursorController()
    {
        if (GetComponent<ViewCursorController>() == null)
            gameObject.AddComponent<ViewCursorController>();
    }

    private CircleHoleTransition EnsureCircleTransition()
    {
        return FindFirstObjectByType<CircleHoleTransition>();
    }

    private void WireCloseButton()
    {
        GameObject closeObject = GameObject.Find(closeButtonName);
        if (closeObject == null)
        {
            Debug.LogWarning("EmailStartup: Close button not found.");
            return;
        }

        SceneLoadInteractable loader = closeObject.GetComponent<SceneLoadInteractable>();
        if (loader == null)
            loader = closeObject.AddComponent<SceneLoadInteractable>();

        loader.Configure(returnSceneName, EnsureCircleTransition(), closeObject.transform);
    }

    private void WireVenomInboxEmail()
    {
        GameObject venomRowObject = GameObject.Find(venomInboxRowName);
        if (venomRowObject == null)
        {
            Debug.LogWarning("EmailStartup: Venom inbox row not found.");
            return;
        }

        Transform venomRow = venomRowObject.transform;
        EnsureCollider(venomRowObject);

        EmailTempPasswordMessageInteractable message =
            venomRowObject.GetComponent<EmailTempPasswordMessageInteractable>();
        if (message == null)
            message = venomRowObject.AddComponent<EmailTempPasswordMessageInteractable>();

        GameObject bodyPanel = ResolveBodyPanel(venomRow);
        TMP_Text bodyText = ResolveBodyText(venomRow, bodyPanel);
        message.Configure(bodyPanel, bodyText, venomRow);
    }

    private GameObject ResolveBodyPanel(Transform venomRow)
    {
        if (emailBodyPanelOverride != null)
            return emailBodyPanelOverride;

        Transform body = venomRow.Find(venomBodyPanelName);
        return body != null ? body.gameObject : null;
    }

    private TMP_Text ResolveBodyText(Transform venomRow, GameObject bodyPanel)
    {
        if (emailBodyTextOverride != null)
            return emailBodyTextOverride;

        if (bodyPanel != null)
            return bodyPanel.GetComponent<TMP_Text>();

        return venomRow.GetComponentInChildren<TMP_Text>(true);
    }

    private void HideEmailBodyUntilOpened()
    {
        GameObject venomRowObject = GameObject.Find(venomInboxRowName);
        if (venomRowObject == null)
            return;

        GameObject bodyPanel = ResolveBodyPanel(venomRowObject.transform);
        if (bodyPanel != null)
            bodyPanel.SetActive(false);
    }

    private static void EnsureCollider(GameObject target)
    {
        if (target.GetComponent<Collider>() != null)
            return;

        BoxCollider box = target.AddComponent<BoxCollider>();
        box.size = new Vector3(2f, 0.5f, 0.5f);
        box.center = Vector3.zero;
    }
}
