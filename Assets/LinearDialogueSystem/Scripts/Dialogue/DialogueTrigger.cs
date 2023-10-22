using UnityEngine;

namespace LW.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private KeyCode _keycode = KeyCode.D;
        [SerializeField] private LinearConversationTriggerReference _triggerReference;
        [SerializeField] private LinearConversation _conversation;

        void Update()
        {
            // Change to any condition you want
            if (Input.GetKeyDown(_keycode)) TriggerDialogue();
        }

        private void TriggerDialogue()
        {
            _triggerReference.TriggerConversation(_conversation);
        }
    }
}