using UnityEngine;

namespace LW.DialogueSystem
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character")]
    public class DialogueCharacter : ScriptableObject
    {
        [SerializeField] private Sprite _image;
        [SerializeField] private string _title;

        public Sprite Image => _image;
        public string Title => _title;
    }
}