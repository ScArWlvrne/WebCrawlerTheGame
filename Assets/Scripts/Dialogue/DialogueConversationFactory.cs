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
}
