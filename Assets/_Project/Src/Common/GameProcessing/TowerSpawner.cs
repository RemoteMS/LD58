using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.PlayerInputs;
using _Project.Src.Common.PlayerInputs.Settings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Common.Towers;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.GameProcessing
{
    public class TowerSpawner : BaseService
    {
        private readonly HexMapController _controller;
        private readonly PlayerInputStorage _storage;
        private readonly CellGenerationService _cellGeneration;
        private readonly CameraMover _cameraMover;
        private readonly CameraSettings _settings;
        private readonly Storage.TowersModels _towersModels;

        public TowerSpawner(
            GameTurnCounter counter,
            HexMapController controller,
            PlayerInputStorage storage,
            CellGenerationService cellGeneration,
            CameraMover cameraMover,
            CameraSettings settings,
            Storage.TowersModels towersModels
        )
        {
            _controller = controller;
            _storage = storage;
            _cellGeneration = cellGeneration;
            _cameraMover = cameraMover;
            _settings = settings;
            _towersModels = towersModels;

            counter.currentTurn
                .Where(x => x % 10 == 0)
                .Subscribe(_ => OnTimer())
                .AddTo(this);
        }

        private void OnTimer()
        {
            _storage.SetPlayerControl(false);
            var findHexAtDistanceFromConnected = _controller.FindRandomHexAtDistanceFromConnected(5);
            var worldPos = _controller.HexToWorld(findHexAtDistanceFromConnected);

            var tower = new Tower()
            {
                hex = findHexAtDistanceFromConnected,
                wordPos = worldPos,
            };


            _controller.SetTile(
                findHexAtDistanceFromConnected,
                _cellGeneration.GetFullGrass(),
                incrementTurn: false
            );

            _towersModels.Add(tower);

            _cameraMover.FocusOn(
                worldPosition: worldPos,
                forTime: _settings.zoomTime,
                waitTime: _settings.waitTime,
                callback: () => { _storage.SetPlayerControl(true); }
            );
        }
    }
}