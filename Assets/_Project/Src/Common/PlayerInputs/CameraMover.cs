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

        public void SetZoom(float zoom)
        {
            if (zoom != 0f)
            {
                var distance = zoom * _settings.zoomSpeed * Time.deltaTime;
                MoveCameraLocally(distance);
            }
        }

        private void MoveCameraLocally(float distance)
        {
            var localMove = _settings.cameraTransform.TransformDirection(Vector3.forward) * distance;
            var newPosition = _settings.cameraTransform.position + localMove;
            var currentY = _settings.cameraTransform.position.y;
            var newY = newPosition.y;

            // Only apply movement if it doesn't exceed limits
            if (newY >= 0.5f && newY <= 100f)
            {
                _settings.cameraTransform.position = newPosition;
            }
            else
            {
                // If moving towards limit, adjust to the boundary
                if (newY < _settings.zoomMin && distance < 0)
                    _settings.cameraTransform.position = new Vector3(newPosition.x, _settings.zoomMin, newPosition.z);
                else if (newY > _settings.zoomMax && distance > 0)
                    _settings.cameraTransform.position = new Vector3(newPosition.x, _settings.zoomMax, newPosition.z);
            }
        }
    }
}