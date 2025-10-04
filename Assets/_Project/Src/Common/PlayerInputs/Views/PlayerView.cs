using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using LitMotion;
using UniRx;
using UnityEngine;

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

        public PlayerView(PlayerInputStorage storage, HexSetting setting)
        {
            _pointer = Object.Instantiate(setting.hexPrefab);
            _settingHexRotationSpeed = setting.hexRotationSpeed;

            _position = _pointer.transform.position;
            _rotation = _pointer.transform.rotation;

            storage.currentCellModel.Subscribe().AddTo(this);
            storage.currentHex.Subscribe().AddTo(this);

            storage.currentHexPosition
                .Subscribe(SetPosition)
                .AddTo(this);
            storage.currentHexRotation
                .Subscribe(RotateOnIndex)
                .AddTo(this);

#if UNITY_EDITOR
            var debug = new GameObject("[DEBUG] PlayerView");
            var pvm = debug.AddComponent<PlayerViewMono>();
            pvm.Setup(storage);
#endif
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
}