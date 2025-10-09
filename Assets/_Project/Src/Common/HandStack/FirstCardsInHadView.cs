using System;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.Hex;
using _Project.Src.Common.PlayerInputs.Storages;
using LitMotion;
using LitMotion.Extensions;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Project.Src.Common.HandStack
{
    public class FirstCardsInHadView : MonoBehaviour, IDisposable
    {
        [Serializable]
        public class ElementView : IDisposable
        {
            public Transform viewTransform => cardView.gameObject.transform;
            [SerializeField] private HexView cardView;

            private CellController _cardController;

            public Transform cardTargetTransform => _cardTargetTransform;
            private Transform _cardTargetTransform;

            public void InitTargetPoint(Transform parent, int index, float heightOffset)
            {
                var pointTarget = new GameObject($"pointTarget_{index}");
                pointTarget.transform.parent = parent;
                pointTarget.transform.localPosition = Vector3.down * (index * heightOffset);
                _cardTargetTransform = pointTarget.transform;

                cardView.transform.localPosition = pointTarget.transform.localPosition;
            }

            public void InitController(CellModel cellModel, CellSettings settings)
            {
                _cardController?.Dispose();
                _cardController = null;

                if (cellModel != null)
                {
                    cardView.EnableRendererContainer();
                    _cardController = new CellController(cellModel, settings);
                    _cardController.BindView(cardView);
                    cardView.Bind(_cardController);
                }
                else
                {
                    cardView.DisableRendererContainer();
                }
            }

            public void SetLocalPosition(Vector3 localPosition)
            {
                cardView.gameObject.transform.localPosition = localPosition;
            }

            public void SetLocalPosition(ElementView elementView)
            {
                cardView.gameObject.transform.localPosition = elementView._cardTargetTransform.localPosition;
            }

            public void Dispose()
            {
                _cardController?.Dispose();
            }
        }

        [SerializeField] private bool moveHexes = true;


        [SerializeField] private ElementView firstEl;
        [SerializeField] private ElementView secondEl;
        [SerializeField] private ElementView thirdEl;


        [SerializeField] private float heightOffset = 3f;
        private readonly CompositeDisposable _disposables = new();


        [Inject]
        public void Inject(PlayerInputStorage storage, CellSettings settings)
        {
            InitializePositions();

            storage.currentCellModelInHand
                .Subscribe(cellModel => { firstEl.InitController(cellModel, settings); })
                .AddTo(_disposables);

            storage.secondCellModelInHand
                .Subscribe(cellModel => { secondEl.InitController(cellModel, settings); })
                .AddTo(_disposables);

            storage.thirdCellModelInHand
                .Subscribe(cellModel => { thirdEl.InitController(cellModel, settings); })
                .AddTo(_disposables);


            // animations

            storage.currentCellModelInHand
                .Subscribe(_ => HandleFirstCardEvent())
                .AddTo(_disposables);

            storage.secondCellModelInHand
                .Subscribe(_ => HandleSecondCardEvent())
                .AddTo(_disposables);
        }

        private MotionHandle _currentAnimationHandle;
        private MotionHandle _secondAnimationHandle;

        [SerializeField] private float timeDuration = 1f;

        private void HandleFirstCardEvent()
        {
            if (_currentAnimationHandle.IsActive())
            {
                _currentAnimationHandle.Cancel();
            }

            firstEl.SetLocalPosition(secondEl);

            _currentAnimationHandle = LMotion
                .Create(secondEl.cardTargetTransform.localPosition, firstEl.cardTargetTransform.localPosition,
                    timeDuration)
                .WithEase(Ease.OutCubic)
                .BindToLocalPosition(firstEl.viewTransform)
                .AddTo(this);
        }

        private void HandleSecondCardEvent()
        {
            if (_secondAnimationHandle.IsActive())
            {
                _secondAnimationHandle.Cancel();
            }

            secondEl.SetLocalPosition(thirdEl);


            _secondAnimationHandle = LMotion
                .Create(thirdEl.cardTargetTransform.localPosition, secondEl.cardTargetTransform.localPosition,
                    timeDuration)
                .WithEase(Ease.OutCubic)
                .BindToLocalPosition(secondEl.viewTransform)
                .AddTo(this);
        }


        private void InitializePositions()
        {
            firstEl.InitTargetPoint(gameObject.transform, 0, heightOffset);
            secondEl.InitTargetPoint(gameObject.transform, 1, heightOffset);
            thirdEl.InitTargetPoint(gameObject.transform, 2, heightOffset);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_currentAnimationHandle.IsActive())
            {
                _currentAnimationHandle.Cancel();
            }

            if (_secondAnimationHandle.IsActive())
            {
                _secondAnimationHandle.Cancel();
            }

            firstEl?.Dispose();
            secondEl?.Dispose();
            thirdEl?.Dispose();
            _disposables?.Dispose();
        }
    }
}