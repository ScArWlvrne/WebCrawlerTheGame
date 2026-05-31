using System.Collections.Generic;

public static class DialogueConversationFactory
{
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
}
