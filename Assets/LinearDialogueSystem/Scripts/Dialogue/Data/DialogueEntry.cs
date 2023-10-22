using UnityEngine;

namespace LW.DialogueSystem
{
    [System.Serializable]
    public class DialogueEntry
    {
        [SerializeField] private DialogueCharacter _character;
        [SerializeField] [TextArea(1, 15)] private string _line;

        public DialogueCharacter Character => _character;
        public string Line => _line;
    }
}