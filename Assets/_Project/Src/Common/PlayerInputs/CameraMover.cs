using _Project.Src.Common.PlayerInputs.Settings;
using _Project.Src.Core.DI.Classes;
using LitMotion;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs
{
    public class CameraMover : BaseService
    {
        private readonly CameraSettings _settings;
        private readonly Transform _cameraContainer;

        private CancellationTokenSource _focusCts;

        public CameraMover(CameraSettings settings)
        {
            _settings = settings;
            _cameraContainer = settings.cameraContainer;
        }

        public void FocusOn(Vector3 worldPosition, float forTime = 0.5f, float waitTime = 1f)
        {
            FocusOn(worldPosition, forTime, waitTime, null);
        }

        public void FocusOn(Vector3 worldPosition, float forTime = 0.5f, float waitTime = 1f, Action callback = null)
        {
            CancelCurrentFocus();

            _focusCts = new CancellationTokenSource();

            FocusSequence(worldPosition, forTime, waitTime, callback, _focusCts.Token).Forget();
        }

        public void OnCallBack(Action callback)
        {
            if (_focusCts != null && !_focusCts.Token.IsCancellationRequested)
            {
                var cts = new CancellationTokenSource();
                _focusCts = cts;

                CallbackAfterFocus(callback, cts.Token).Forget();
            }
            else
            {
                callback?.Invoke();
            }
        }

        private async UniTaskVoid FocusSequence(Vector3 worldPosition, float forTime, float waitTime, Action callback,
            CancellationToken cancellationToken)
        {
            try
            {
                Vector3 startPosition = _cameraContainer.position;

                await LMotion.Create(startPosition, worldPosition, forTime)
                    // .WithEase(_settings.easing)
                    .Bind(newPosition => _cameraContainer.position = newPosition)
                    .ToUniTask(cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                callback?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _focusCts?.Dispose();
                    _focusCts = null;
                }
            }
        }

        private async UniTaskVoid CallbackAfterFocus(Action callback, CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.NextFrame(cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                callback?.Invoke();
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _focusCts?.Dispose();
                    _focusCts = null;
                }
            }
        }

        private void CancelCurrentFocus()
        {
            _focusCts?.Cancel();
            _focusCts?.Dispose();
            _focusCts = null;
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