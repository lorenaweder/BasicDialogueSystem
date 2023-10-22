using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LW.DialogueSystem
{
    public class SideDialogueView : BaseDialogueView
    {
        public enum Side
        {
            Left,
            Right
        }


        [System.Serializable]
        public class SideDialogueCharacterView
        {
            [Header("View")]
            public Image image;
            public CanvasGroup boxCanvasGroup;
            public TMP_Text title;
            public TMP_Text line;

            [HideInInspector] public RectTransform boxRectTransform;
            [HideInInspector] public Vector2 boxInitialPosition;
            [HideInInspector] public Vector2 imageInitialPosition;
            [HideInInspector] public Vector2 boxOffset = new(0f, -20f);

            private float _sideMultiplier;

            public float imageHiddenAnchorX => image.rectTransform.rect.width * _sideMultiplier;

            public void InitializeHidden(Side side)
            {
                boxRectTransform = boxCanvasGroup.transform as RectTransform;
                boxInitialPosition = boxRectTransform.anchoredPosition;
                imageInitialPosition = image.rectTransform.anchoredPosition;

                _sideMultiplier = side == Side.Right ? 1f : -1f;

                HideVisuals();
            }

            public void HideVisuals()
            {
                var color = image.color;
                color.a = 0f;
                image.color = color;
                var pos = image.rectTransform.anchoredPosition;
                pos.x = _sideMultiplier * image.rectTransform.rect.width;
                image.rectTransform.anchoredPosition = pos;

                boxCanvasGroup.alpha = 0f;
                pos = boxRectTransform.anchoredPosition;
                pos += boxOffset;
                boxRectTransform.anchoredPosition = pos;
            }
        }


        [SerializeField] private Side _startingSide = Side.Left;
        [SerializeField] private SideDialogueCharacterView _right;
        [SerializeField] private SideDialogueCharacterView _left;

        private object _animation = new();
        private bool _isMidUncancellableAnimation;

        private SideDialogueCharacterView _activeView;


        protected override bool CanAdvanceDialogue => base.CanAdvanceDialogue && !_isMidUncancellableAnimation;

        private void Update()
        {
            // Example input, ideally decouple this from here
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) Next();
        }

        protected override void InitializeHiddenView()
        {
            // References
            _right.InitializeHidden(Side.Right);
            _left.InitializeHidden(Side.Left);

            _activeView = _startingSide == Side.Right ? _right : _left;

            gameObject.SetActive(false);
        }


        protected override void Terminate()
        {
            DOTween.Kill(_animation);
            _animation = null;
        }

        protected override void AnimateDialogueIn()
        {
            gameObject.SetActive(true);

            var time = 0.5f;
            DOTween.Kill(_animation);
            DOTween.Sequence()
                .Insert(0f, _activeView.image.rectTransform.DOAnchorPosX(_activeView.imageInitialPosition.x, time * 0.5f))
                .Insert(0f, _activeView.image.DOFade(1f, time * 0.5f))
                .Insert(time * 0.3f, _activeView.boxCanvasGroup.DOFade(1f, time * 0.5f))
                .Insert(time * 0.3f, _activeView.boxRectTransform.DOAnchorPos(_activeView.boxInitialPosition, time * 0.5f))
                .OnComplete(() => _state = DialogueState.Started)
                .SetId(_animation);
        }

        protected override void AnimateOnlyLineChange(string nextEntryLine)
        {
            DOTween.Kill(_animation);
            DOTween.Sequence()
                .Append(_activeView.line.DOFade(0f, 0.1f))
                .AppendCallback(() => _activeView.line.SetText(nextEntryLine))
                .Append(_activeView.line.DOFade(1f, 0.1f))
                .SetId(_animation);
        }

        protected override void AnimateFullEntryChange(DialogueEntry entry)
        {
            _isMidUncancellableAnimation = true;

            var nonActiveView = _activeView == _left ? _right : _left;

            var time = 0.5f;
            DOTween.Kill(_animation);
            DOTween.Sequence()
            // Fade out
                .Insert(0f, _activeView.boxRectTransform.DOAnchorPos(_activeView.boxInitialPosition + _activeView.boxOffset, time * 0.5f))
                .Insert(0f, _activeView.boxCanvasGroup.DOFade(0f, time * 0.5f))
                .Insert(time * 0.3f, _activeView.image.rectTransform.DOAnchorPosX(_activeView.imageHiddenAnchorX, time * 0.5f))
                .Insert(time * 0.3f, _activeView.image.DOFade(0f, time * 0.5f))
            // Change values and view while hidden
                .InsertCallback(time, () =>
                {
                    _activeView = _activeView == _left ? _right : _left;
                    InstantlyUpdateEntry(entry);
                    _isMidUncancellableAnimation = false;
                })
            // Fade the dialogue line fully in, in case things got interrupted mid-way
                .Insert(time, nonActiveView.line.DOFade(1f, 0.1f))
            // Begin fade in again
                .Insert(time, nonActiveView.image.rectTransform.DOAnchorPosX(nonActiveView.imageInitialPosition.x, time * 0.5f))
                .Insert(time, nonActiveView.image.DOFade(1f, time * 0.5f))
                .Insert(time + time * 0.3f, nonActiveView.boxCanvasGroup.DOFade(1f, time * 0.5f))
                .Insert(time + time * 0.3f, nonActiveView.boxRectTransform.DOAnchorPos(nonActiveView.boxInitialPosition, time * 0.5f))
                .SetId(_animation);
        }

        protected override void InstantlyUpdateEntry(DialogueEntry entry)
        {
            _activeView.image.sprite = entry.Character.Image;
            _activeView.title.SetText(entry.Character.Title);
            _activeView.line.SetText(entry.Line);
        }

        protected override void AnimateDialogueOut()
        {
            var time = 0.5f;
            DOTween.Kill(_animation);
            DOTween.Sequence()
                .Insert(0f, _activeView.boxRectTransform.DOAnchorPos(_activeView.boxInitialPosition + _activeView.boxOffset, time * 0.5f))
                .Insert(0f, _activeView.boxCanvasGroup.DOFade(0f, time * 0.5f))
                .Insert(time * 0.3f, _activeView.image.rectTransform.DOAnchorPosX(_activeView.imageHiddenAnchorX, time * 0.5f))
                .Insert(time * 0.3f, _activeView.image.DOFade(0f, time * 0.5f))
                .OnComplete(() =>
                {
                    MarkDialogueFinished();
                    gameObject.SetActive(false);
                })
                .SetId(_animation);

            var nonActiveView = _activeView == _left ? _right : _left;
            nonActiveView.HideVisuals();

            // Reset for next time
            _activeView = _startingSide == Side.Right ? _right : _left;
        }
    }
}