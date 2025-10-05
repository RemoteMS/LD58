using LitMotion;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Project.Src.Common.HandStack.UI
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] TMP_Text _scoreText;

        private MotionHandle _currentMotionHandle;
        private MotionHandle _scaleMotionHandle;
        private int _currentDisplayedScore;

        [Inject]
        public void Inject(Hand hand)
        {
            _currentDisplayedScore = hand.count.Value;
            _scoreText.text = _currentDisplayedScore.ToString();

            hand.count.Subscribe(HandleCardsCount).AddTo(this);
        }

        private void HandleCardsCount(int newScore)
        {
            // Анимация перебора чисел
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

            // Анимация "дергания" текста
            PlayBounceAnimation();
        }

        private void PlayBounceAnimation()
        {
            // Отменяем предыдущую анимацию масштаба
            if (_scaleMotionHandle.IsActive())
            {
                _scaleMotionHandle.Cancel();
            }

            // Анимация: увеличение -> возврат к нормальному размеру
            _scaleMotionHandle = LMotion
                .Create(_scoreText.transform.localScale, Vector3.one * 1.3f, 0.1f) // Быстро увеличиваем
                .WithEase(Ease.OutQuad)
                .WithOnComplete(() =>
                {
                    // Возвращаем к нормальному размеру с пружинным эффектом
                    LMotion.Create(_scoreText.transform.localScale, Vector3.one, 0.2f)
                        .WithEase(Ease.OutBack)
                        .Bind(scale => _scoreText.transform.localScale = scale);
                })
                .Bind(scale => _scoreText.transform.localScale = scale);
        }

        private void OnDestroy()
        {
            if (_currentMotionHandle.IsActive())
            {
                _currentMotionHandle.Cancel();
            }

            if (_scaleMotionHandle.IsActive())
            {
                _scaleMotionHandle.Cancel();
            }
        }
    }
}