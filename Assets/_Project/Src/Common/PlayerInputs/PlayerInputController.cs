using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.Hex;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Src.Common.PlayerInputs
{
    public class PlayerInputController : BaseService, IStartable, ITickable
    {
        private readonly Camera _camera;
        private readonly HexMapController _controller;
        private readonly PlayerInputStorage _storage;

        private readonly CameraMover _cameraMover;

        public PlayerInputController(HexMapController controller, PlayerInputStorage storage, CameraMover cameraMover)
        {
            _controller = controller;
            _storage = storage;
            _cameraMover = cameraMover;


            _camera = Camera.main;
        }

        void IStartable.Start()
        {
        }

        void ITickable.Tick()
        {
            HandleCamera();

            GetAndSetCurrentHex();
            HandleHexPlacing();
        }

        private void HandleCamera()
        {
            // Movement
            var horizontal = Input.GetAxisRaw("Horizontal"); // A/D
            var vertical = Input.GetAxisRaw("Vertical");     // W/S
            var moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

            _cameraMover.SetMoveDirection(moveDirection);

            // Rotation
            var rotation = 0f;
            if (Input.GetKey(KeyCode.Q)) rotation = -1f;
            else if (Input.GetKey(KeyCode.E)) rotation = 1f;

            _cameraMover.SetRotation(rotation);
        }

        private void HandleHexPlacing()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (TryGetWorldPoint(out var worldPoint))
                {
                    Debug.LogWarning($"worldPoint - {worldPoint}");
                    worldPoint.y = 0;
                    var hex = _controller.WorldToHex(worldPoint);
                    var hexCenter = _controller.HexToWorld(hex);
                    _controller.SetTile(hex, new CellModel());
                }
            }
        }

        private void GetAndSetCurrentHex()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var distance))
            {
                var worldPoint = ray.GetPoint(distance);

                var hex = _controller.WorldToHex(worldPoint);
                _storage.SetCurrentHex(hex);

                var hexCenter = _controller.HexToWorld(hex);
                _storage.SetCurrentHexPosition(hexCenter);
            }
        }


        private bool TryGetWorldPoint(out Vector3 worldPoint)
        {
            worldPoint = Vector3.zero;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out var hit))
            {
                worldPoint = hit.point;
                return true;
            }


            if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var distance))
            {
                worldPoint = ray.GetPoint(distance);
                return true;
            }

            return false;
        }
    }
}