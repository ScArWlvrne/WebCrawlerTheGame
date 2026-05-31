using System.Collections.Generic;
using UnityEngine;

public class DialogueRunnerTester : MonoBehaviour
{
    private void Start()
    {
        DialogueConversation testConversation = DialogueConversationFactory.GetLilyTestConversation();

        RunConversationTest(testConversation, 20, "LOW TRUST");
        RunConversationTest(testConversation, 50, "MEDIUM TRUST");
        RunConversationTest(testConversation, 70, "HIGH TRUST");
        RunAraknydCrawlerTest();
    }

    private void RunConversationTest(DialogueConversation conversation, int trustValue, string label)
    {
        Debug.Log("==============================");
        Debug.Log("DIALOGUE TEST: " + label);
        Debug.Log("Starting Lily trust: " + trustValue);

        GameStateManager.Instance.SetTrust(GameCharacters.Lily, trustValue);

        DialogueRunner runner = new DialogueRunner();
        runner.StartConversation(conversation);

        LogCurrentNode(runner);

        List<DialogueOption> visibleOptions = runner.GetVisibleOptions();

        Debug.Log("Visible option count: " + visibleOptions.Count);

        for (int i = 0; i < visibleOptions.Count; i++)
        {
            Debug.Log("Option " + i + ": " + visibleOptions[i].optionText);
        }

        if (visibleOptions.Count == 0)
        {
            Debug.LogWarning("No visible options. Ending test early.");
            return;
        }

        DialogueOption chosenOption = visibleOptions[visibleOptions.Count - 1];

        Debug.Log("Chosen option: " + chosenOption.optionText);

        runner.ChooseOption(chosenOption);

        LogCurrentNode(runner);

        runner.Continue();

        LogCurrentNode(runner);

        runner.Continue();

        Debug.Log("Conversation over: " + runner.IsConversationOver());
        Debug.Log("END TEST: " + label);
        Debug.Log("==============================");
    }

    private void LogCurrentNode(DialogueRunner runner)
    {
        if (runner.CurrentNode == null)
        {
            Debug.Log("Current node: null");
            return;
        }

        Debug.Log("Current speaker: " + runner.CurrentNode.speaker);
        Debug.Log("Current message: " + runner.CurrentNode.message);
    }

    private void RunAraknydCrawlerTest()
    {
        string urlsPath = JournalPaths.Build(JournalPaths.Araknyd, "urls.txt");
        string robotsPath = JournalPaths.Build(JournalPaths.Araknyd, "robots.txt");
        string blogPath = JournalPaths.Build(JournalPaths.Araknyd, "blog_comment.txt");

        ResetAraknydCrawlerState(urlsPath, robotsPath, blogPath);

        DialogueConversation crawlerConversation = DialogueConversationFactory.GetAraknydCrawlerTestConversation();
        DialogueRunner runner = new DialogueRunner();
        runner.StartConversation(crawlerConversation);

        Debug.Log("==============================");
        Debug.Log("DIALOGUE TEST: ARAKNYD CRAWLER");
        LogCurrentNode(runner);
        LogVisibleOptions(runner);

        ChooseFirstOptionContaining(runner, "crawl https://www.araknyd.io");
        LogCurrentNode(runner);
        LogVisibleOptions(runner);

        ChooseFirstOptionContaining(runner, "robots.txt");
        LogCurrentNode(runner);
        Debug.Log("Robots journal file exists: " + GameStateManager.Instance.HasJournalFile(robotsPath));
        LogVisibleOptions(runner);

        ChooseFirstOptionContaining(runner, "sitemap.xml");
        LogCurrentNode(runner);
        LogVisibleOptions(runner);

        ChooseFirstOptionContaining(runner, "admin-beta");
        LogCurrentNode(runner);
        Debug.Log("Admin beta discovered flag: " + GameStateManager.Instance.GetFlag(GameFlags.AraknydAdminBetaDiscovered));
        Debug.Log("Araknyd URL journal file before write: " + GameStateManager.Instance.HasJournalFile(urlsPath));
        LogVisibleOptions(runner);

        ChooseFirstOptionContaining(runner, "journal");
        LogCurrentNode(runner);
        Debug.Log("Araknyd URL journal file exists: " + GameStateManager.Instance.HasJournalFile(urlsPath));
        Debug.Log("Araknyd URL journal content: " + GameStateManager.Instance.GetJournalFileContent(urlsPath));

        runner.Continue();
        Debug.Log("Conversation over: " + runner.IsConversationOver());
        Debug.Log("END TEST: ARAKNYD CRAWLER");
        Debug.Log("==============================");
    }

    private void ResetAraknydCrawlerState(string urlsPath, string robotsPath, string blogPath)
    {
        GameStateManager.Instance.SetFlag(GameFlags.AraknydHomeCrawled, false);
        GameStateManager.Instance.SetFlag(GameFlags.AraknydRobotsCrawled, false);
        GameStateManager.Instance.SetFlag(GameFlags.AraknydAdminBetaDiscovered, false);
        GameStateManager.Instance.SetFlag(GameFlags.JournalUrlsAraknydUpdated, false);
        GameStateManager.Instance.RemoveJournalFile(urlsPath);
        GameStateManager.Instance.RemoveJournalFile(robotsPath);
        GameStateManager.Instance.RemoveJournalFile(blogPath);
    }

    private void LogVisibleOptions(DialogueRunner runner)
    {
        List<DialogueOption> visibleOptions = runner.GetVisibleOptions();

        Debug.Log("Visible option count: " + visibleOptions.Count);

        for (int i = 0; i < visibleOptions.Count; i++)
        {
            Debug.Log("Option " + i + ": " + visibleOptions[i].optionText);
        }
    }

    private void ChooseFirstOptionContaining(DialogueRunner runner, string text)
    {
        List<DialogueOption> visibleOptions = runner.GetVisibleOptions();

        foreach (DialogueOption option in visibleOptions)
        {
            if (option.optionText.Contains(text))
            {
                Debug.Log("Chosen option: " + option.optionText);
                runner.ChooseOption(option);
                return;
            }
        }

        Debug.LogWarning("No visible option containing: " + text);
    }
}