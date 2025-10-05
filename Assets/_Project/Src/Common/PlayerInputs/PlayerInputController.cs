using _Project.Src.Common.Audio;
using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Common.Points;
using _Project.Src.Core.DI.Classes;
using Unity.Mathematics;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Src.Common.PlayerInputs
{
    public class PlayerInputController : BaseService, IStartable, ITickable
    {
        private readonly Camera _camera;
        private readonly CellGenerationService _cellGeneration;
        private readonly PointService _pointService;
        private readonly HexMapController _controller;
        private readonly PlayerInputStorage _storage;

        private readonly CameraMover _cameraMover;
        private readonly HexSetting _settings;
        private readonly Hand _hand;

        public PlayerInputController(
            CellGenerationService cellGeneration,
            PointService pointService,
            HexMapController controller,
            PlayerInputStorage storage,
            CameraMover cameraMover,
            HexSetting settings,
            Hand hand)
        {
            _cellGeneration = cellGeneration;
            _pointService = pointService;
            _controller = controller;
            _storage = storage;
            _cameraMover = cameraMover;
            _settings = settings;
            _hand = hand;


            _camera = Camera.main;
        }

        void IStartable.Start()
        {
        }

        void ITickable.Tick()
        {
            if (!_storage.playerHasControl.Value)
                return;

            HandleRotateCurrentHex();
            HandleCamera();

            GetAndSetCurrentHex();
            HandleHexPlacing();

            HandleAddNewTile();
        }

        private void HandleAddNewTile()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _hand.AddToHandEnd(_hand.GetRandomCellModel());
            }
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
            // ПКМ
            if (Input.GetMouseButtonDown(1))
            {
                if (TryGetWorldPoint(out var worldPoint))
                {
                    Debug.LogWarning($"worldPoint - {worldPoint}");
                    worldPoint.y = 0;
                    var hex = _controller.WorldToHex(worldPoint);

                    if (!_storage.isHexOnAvailable.Value)
                    {
                        Debug.LogWarning("Cannot place tile: Hex is not available.");
                        return;
                    }

                    if (_hand.count.Value > 0)
                    {
                        CellModel currentModel = _storage.currentCellModelInHand.Value;
                        int rotation = _storage.currentHexRotation.Value;

                        var cellModel = currentModel.Clone();
                        var (canPlace, neighborCount) = _controller.CanPlaceTile(hex, cellModel, rotation);

                        if (canPlace)
                        {
                            currentModel.SetRotation(rotation);
                            _controller.SetTile(
                                hex,
                                _hand.TakeHexFromHandAndReduceCount()
                            );

                            AudioManager.Instance.PlaySound(SoundType.TilePlace, 0.4f);

                            if (neighborCount > 1)
                            {
                                _pointService.AddPoints(neighborCount * 10);
                            }

                            if (neighborCount == 6)
                            {
                                Debug.Log("Full circle! Bonus!");
                            }
                        }
                        else
                        {
                            AudioManager.Instance.PlaySound(SoundType.OnErrorSetTile, 0.5f);
                            Debug.LogWarning(
                                $"Cannot place tile: Side types do not match with {neighborCount} neighbors.");
                        }
                    }
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