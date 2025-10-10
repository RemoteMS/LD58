using System;
using LitMotion;
using UnityEngine;

namespace _Project.Src.Common.HandStack
{
    public interface IStackHex : IDisposable
    {
        GameObject gameObject { get; }

        GameObject tileRenderer { get; }
        void Activate();
        void Deactivate();

        void SetContainerLocalPosition(Vector3 localPosition);
        void SetCamaraRotatorRotation(Quaternion localRotation);

        bool HexAnimationRotationIsActive();
        void CancelHexAnimationRotation();

        void SetMotionHandle(ref MotionHandle handle);
    }

    public class StackHex : MonoBehaviour, IStackHex
    {
        [SerializeField] private GameObject _cameraRotator;

        public GameObject tileRenderer => _tileRenderer;
        [SerializeField] private GameObject _tileRenderer;

        private MotionHandle _onHexRotateAnimation;


        public void Activate()
        {
            _tileRenderer.SetActive(true);
        }

        public void Deactivate()
        {
            _tileRenderer.SetActive(false);
        }

        public void SetCamaraRotatorRotation(Quaternion localRotation)
        {
            _cameraRotator.transform.localRotation = localRotation;
        }

        public void SetContainerLocalPosition(Vector3 localPosition)
        {
            gameObject.transform.localPosition = localPosition;
        }

        public bool HexAnimationRotationIsActive()
        {
            return _onHexRotateAnimation.IsActive();
        }

        public void CancelHexAnimationRotation()
        {
            _onHexRotateAnimation.Cancel();
        }

        public void Dispose()
        {
            if (_onHexRotateAnimation.IsActive())
            {
                _onHexRotateAnimation.Cancel();
            }
        }

        public void SetMotionHandle(ref MotionHandle handle)
        {
            _onHexRotateAnimation = handle;
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}