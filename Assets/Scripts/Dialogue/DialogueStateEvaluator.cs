public static class DialogueStateEvaluator
{
    public static bool CanShowOption(DialogueOption option)
    {
        if (option == null)
            return false;

        if (!string.IsNullOrEmpty(option.requiredFlag) &&
            !GameStateManager.Instance.GetFlag(option.requiredFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.requiredNotFlag) &&
            GameStateManager.Instance.GetFlag(option.requiredNotFlag))
        if (!string.IsNullOrEmpty(option.suppressIfFlag) &&
            GameStateManager.Instance.GetFlag(option.suppressIfFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.requiredJournalFile) &&
            !GameStateManager.Instance.HasJournalFile(option.requiredJournalFile))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.requiredUncommentedCodeBlock) &&
            !GameStateManager.Instance.IsCodeBlockUncommented(option.requiredUncommentedCodeBlock))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.requiredExhaustedInteractable) &&
            !GameStateManager.Instance.IsInteractableExhausted(option.requiredExhaustedInteractable))
        {
            return false;
        }

        bool shouldCheckOptionMinTrust = option.hasMinTrust || option.minTrust > 0f;
        bool shouldCheckOptionMaxTrust = option.hasMaxTrust || option.maxTrust >= 0f;

        if (!string.IsNullOrEmpty(option.trustCharacter) &&
            shouldCheckOptionMinTrust &&
            GameStateManager.Instance.GetTrust(option.trustCharacter) < option.minTrust)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.trustCharacter) &&
            shouldCheckOptionMaxTrust &&
            GameStateManager.Instance.GetTrust(option.trustCharacter) > option.maxTrust)
        {
            return false;
        }

        return true;
    }

    public static bool CanFollowRoute(DialogueRoute route)
    {
        if (route == null)
            return false;

        return CanMeetConditions(route.conditions);
    }

    public static bool CanMeetConditions(DialogueConditionSet conditions)
    {
        if (conditions == null)
            return true;

        if (!string.IsNullOrEmpty(conditions.requiredFlag) &&
            !GameStateManager.Instance.GetFlag(conditions.requiredFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(conditions.requiredNotFlag) &&
            GameStateManager.Instance.GetFlag(conditions.requiredNotFlag))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(conditions.trustCharacter) &&
            conditions.hasMinTrust &&
            GameStateManager.Instance.GetTrust(conditions.trustCharacter) < conditions.minTrust)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(conditions.trustCharacter) &&
            conditions.hasMaxTrust &&
            GameStateManager.Instance.GetTrust(conditions.trustCharacter) > conditions.maxTrust)
        {
            return false;
        }

        return true;
    }

    public static void ApplyOptionEffects(DialogueOption option)
    {
        if (option == null)
            return;

        if (!string.IsNullOrEmpty(option.flagToSet))
        {
            GameStateManager.Instance.SetFlag(option.flagToSet, true);
        }

        if (!string.IsNullOrEmpty(option.journalFileToAddPath))
        {
            GameStateManager.Instance.AddJournalFile(
                option.journalFileToAddPath,
                option.journalFileToAddContent
            );
        }

        if (!string.IsNullOrEmpty(option.trustChangeCharacter))
        {
            GameStateManager.Instance.AddTrust(
                option.trustChangeCharacter,
                option.trustChange
            );
        }

        if (option.effects == null)
            return;

        foreach (DialogueEffect effect in option.effects)
        {
            if (!string.IsNullOrEmpty(effect.flagKey))
            {
                GameStateManager.Instance.SetFlag(effect.flagKey, effect.flagValue);
            }

            if (!string.IsNullOrEmpty(effect.trustChangeCharacter))
            {
                GameStateManager.Instance.AddTrust(
                    effect.trustChangeCharacter,
                    effect.trustChange
                );
            }

            if (!string.IsNullOrEmpty(effect.journalEntry))
            {
                GameStateManager.Instance.AddJournalEntry(effect.journalEntry);
            }
        }
    }
}
