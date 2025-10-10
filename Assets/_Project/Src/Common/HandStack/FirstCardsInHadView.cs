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

            // public Transform viewTransform => cardView.gameObject.transform;
            [SerializeField] public Transform cardTransform;

            private IHexView _cardView => cardView;
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

            public void SetLocalPosition(Vector3 localPosition)
            {
                cardView.gameObject.transform.localPosition = localPosition;
            }

            public void SetLocalPosition(ElementView elementView)
            {
                cardContainer.gameObject.transform.localPosition = elementView._cardTargetTransform.localPosition;
            }

            public void SetRotation(Quaternion rotation)
            {
                cardContainer.localRotation = rotation;
            }

            public void Dispose()
            {
                _cardController?.Dispose();
            }

            public void SetCardRotatorRotation(Quaternion rotation)
            {
                var eulerAngles = rotation.eulerAngles;
                eulerAngles.y = -eulerAngles.y;
                var newRotation = Quaternion.Euler(eulerAngles);

                cardCameraRotateContainer.localRotation = newRotation;
            }
        }

        [SerializeField] private bool moveHexes = true;


        [SerializeField] private ElementView firstEl;
        [SerializeField] private ElementView secondEl;
        [SerializeField] private ElementView thirdEl;


        [Header("Cards Params")] [SerializeField]
        private float heightOffset = 3f;

        private readonly CompositeDisposable _disposables = new();


        private Quaternion _rotation;
        private MotionHandle _rotationHandle;
        private float _settingHexRotationSpeed;

        [Inject]
        public void Inject(PlayerInputStorage storage, CellSettings cellSettings, HexSetting hexSetting)
        {
            InitializePositions();

            _settingHexRotationSpeed = hexSetting.hexRotationSpeed;
            _rotation = Quaternion.identity;

            storage.currentCellModelInHand
                .Subscribe(cellModel => { firstEl.InitController(cellModel, cellSettings); })
                .AddTo(_disposables);

            storage.secondCellModelInHand
                .Subscribe(cellModel => { secondEl.InitController(cellModel, cellSettings); })
                .AddTo(_disposables);

            storage.thirdCellModelInHand
                .Subscribe(cellModel => { thirdEl.InitController(cellModel, cellSettings); })
                .AddTo(_disposables);


            // animations

            storage.currentCellModelInHand
                .Subscribe(_ => HandleFirstCardEvent())
                .AddTo(_disposables);
            
            storage.secondCellModelInHand
                .Subscribe(_ => HandleSecondCardEvent())
                .AddTo(_disposables);

            // R/T Card rotation

            storage.currentHexRotation
                .Subscribe(RotateOnIndex)
                .AddTo(_disposables);

            // Q/E Camera rotation

            storage.currentCameraRotation
                .Subscribe(RotateHand)
                .AddTo(_disposables);
        }


        private void RotateHand(Quaternion rotation)
        {
            firstEl.SetCardRotatorRotation(rotation);
        }

        private void RotateOnIndex(int index)
        {
            if (index < 0 || index > 5)
            {
                Debug.LogError($"Invalid rotation index: {index}. Must be between 0 and 5.");
                return;
            }

            if (_rotationHandle.IsActive())
            {
                _rotationHandle.Cancel();
            }

            var targetAngle = index * 60f;
            var targetRotation = Quaternion.Euler(0, targetAngle, 0);


            _rotationHandle = LMotion.Create(_rotation, targetRotation, _settingHexRotationSpeed)
                .WithEase(Ease.InOutQuad)
                .Bind(x =>
                {
                    _rotation = x;
                    if (firstEl.cardTransform.gameObject)
                        firstEl.cardTransform.rotation = _rotation;
                });
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
                .BindToLocalPosition(firstEl.cardContainer)
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
                .BindToLocalPosition(secondEl.cardContainer)
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