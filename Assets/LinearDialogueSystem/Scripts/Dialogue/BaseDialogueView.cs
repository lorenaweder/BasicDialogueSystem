using UnityEngine;

namespace LW.DialogueSystem
{
    public enum DialogueState
    {
        NotStarted,
        Starting,
        Started,
        Ending
    }

    public abstract class BaseDialogueView : MonoBehaviour
    {
        [SerializeField] protected LinearConversationTriggerReference _triggerReference;

        protected LinearConversation _currentConversation;
        protected int _currentIndex;
        protected DialogueState _state = DialogueState.NotStarted;

        protected virtual bool CanAdvanceDialogue => _state == DialogueState.Started;

        private static bool _gameHasActiveDialogue;

        private void Awake()
        {
            _triggerReference.OnLinearConversationTriggered += StartDialogue;
            InitializeHiddenView();
        }

        private void OnDestroy()
        {
            Terminate();
            _currentConversation = null;
            _triggerReference.OnLinearConversationTriggered -= StartDialogue;
        }

        protected void StartDialogue(LinearConversation conversation)
        {
            if (_gameHasActiveDialogue) return;
            if (_state != DialogueState.NotStarted) return;

            _gameHasActiveDialogue = true;
            _state = DialogueState.Starting;

            _currentConversation = conversation;
            _currentIndex = 0;

            var entry = _currentConversation.Entries[_currentIndex];
            InstantlyUpdateEntry(entry);

            AnimateDialogueIn();
        }

        protected void Next()
        {
            if (!CanAdvanceDialogue) return;
            if (_currentIndex + 1 >= _currentConversation.Entries.Count)
            {
                EndDialogue();
                return;
            }

            var lastEntry = _currentConversation.Entries[_currentIndex];
            var nextEntry = _currentConversation.Entries[++_currentIndex];

            if (nextEntry.Character == lastEntry.Character)
            {
                AnimateOnlyLineChange(nextEntry.Line);
                return;
            }

            AnimateFullEntryChange(nextEntry);
        }

        protected void EndDialogue()
        {
            if (_state == DialogueState.Ending) return;
            _state = DialogueState.Ending;

            AnimateDialogueOut();
        }

        protected void MarkDialogueFinished()
        {
            _state = DialogueState.NotStarted;
            _gameHasActiveDialogue = false;
        }

        protected abstract void InitializeHiddenView();
        protected abstract void Terminate();
        protected abstract void AnimateDialogueIn();
        protected abstract void AnimateDialogueOut();
        protected abstract void AnimateOnlyLineChange(string nextEntryLine);
        protected abstract void AnimateFullEntryChange(DialogueEntry entry);
        protected abstract void InstantlyUpdateEntry(DialogueEntry entry);
        
    }
}