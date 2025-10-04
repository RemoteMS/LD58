using _Project.Src.Common.PlayerInputs.Settings;
using _Project.Src.Core.DI.Classes;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs
{
    public class CameraMover : BaseService
    {
        private readonly CameraSettings _settings;

        private readonly Transform _cameraContainer;

        public CameraMover(CameraSettings settings)
        {
            _settings = settings;
            _cameraContainer = settings.cameraContainer;
        }

        public void SetMoveDirection(Vector3 dir)
        {
            if (dir == Vector3.zero)
                return;

            Vector3 forward = _cameraContainer.forward;
            forward.y = 0f;
            forward = forward.normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;

            var move = (forward * dir.z + right * dir.x) * _settings.moveSpeed * Time.deltaTime;

            _cameraContainer.position += move;
        }

        public void SetRotation(float rotation)
        {
            if (rotation != 0f)
            {
                var rotateAmount = rotation * _settings.rotationSpeed * Time.deltaTime;
                _cameraContainer.Rotate(Vector3.up, rotateAmount);
            }
        }
    }
}