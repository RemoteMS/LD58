using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.Hex;
using _Project.Src.Core.DI.Classes;
using UnityEngine;
using VContainer.Unity;

namespace _Project.Src.Common.PlayerInputs
{
    public class PlayerInputController : BaseService, IStartable, ITickable
    {
        private readonly Camera _camera;
        private readonly HexMapController _controller;

        public PlayerInputController(HexMapController controller)
        {
            _controller = controller;
            _camera = Camera.main;
        }

        void IStartable.Start()
        {
        }

        void ITickable.Tick()
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