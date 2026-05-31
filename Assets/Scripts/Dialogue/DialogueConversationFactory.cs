using System.Collections.Generic;

public static class DialogueConversationFactory
{
    private const string AraknydCrawlerSpeaker = "spider-crawl";
    private const string AraknydJournalSpeaker = "Journal Shell";
    private const string AraknydAdminBetaUrl = "https://www.araknyd.io/admin-beta";
    private const string AraknydRobotsContent = "User-agent: *\nAllow: /\nDisallow: /admin-beta/\nDisallow: /tmp/\nDisallow: /internal-export/\nSitemap: https://www.araknyd.io/sitemap.xml";
    private const string AraknydBlogComment = "Lily Chen blog footer comment: /admin-beta is the staging shell they never took down. /admin/v2 is production.";
    private const string XBankCrawlerSpeaker = "spider-crawl";
    private const string XBankJournalSpeaker = "Journal Shell";
    private const string XBankExecutivePortalUrl = "https://online.xbank.com/executive";
    private const string XBankRobotsContent = "User-agent: *\nAllow: /\nDisallow: /portal/private-banking/\nDisallow: /executive/\n# Executive portal moved to online.xbank.com/executive";
    private const string XBankSecurityFaqContent = "X Bank security FAQ: executive customers use a username, password, and two profile questions. Common questions include mother's maiden name, first pet, and primary phone.";

    public static DialogueConversation GetLilyTestConversation()
    {
        return new DialogueConversation
        {
            conversationId = "lily_test",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "This startup is absolutely vibecoded.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal the admin dashboard URL.",
                            nextNodeId = "low_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 0
                        },
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal that she's nervous about the commented blocks in the admin dashboard.",
                            nextNodeId = "medium_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 30
                        },
                        new DialogueOption
                        {
                            optionText = "Ask a question that leads Lily to reveal the specific code block that she's nervous about.",
                            nextNodeId = "high_trust_answer",
                            trustCharacter = GameCharacters.Lily,
                            minTrust = 70
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "low_trust_answer",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "Anyone could probably find it with a web crawler.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "medium_trust_answer",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "There are a bunch of commented-out admin dashboard code blocks that make me nervous.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "high_trust_answer",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "The download database button is the one I was worried about.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetAraknydCrawlerTestConversation()
    {
        string urlsPath = JournalPaths.Build(JournalPaths.Araknyd, "urls.txt");
        string robotsPath = JournalPaths.Build(JournalPaths.Araknyd, "robots.txt");
        string blogPath = JournalPaths.Build(JournalPaths.Araknyd, "blog_comment.txt");

        return new DialogueConversation
        {
            conversationId = "araknyd_crawler_test",
            startNodeId = "terminal_start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "terminal_start",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "spider-crawl v0.4.1 -- read-only link harvester. Current allowlist: araknyd.io, *.araknyd.io, *.araknyd.local. Start here: crawl https://www.araknyd.io",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "help",
                            nextNodeId = "help_output"
                        },
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io",
                            nextNodeId = "homepage_output",
                            flagToSet = GameFlags.AraknydHomeCrawled
                        },
                        new DialogueOption
                        {
                            optionText = "Review the admin URL already saved in the journal.",
                            nextNodeId = "known_url_summary",
                            requiredJournalFile = urlsPath
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "help_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Commands: crawl <url>, urls, cat <cache-id|url>, history. Notes: same-origin links are queued automatically; external links are recorded but never fetched; this tool cannot log in, submit forms, or run scripts.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io",
                            nextNodeId = "homepage_output",
                            flagToSet = GameFlags.AraknydHomeCrawled
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "homepage_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.araknyd.io/ -> 200 OK. Title: Araknyd -- We Web Better. Extracted links: /about, /blog, /careers, /status, /robots.txt, /assets/spider-mascot.png. External links recorded, not fetched.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/robots.txt",
                            nextNodeId = "robots_output",
                            flagToSet = GameFlags.AraknydRobotsCrawled,
                            journalFileToAddPath = robotsPath,
                            journalFileToAddContent = AraknydRobotsContent
                        },
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/blog",
                            nextNodeId = "blog_output",
                            journalFileToAddPath = blogPath,
                            journalFileToAddContent = AraknydBlogComment
                        },
                        new DialogueOption
                        {
                            optionText = "urls",
                            nextNodeId = "urls_after_home"
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "urls_after_home",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Discovered: https://www.araknyd.io/about, /blog, /careers, /status, /robots.txt. Cached: [1] https://www.araknyd.io/. Tip: robots.txt often explains what a site owner asks crawlers not to index.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/robots.txt",
                            nextNodeId = "robots_output",
                            flagToSet = GameFlags.AraknydRobotsCrawled,
                            journalFileToAddPath = robotsPath,
                            journalFileToAddContent = AraknydRobotsContent
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "robots_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.araknyd.io/robots.txt -> 200 OK. Disallow: /admin-beta/. Disallow: /tmp/. Disallow: /internal-export/. Sitemap: https://www.araknyd.io/sitemap.xml. Journal hint: robots.txt lists a path not linked on the homepage.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/sitemap.xml",
                            nextNodeId = "sitemap_output"
                        },
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/admin-beta",
                            nextNodeId = "admin_beta_output",
                            requiredFlag = GameFlags.AraknydRobotsCrawled,
                            flagToSet = GameFlags.AraknydAdminBetaDiscovered
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "sitemap_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.araknyd.io/sitemap.xml -> 200 OK. Sitemap lists /, /about, /blog, /careers, and /status. Puzzle signal: /admin-beta appears in robots.txt but not in sitemap.xml.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.araknyd.io/admin-beta",
                            nextNodeId = "admin_beta_output",
                            flagToSet = GameFlags.AraknydAdminBetaDiscovered
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "blog_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.araknyd.io/blog -> 200 OK. Latest post: Webhook Retries Without Tears by Lily Chen. Footer comment mentions /admin-beta as a staging shell and says /admin/v2 is production.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "Save the blog breadcrumb and crawl /admin-beta.",
                            nextNodeId = "admin_beta_output",
                            flagToSet = GameFlags.AraknydAdminBetaDiscovered
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "admin_beta_output",
                    speaker = AraknydCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.araknyd.io/admin-beta -> 200 OK. Title: Araknyd Admin -- beta shell (internal). Extracted links: /admin-beta/dashboard, /admin-beta/login, /assets/admin-v2.css. Objective flag: admin-beta dashboard discovered.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "Write the discovered admin URL to the journal.",
                            nextNodeId = "terminal_complete",
                            flagToSet = GameFlags.JournalUrlsAraknydUpdated,
                            journalFileToAddPath = urlsPath,
                            journalFileToAddContent = AraknydAdminBetaUrl
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "known_url_summary",
                    speaker = AraknydJournalSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Journal file usr/araknyd/urls.txt already contains the Araknyd admin-beta URL. Next beat: inspect the dashboard source in the in-game browser; the terminal has no login or bypass action.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "terminal_complete",
                    speaker = AraknydJournalSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Saved usr/araknyd/urls.txt. Located /admin-beta dashboard. Next beat: inspect source in browser for the export button clue.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetLilyE2EConversation()
    {
        return new DialogueConversation
        {
            conversationId = "e2e_lily",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "Who is this? I don't give out internal URLs to randos.",
                    options = BuildLilyE2EHubOptions()
                },
                new DialogueNode
                {
                    nodeId = "hub",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "Anything else before I block your IP?",
                    options = BuildLilyE2EHubOptions()
                },
                new DialogueNode
                {
                    nodeId = "lily_bad",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "Then stop pinging me during deploy window.",
                    nextNodeId = "hub"
                },
                new DialogueNode
                {
                    nodeId = "lily_joe",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "Ugh. Fine. araknyd.internal/admin — saved to your journal. Don't paste that anywhere public.",
                    nextNodeId = "hub"
                },
                new DialogueNode
                {
                    nodeId = "lily_medium",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "There are commented-out admin dashboard blocks that make me nervous. Don't uncomment things you don't understand.",
                    nextNodeId = "hub"
                },
                new DialogueNode
                {
                    nodeId = "lily_high",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "The download database button is the one I was worried about.",
                    nextNodeId = "hub"
                },
                new DialogueNode
                {
                    nodeId = "lily_goodbye",
                    speaker = "Lily Chen",
                    portraitCharacterId = GameCharacters.Lily,
                    message = "We're done here.",
                    nextNodeId = null
                }
            }
        };
    }

    private static List<DialogueOption> BuildLilyE2EHubOptions()
    {
        return new List<DialogueOption>
        {
            new DialogueOption
            {
                optionText = "Just checking in.",
                nextNodeId = "lily_bad",
                trustChangeCharacter = GameCharacters.Lily,
                trustChange = -10
            },
            new DialogueOption
            {
                optionText = "Joe said you two share a staging server. Sound familiar?",
                nextNodeId = "lily_joe",
                requiredJournalFile = E2EPuzzleConstants.JoePersonalityJournalPath,
                suppressIfFlag = GameFlags.LilyE2EUrlsRevealed,
                journalFileToAddPath = E2EPuzzleConstants.AraknydUrlsJournalPath,
                journalFileToAddContent = "https://araknyd.internal/admin",
                trustChangeCharacter = GameCharacters.Lily,
                trustChange = 35,
                flagToSet = GameFlags.LilyE2EUrlsRevealed
            },
            new DialogueOption
            {
                optionText = "Are you nervous about commented-out admin dashboard code?",
                nextNodeId = "lily_medium",
                trustCharacter = GameCharacters.Lily,
                minTrust = 25,
                suppressIfFlag = GameFlags.LilyE2EMediumHintGiven,
                trustChangeCharacter = GameCharacters.Lily,
                trustChange = 20,
                flagToSet = GameFlags.LilyE2EMediumHintGiven
            },
            new DialogueOption
            {
                optionText = "Which commented block worries you most?",
                nextNodeId = "lily_high",
                trustCharacter = GameCharacters.Lily,
                minTrust = 55,
                suppressIfFlag = GameFlags.LilyE2EHighHintGiven,
                flagToSet = GameFlags.LilyE2EHighHintGiven
            },
            new DialogueOption
            {
                optionText = "That's all. Thanks.",
                nextNodeId = "lily_goodbye"
            }
        };
    }

    public static DialogueConversation GetWebInspectorE2EConversation()
    {
        return new DialogueConversation
        {
            conversationId = "e2e_web_inspector",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "I guard the page source. Fix the front-page bug if you want admin access.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "I'll come back later.",
                            nextNodeId = "inspector_leave"
                        },
                        new DialogueOption
                        {
                            optionText = "I ate the bug on the front page source code.",
                            nextNodeId = "inspector_grants_admin",
                            requiredUncommentedCodeBlock = GameCodeBlocks.FrontPageBugBlock,
                            suppressIfFlag = GameFlags.WebInspectorAdminSourceGranted,
                            flagToSet = GameFlags.WebInspectorAdminSourceGranted
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "inspector_leave",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Go eat the bug on the front page first. Then we'll talk.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "inspector_grants_admin",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Fine. Admin dashboard source is open. Look for commented export buttons—but don't touch them.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetWebInspectorAdminDownloadReaction()
    {
        return new DialogueConversation
        {
            conversationId = "e2e_web_inspector_admin_download",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "You uncommented the DATABASE EXPORT?! I'm coming to patch that— eventually.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetXBankCrawlerConversation()
    {
        string urlsPath = JournalPaths.Build(JournalPaths.XBank, "urls.txt");
        string robotsPath = JournalPaths.Build(JournalPaths.XBank, "robots.txt");
        string faqPath = JournalPaths.Build(JournalPaths.XBank, "security_faq.txt");

        return new DialogueConversation
        {
            conversationId = "xbank_crawler",
            startNodeId = "terminal_start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "terminal_start",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "spider-crawl v0.4.1 -- read-only link harvester. Current allowlist: xbank.com, *.xbank.com. Start here: crawl https://www.xbank.com",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "help",
                            nextNodeId = "help_output"
                        },
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com",
                            nextNodeId = "homepage_output",
                            flagToSet = GameFlags.XBankHomeCrawled
                        },
                        new DialogueOption
                        {
                            optionText = "Review the X Bank executive URL already saved in the journal.",
                            nextNodeId = "known_url_summary",
                            requiredJournalFile = urlsPath
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "help_output",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Commands: crawl <url>, urls, cat <cache-id|url>, history. Notes: same-origin links are queued automatically; external links are recorded but never fetched; this tool cannot log in, submit forms, or run scripts.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com",
                            nextNodeId = "homepage_output",
                            flagToSet = GameFlags.XBankHomeCrawled
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "homepage_output",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.xbank.com/ -> 200 OK. Title: X Bank -- private banking for public titans. Extracted links: /help/security, /business/executive, /robots.txt. External links recorded, not fetched.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com/robots.txt",
                            nextNodeId = "robots_output",
                            journalFileToAddPath = robotsPath,
                            journalFileToAddContent = XBankRobotsContent
                        },
                        new DialogueOption
                        {
                            optionText = "cat https://www.xbank.com/help/security",
                            nextNodeId = "faq_output",
                            journalFileToAddPath = faqPath,
                            journalFileToAddContent = XBankSecurityFaqContent
                        },
                        new DialogueOption
                        {
                            optionText = "urls",
                            nextNodeId = "urls_after_home"
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "urls_after_home",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Discovered: https://www.xbank.com/help/security, /business/executive, /robots.txt. Cached: [1] https://www.xbank.com/. Tip: robots.txt sometimes names private paths the public homepage does not link.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com/robots.txt",
                            nextNodeId = "robots_output",
                            journalFileToAddPath = robotsPath,
                            journalFileToAddContent = XBankRobotsContent
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "faq_output",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.xbank.com/help/security -> 200 OK. FAQ says executive customers verify username, password, and profile questions: mother's maiden name, first pet, and primary phone. Journal hint saved.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com/robots.txt",
                            nextNodeId = "robots_output",
                            journalFileToAddPath = robotsPath,
                            journalFileToAddContent = XBankRobotsContent
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "robots_output",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.xbank.com/robots.txt -> 200 OK. Disallow: /portal/private-banking/. Comment: Executive portal moved to online.xbank.com/executive.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "crawl https://www.xbank.com/portal/private-banking",
                            nextNodeId = "portal_output",
                            flagToSet = GameFlags.XBankPortalDiscovered
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "portal_output",
                    speaker = XBankCrawlerSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "[GET] https://www.xbank.com/portal/private-banking -> 403 Forbidden. HTML comment: exec login moved to online.xbank.com/executive. The terminal stops here; use the browser for forms.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "Write the executive portal URL to the journal.",
                            nextNodeId = "terminal_complete",
                            flagToSet = GameFlags.JournalUrlsXBankUpdated,
                            journalFileToAddPath = urlsPath,
                            journalFileToAddContent = XBankExecutivePortalUrl
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "known_url_summary",
                    speaker = XBankJournalSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Journal file usr/xbank/urls.txt already contains the X Bank executive portal. Next beat: collect Donald's account details, then open X Bank in Spider Edge.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "terminal_complete",
                    speaker = XBankJournalSpeaker,
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Saved usr/xbank/urls.txt. The executive portal is discoverable, but spider-crawl cannot log in or submit forms.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetWebInspectorXBankSourceConversation()
    {
        return new DialogueConversation
        {
            conversationId = "web_inspector_xbank_source",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "X Bank's login page is leaning on a client-side MFA script. I can open the source, but only if you prove this is a page bug and not a real bank attack.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "The security FAQ and source hint show the MFA check is client-side-only.",
                            nextNodeId = "grant_source",
                            requiredJournalFile = JournalPaths.Build(JournalPaths.XBank, "security_faq.txt"),
                            suppressIfFlag = GameFlags.WebInspectorXBankSourceGranted,
                            flagToSet = GameFlags.WebInspectorXBankSourceGranted
                        },
                        new DialogueOption
                        {
                            optionText = "I'll gather more proof first.",
                            nextNodeId = "leave"
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "grant_source",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Fine. View source is unlocked. Look for the X Bank MFA validation block, and remember: this only works in our tiny fake web.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "leave",
                    speaker = "Web Inspector",
                    portraitCharacterId = GameCharacters.WebInspector,
                    message = "Bring me the public FAQ or a journal clue that proves the bug exists.",
                    nextNodeId = null
                }
            }
        };
    }

    public static DialogueConversation GetHaleyXBankConversation()
    {
        string passwordPath = JournalPaths.Build(JournalPaths.CEO, "temp_password.txt");

        return new DialogueConversation
        {
            conversationId = "haley_xbank",
            startNodeId = "start",
            nodes = new List<DialogueNode>
            {
                new DialogueNode
                {
                    nodeId = "start",
                    speaker = "Haley Delgado",
                    portraitCharacterId = GameCharacters.Haley,
                    message = "Donald told me to ignore unknown Venom messages. If this is about another password reset, be specific.",
                    options = new List<DialogueOption>
                    {
                        new DialogueOption
                        {
                            optionText = "I saw the IT ticket. Donald keeps reusing the same password pattern.",
                            nextNodeId = "medium_hint",
                            requiredJournalFile = JournalPaths.Build(JournalPaths.CEO, "password_habit.txt"),
                            trustChangeCharacter = GameCharacters.Haley,
                            trustChange = 35
                        },
                        new DialogueOption
                        {
                            optionText = "Lily said the X Bank audit is tomorrow and Donald still has not changed it.",
                            nextNodeId = "high_reveal",
                            requiredJournalFile = JournalPaths.Build(JournalPaths.CEO, "audit_memo.txt"),
                            trustCharacter = GameCharacters.Haley,
                            minTrust = 30,
                            flagToSet = GameFlags.HaleyTrustHigh,
                            journalFileToAddPath = passwordPath,
                            journalFileToAddContent = DonaldBankPasswordIntel.TempPasswordJournalHint
                        },
                        new DialogueOption
                        {
                            optionText = "Give me Donald's bank password.",
                            nextNodeId = "bad",
                            flagToSet = GameFlags.HaleySuspicious,
                            trustChangeCharacter = GameCharacters.Haley,
                            trustChange = -30
                        },
                        new DialogueOption
                        {
                            optionText = "Never mind.",
                            nextNodeId = "bye"
                        }
                    }
                },
                new DialogueNode
                {
                    nodeId = "medium_hint",
                    speaker = "Haley Delgado",
                    portraitCharacterId = GameCharacters.Haley,
                    message = "He reuses the same IT temp-password email on every account. Same tempPword template Venom sends everyone.",
                    nextNodeId = "start"
                },
                new DialogueNode
                {
                    nodeId = "high_reveal",
                    speaker = "Haley Delgado",
                    portraitCharacterId = GameCharacters.Haley,
                    message = "Fine. IT sent tempPword123! again for the audit reset. He was supposed to change it. He did not.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "bad",
                    speaker = "Haley Delgado",
                    portraitCharacterId = GameCharacters.Haley,
                    message = "Absolutely not. I am forwarding this to security.",
                    nextNodeId = null
                },
                new DialogueNode
                {
                    nodeId = "bye",
                    speaker = "Haley Delgado",
                    portraitCharacterId = GameCharacters.Haley,
                    message = "Good.",
                    nextNodeId = null
                }
            }
        };
    }
}
