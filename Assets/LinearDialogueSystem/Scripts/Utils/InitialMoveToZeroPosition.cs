using UnityEngine;

namespace LW.Utils
{
    [DefaultExecutionOrder(-100)]
    public class InitialMoveToZeroPosition : MonoBehaviour
    {
        private void Awake()
        {
            if (transform is RectTransform t)
            {
                t.anchoredPosition = Vector2.zero;
            }
            else transform.position = Vector3.zero;
        }
    }
}