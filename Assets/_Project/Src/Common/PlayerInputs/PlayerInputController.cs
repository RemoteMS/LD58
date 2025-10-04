using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
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
        private readonly HexSetting _settings;

        public PlayerInputController(HexMapController controller, PlayerInputStorage storage, CameraMover cameraMover,
            HexSetting settings)
        {
            _controller = controller;
            _storage = storage;
            _cameraMover = cameraMover;
            _settings = settings;


            _camera = Camera.main;
        }

        void IStartable.Start()
        {
        }

        void ITickable.Tick()
        {
            HandleRotateCurrentHex();
            HandleCamera();

            GetAndSetCurrentHex();
            HandleHexPlacing();

            HandleAddTower();
        }

        private void HandleRotateCurrentHex()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _storage.RotateHexClockwise();
            }
            // todo: change input key
            else if (Input.GetKeyDown(KeyCode.T))
            {
                _storage.RotateHexCounterclockwise();
            }
        }

        private void HandleAddTower()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                var findHexAtDistanceFromConnected = _controller.FindRandomHexAtDistanceFromConnected(5);

                Debug.LogWarning($"random hex - {findHexAtDistanceFromConnected.qrs}");
                _controller.SetTile(findHexAtDistanceFromConnected, new CellModel());
            }

            if (Input.GetKeyUp(KeyCode.Z))
            {
                var allAvailableNeighbors = _controller.GetAllAvailableNeighbors();
                foreach (var neighbor in allAvailableNeighbors)
                {
                    Object.Instantiate(
                        _settings.emptyAvailableHexPrefab,
                        _controller.HexToWorld(neighbor),
                        Quaternion.identity
                    );
                }
            }
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

            // Zoom/Movement with mouse wheel or X/C
            var zoomInput = Input.GetAxis("Mouse ScrollWheel");
            if (Input.GetKey(KeyCode.X)) zoomInput = 1f;
            else if (Input.GetKey(KeyCode.C)) zoomInput = -1f;

            _cameraMover.SetZoom(zoomInput);
        }

        private void HandleHexPlacing()
        {
            // пкм
            if (Input.GetMouseButtonDown(1))
            {
                if (TryGetWorldPoint(out var worldPoint))
                {
                    Debug.LogWarning($"worldPoint - {worldPoint}");
                    worldPoint.y = 0;
                    var hex = _controller.WorldToHex(worldPoint);

                    if (!_storage.isHexOnAvailable.Value)
                    {
                        return;
                    }

                    var hexCenter = _controller.HexToWorld(hex);
                    _controller.SetTile(
                        hex,
                        _storage.currentCellModel.Value
                        // new CellModel(rotation: _storage.currentHexRotation.Value)
                    );
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

                var isHexOnAvailable = _controller.IsHexOnAvailable(hex);
                _storage.SetHexOnAvailable(isHexOnAvailable);
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