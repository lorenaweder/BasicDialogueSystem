using UnityEngine;

namespace LW.DialogueSystem
{
    [CreateAssetMenu(fileName = "Linear Conversation Trigger", menuName = "Dialogue/Linear Conversation Trigger")]
    public class LinearConversationTriggerReference : ScriptableObject
    {
        public event System.Action<LinearConversation> OnLinearConversationTriggered;

        public void TriggerConversation(LinearConversation conversation)
        {
            OnLinearConversationTriggered?.Invoke(conversation);
        }

        private void OnDestroy()
        {
            OnLinearConversationTriggered = null;
        }
    }
}