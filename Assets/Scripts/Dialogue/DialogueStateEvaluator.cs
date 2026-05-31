public static class DialogueStateEvaluator
{
    public static bool CanShowOption(DialogueOption option)
    {
        if (!string.IsNullOrEmpty(option.requiredFlag) &&
            !GameStateManager.Instance.GetFlag(option.requiredFlag))
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

        if (!string.IsNullOrEmpty(option.trustCharacter) &&
            GameStateManager.Instance.GetTrust(option.trustCharacter) < option.minTrust)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(option.trustCharacter) &&
            option.maxTrust >= 0f &&
            GameStateManager.Instance.GetTrust(option.trustCharacter) > option.maxTrust)
        {
            return false;
        }

        return true;
    }

    public static void ApplyOptionEffects(DialogueOption option)
    {
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
    }
}