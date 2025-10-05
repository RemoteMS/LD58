using TMPro;
using UniRx;
using UnityEngine;
using VContainer;
using LitMotion;

namespace _Project.Src.Common.Points
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] Ease anim = Ease.OutBack;
        [SerializeField] TMP_Text _scoreText;
        [SerializeField] RectTransform panel;

        private MotionHandle _currentMotionHandle;
        private MotionHandle _panelMotionHandle;
        private int _currentDisplayedScore;
        private float _panelHiddenX;
        private float _panelVisibleX;

        [Inject]
        public void Inject(PointService service)
        {
            InitializePanelPositions();

            _currentDisplayedScore = service.score.Value;
            _scoreText.text = _currentDisplayedScore.ToString();

            SetPanelHiddenImmediately();

            var shouldBeVisible = _currentDisplayedScore > 0;
            if (shouldBeVisible)
            {
                UpdatePanelVisibility(true);
            }

            service.score.Subscribe(HandleNewScore).AddTo(this);
        }

        private void InitializePanelPositions()
        {
            _panelVisibleX = panel.anchoredPosition.x;

            _panelHiddenX = _panelVisibleX + panel.rect.width;
        }

        private void SetPanelHiddenImmediately()
        {
            var hiddenPosition = new Vector2(_panelHiddenX, panel.anchoredPosition.y);
            panel.anchoredPosition = hiddenPosition;
        }

        private void HandleNewScore(int newScore)
        {
            if (_currentMotionHandle.IsActive())
            {
                _currentMotionHandle.Cancel();
            }

            _currentMotionHandle = LMotion
                .Create(_currentDisplayedScore, newScore, 0.5f)
                .Bind(value =>
                {
                    var displayValue = Mathf.RoundToInt(value);
                    _scoreText.text = displayValue.ToString();
                    _currentDisplayedScore = displayValue;
                });

            var shouldBeVisible = newScore                > 0;
            var currentlyVisible = _currentDisplayedScore > 0;

            if (shouldBeVisible != currentlyVisible)
            {
                UpdatePanelVisibility(shouldBeVisible);
            }
        }

        private void UpdatePanelVisibility(bool show)
        {
            if (_panelMotionHandle.IsActive())
            {
                _panelMotionHandle.Cancel();
            }

            var targetX = show ? _panelVisibleX : _panelHiddenX;
            var targetPosition = new Vector2(targetX, panel.anchoredPosition.y);

            _panelMotionHandle = LMotion
                .Create(panel.anchoredPosition, targetPosition, 0.5f)
                .WithEase(anim)
                .Bind(position => panel.anchoredPosition = position);
        }

        private void OnDestroy()
        {
            if (_currentMotionHandle.IsActive())
            {
                _currentMotionHandle.Cancel();
            }

            if (_panelMotionHandle.IsActive())
            {
                _panelMotionHandle.Cancel();
            }
        }
    }
}