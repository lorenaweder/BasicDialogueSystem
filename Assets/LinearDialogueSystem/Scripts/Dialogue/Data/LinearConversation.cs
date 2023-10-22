using System.Collections.Generic;
using UnityEngine;

namespace LW.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Conversation", menuName = "Dialogue/Conversation")]
    public class LinearConversation : ScriptableObject
    {
        public List<DialogueEntry> Entries;
    }
}