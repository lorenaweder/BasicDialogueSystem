using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace LW.DialogueSystem
{
    public class BottomDialogueView : BaseDialogueView
    {
        [Header("View")]
        [SerializeField] private Image _image;
        [SerializeField] private CanvasGroup _boxCanvasGroup;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _line;

        private object _animation = new();
        private RectTransform _boxRectTransform;
        private Vector2 _boxInitialPosition;
        private Vector2 _boxOffset = new(40f, 0f);

        private bool _isMidUncancellableAnimation;

        protected override bool CanAdvanceDialogue => base.CanAdvanceDialogue && !_isMidUncancellableAnimation;

        private void Update()
        {
            // Example input, ideally decouple this from here
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) Next();
        }

        protected override void InitializeHiddenView()
        {
            // References
            _boxRectTransform = _boxCanvasGroup.transform as RectTransform;
            _boxInitialPosition = _boxRectTransform.anchoredPosition;

            // Hide Visuals
            var color = _image.color;
            color.a = 0f;
            _image.color = color;
            var pos = _image.rectTransform.anchoredPosition;
            pos.y = -_image.rectTransform.rect.height;
            _image.rectTransform.anchoredPosition = pos;

            _boxCanvasGroup.alpha = 0f;
            pos = _boxRectTransform.anchoredPosition;
            pos += _boxOffset;
            _boxRectTransform.anchoredPosition = pos;

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
                .Insert(0f, _image.rectTransform.DOAnchorPosY(0f, time * 0.5f))
                .Insert(0f, _image.DOFade(1f, time * 0.5f))
                .Insert(time * 0.3f, _boxCanvasGroup.DOFade(1f, time * 0.5f))
                .Insert(time * 0.3f, _boxRectTransform.DOAnchorPos(_boxInitialPosition, time * 0.5f))
                .OnComplete(() => _state = DialogueState.Started)
                .SetId(_animation);
        }

        protected override void AnimateOnlyLineChange(string nextEntryLine)
        {
            DOTween.Kill(_animation);
            DOTween.Sequence()
                .Append(_line.DOFade(0f, 0.1f))
                .AppendCallback(() => _line.SetText(nextEntryLine))
                .Append(_line.DOFade(1f, 0.1f))
                .SetId(_animation);
        }

        protected override void AnimateFullEntryChange(DialogueEntry entry)
        {
            _isMidUncancellableAnimation = true;

            var time = 0.5f;
            DOTween.Kill(_animation);
            DOTween.Sequence()
            // Fade out
                .Insert(0f, _boxRectTransform.DOAnchorPos(_boxInitialPosition + _boxOffset, time * 0.5f))
                .Insert(0f, _boxCanvasGroup.DOFade(0f, time * 0.5f))
                .Insert(time * 0.3f, _image.rectTransform.DOAnchorPosY(-_image.rectTransform.rect.height, time * 0.5f))
                .Insert(time * 0.3f, _image.DOFade(0f, time * 0.5f))
            // Change values while hidden
                .InsertCallback(time, () =>
                {
                    InstantlyUpdateEntry(entry);
                    _isMidUncancellableAnimation = false;
                })
            // Fade the dialogue line fully in, in case things got interrupted mid-way
                .Insert(time,_line.DOFade(1f, 0.1f))
            // Begin fade in again
                .Insert(time, _image.rectTransform.DOAnchorPosY(0f, time * 0.5f))
                .Insert(time, _image.DOFade(1f, time * 0.5f))
                .Insert(time + time * 0.3f, _boxCanvasGroup.DOFade(1f, time * 0.5f))
                .Insert(time + time * 0.3f, _boxRectTransform.DOAnchorPos(_boxInitialPosition, time * 0.5f))
                .SetId(_animation);
        }

        protected override void InstantlyUpdateEntry(DialogueEntry entry)
        {
            _image.sprite = entry.Character.Image;
            _title.SetText(entry.Character.Title);
            _line.SetText(entry.Line);
        }

        protected override void AnimateDialogueOut()
        {
            var time = 0.5f;
            DOTween.Kill(_animation);
            DOTween.Sequence()
                .Insert(0f, _boxRectTransform.DOAnchorPos(_boxInitialPosition + _boxOffset, time * 0.5f))
                .Insert(0f, _boxCanvasGroup.DOFade(0f, time * 0.5f))
                .Insert(time * 0.3f, _image.rectTransform.DOAnchorPosY(-_image.rectTransform.rect.height, time * 0.5f))
                .Insert(time * 0.3f, _image.DOFade(0f, time * 0.5f))
                .OnComplete(()=>
                {
                    MarkDialogueFinished();
                    gameObject.SetActive(false);
                })
                .SetId(_animation);
        }
    }
}