using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Settings
{
    [System.Serializable]
    public class CameraSettings
    {
        [Header("Container")] public Transform cameraContainer;
        [Header("Camera")] public Camera camera;
        public Transform cameraTransform;

        // todo: maybe move something else 
        [Header("Controller setting")] public float moveSpeed = 5f;
        public float rotationSpeed = 100f;
    }
}