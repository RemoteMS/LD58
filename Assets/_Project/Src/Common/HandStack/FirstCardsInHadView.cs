using System;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
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
            [field: SerializeField] public Transform cardContainer { get; private set; }
            [SerializeField] private Transform cardCameraRotateContainer;

            [SerializeField] public Transform cardTransform;

            private IHexView _cardView => cardView;
            [SerializeField] private HexView cardView;

            private CellController _cardController;

            public Transform cardTargetTransform => _cardTargetTransform;
            private Transform _cardTargetTransform;

            public MotionHandle _onChangeCardAnimation;
            public MotionHandle _onHexRotateAnimation;

            public void InitTargetPoint(Transform parent, int index, float heightOffset)
            {
                var pointTarget = new GameObject($"pointTarget_{index}");
                pointTarget.transform.parent = parent;
                pointTarget.transform.localPosition = Vector3.down * (index * heightOffset);
                _cardTargetTransform = pointTarget.transform;

                cardContainer.localPosition = pointTarget.transform.localPosition;
            }

            public void InitController(CellModel cellModel, CellSettings settings)
            {
                _cardController?.Dispose();
                _cardController = null;

                if (cellModel != null)
                {
                    _cardView.EnableRendererContainer();
                    _cardController = new CellController(cellModel, settings);
                    _cardView.Bind(_cardController);
                }
                else
                {
                    _cardView.DisableRendererContainer();
                }
            }

            public void SetLocalPosition(ElementView elementView)
            {
                cardContainer.gameObject.transform.localPosition = elementView._cardTargetTransform.localPosition;
            }

            public void SetRotation(Quaternion rotation)
            {
                cardContainer.localRotation = rotation;
            }

            public void SetCardRotatorRotation(Quaternion rotation)
            {
                cardCameraRotateContainer.localRotation = rotation;
            }

            public void Dispose()
            {
                if (_onChangeCardAnimation.IsActive())
                {
                    _onChangeCardAnimation.Cancel();
                }

                if (_onHexRotateAnimation.IsActive())
                {
                    _onHexRotateAnimation.Cancel();
                }

                _cardController?.Dispose();
            }
        }


        [SerializeField] private bool moveHexes = true;


        [SerializeField] private ElementView firstEl;
        [SerializeField] private ElementView secondEl;
        [SerializeField] private ElementView thirdEl;


        [Header("Cards Params")] [SerializeField]
        private float heightOffset = 3f;

        [SerializeField] private float timeDuration = 1f;

        private Quaternion _rotation;
        private float _settingHexRotationSpeed;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Inject(PlayerInputStorage storage, CellSettings cellSettings, HexSetting hexSetting)
        {
            InitializePositions();

            _settingHexRotationSpeed = hexSetting.hexRotationSpeed;
            _rotation = Quaternion.identity;

            // data bind

            BindHexData(storage.currentCellModelInHand, firstEl,  cellSettings);
            BindHexData(storage.secondCellModelInHand,  secondEl, cellSettings);
            BindHexData(storage.thirdCellModelInHand,   thirdEl,  cellSettings);

            // animations

            BindCardMovementToPrev(storage.currentCellModelInHand, firstEl,  secondEl);
            BindCardMovementToPrev(storage.secondCellModelInHand,  secondEl, thirdEl);

            // R/T Card rotation

            storage.currentHexRotation
                .Subscribe(x =>
                {
                    RotateOnIndex(x, firstEl);
                    RotateOnIndex(x, secondEl);
                    RotateOnIndex(x, thirdEl);
                })
                .AddTo(_disposables);

            // Q/E Camera rotation

            storage.invertedCameraRotation
                .Subscribe(quaternion => { RotateHand(quaternion, firstEl, secondEl, thirdEl); })
                .AddTo(_disposables);
        }

        private void InitializePositions()
        {
            firstEl.InitTargetPoint(gameObject.transform, 0, heightOffset);
            secondEl.InitTargetPoint(gameObject.transform, 1, heightOffset);
            thirdEl.InitTargetPoint(gameObject.transform, 2, heightOffset);
        }

        private void BindCardMovementToPrev(
            IReadOnlyReactiveProperty<CellModel> data,
            ElementView element,
            ElementView prev
        )
        {
            data
                .Subscribe(_ => MoveToPrevOnValueChange(element, prev))
                .AddTo(_disposables);
        }


        private void BindHexData(
            IReadOnlyReactiveProperty<CellModel> data,
            ElementView element,
            CellSettings cellSettings
        )
        {
            data
                .Subscribe(cellModel => { element.InitController(cellModel, cellSettings); })
                .AddTo(_disposables);
        }

        private void RotateHand(Quaternion rotation, params ElementView[] elems)
        {
            foreach (var elem in elems)
            {
                elem.SetCardRotatorRotation(rotation);
            }
        }

        private void RotateOnIndex(int index, ElementView element)
        {
            if (index < 0 || index > 5)
            {
                Debug.LogError($"Invalid rotation index: {index}. Must be between 0 and 5.");
                return;
            }

            if (element._onHexRotateAnimation.IsActive())
            {
                element._onHexRotateAnimation.Cancel();
            }

            var targetAngle = index * 60f;
            var targetRotation = Quaternion.Euler(0, targetAngle, 0);

            element._onHexRotateAnimation = LMotion.Create(_rotation, targetRotation, _settingHexRotationSpeed)
                .WithEase(Ease.InOutQuad)
                .Bind(x =>
                {
                    _rotation = x;
                    if (element.cardTransform.gameObject)
                        element.cardTransform.rotation = _rotation;
                });
        }


        private void MoveToPrevOnValueChange(ElementView element, ElementView prevElement)
        {
            if (element._onChangeCardAnimation.IsActive())
            {
                element._onChangeCardAnimation.Cancel();
            }

            element.SetLocalPosition(prevElement);

            element._onChangeCardAnimation = LMotion
                .Create(prevElement.cardTargetTransform.localPosition, element.cardTargetTransform.localPosition,
                    timeDuration)
                .WithEase(Ease.OutCubic)
                .BindToLocalPosition(element.cardContainer)
                .AddTo(this);
        }


        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            firstEl?.Dispose();
            secondEl?.Dispose();
            thirdEl?.Dispose();

            _disposables?.Dispose();
        }
    }
}