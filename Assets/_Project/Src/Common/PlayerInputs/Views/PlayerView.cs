using System;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using LitMotion;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Src.Common.PlayerInputs.Views
{
    public class PlayerView : BaseService
    {
        private readonly GameObject _pointer;

        // todo: move to another settings
        private float _settingHexRotationSpeed;
        private MotionHandle _rotationHandle;

        private Vector3 _position;
        private Quaternion _rotation;


        private readonly PointerBinder _pointerBinder;
        private readonly Vector3 _hiddenPosition = new Vector3(0, 1000, 0);

        public PlayerView(PlayerInputStorage storage, HexSetting setting, CellSettings cellSettings, Hand hand)
        {
            _pointer = Object.Instantiate(setting.pointerPrefab);
            _settingHexRotationSpeed = setting.hexRotationSpeed;

            _pointerBinder = new PointerBinder(cellSettings, _pointer);
            _pointerBinder.AddTo(this);

            _position = _pointer.transform.position;
            _rotation = _pointer.transform.rotation;

            storage.currentCellModelInHand.Subscribe(x => OnCellModelChange(x)).AddTo(this);
            storage.currentHexRotation.Subscribe(RotateOnIndex).AddTo(this);


            storage.currentHexPosition
                .CombineLatest(
                    storage.isHexOnAvailable,
                    hand.count,
                    (pos, isAvailable, handCount) => (pos, isAvailable, handCount)
                )
                .Subscribe(x => SetPosition(x.isAvailable && x.handCount > 0 ? x.pos : _hiddenPosition))
                .AddTo(this);

#if UNITY_EDITOR
            var debug = new GameObject("[DEBUG] PlayerView");
            var pvm = debug.AddComponent<PlayerViewMono>();
            pvm.Setup(storage);
#endif
        }

        private void OnCellModelChange(CellModel cellModel)
        {
            if (cellModel == null)
            {
                _pointerBinder.Unbind();
            }
            else
            {
                _pointerBinder.Bind(cellModel);
            }
        }


        private void SetPosition(Vector3 pos)
        {
            _position = pos;
            if (_pointer)
                _pointer.transform.position = _position;
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
                    if (_pointer)
                        _pointer.transform.rotation = _rotation;
                });
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_rotationHandle.IsActive())
            {
                _rotationHandle.Cancel();
            }

            if (_pointer)
            {
                Object.Destroy(_pointer);
            }
        }
    }


    public class PointerBinder : IDisposable
    {
        private readonly GameObject _pointer;
        private readonly CellSettings _cellSettings;

        private readonly CompositeDisposable _disposables = new();

        public IReadOnlyReactiveProperty<GameObject> material1 => _material1;
        private readonly ReactiveProperty<GameObject> _material1;

        public IReadOnlyReactiveProperty<GameObject> material2 => _material2;
        private readonly ReactiveProperty<GameObject> _material2;

        public IReadOnlyReactiveProperty<GameObject> material3 => _material3;
        private readonly ReactiveProperty<GameObject> _material3;

        public IReadOnlyReactiveProperty<GameObject> material4 => _material4;
        private readonly ReactiveProperty<GameObject> _material4;

        public IReadOnlyReactiveProperty<GameObject> material5 => _material5;
        private readonly ReactiveProperty<GameObject> _material5;

        public IReadOnlyReactiveProperty<GameObject> material6 => _material6;
        private readonly ReactiveProperty<GameObject> _material6;

        public PointerBinder(CellSettings cellSettings, GameObject pointer)
        {
            _cellSettings = cellSettings;
            _pointer = pointer;

            _material1 = new ReactiveProperty<GameObject>().AddTo(_disposables);
            _material2 = new ReactiveProperty<GameObject>().AddTo(_disposables);
            _material3 = new ReactiveProperty<GameObject>().AddTo(_disposables);
            _material4 = new ReactiveProperty<GameObject>().AddTo(_disposables);
            _material5 = new ReactiveProperty<GameObject>().AddTo(_disposables);
            _material6 = new ReactiveProperty<GameObject>().AddTo(_disposables);

            if (_pointer.TryGetComponent<Pointer>(out var pointerPointer))
            {
                pointerPointer.Bind(this);
            }
        }

        public void Bind(CellModel model)
        {
            _material1.Value = _cellSettings.GetPartBy(model._sides[0].Type);
            _material2.Value = _cellSettings.GetPartBy(model._sides[1].Type);
            _material3.Value = _cellSettings.GetPartBy(model._sides[2].Type);
            _material4.Value = _cellSettings.GetPartBy(model._sides[3].Type);
            _material5.Value = _cellSettings.GetPartBy(model._sides[4].Type);
            _material6.Value = _cellSettings.GetPartBy(model._sides[5].Type);
        }

        public void Unbind()
        {
            _material1.Value = null;
            _material2.Value = null;
            _material3.Value = null;
            _material4.Value = null;
            _material5.Value = null;
            _material6.Value = null;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}