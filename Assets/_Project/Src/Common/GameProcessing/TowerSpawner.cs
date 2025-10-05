using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.PlayerInputs;
using _Project.Src.Common.PlayerInputs.Settings;
using _Project.Src.Common.PlayerInputs.Storages;
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

        public TowerSpawner(
            GameTurnCounter counter,
            HexMapController controller,
            PlayerInputStorage storage,
            CellGenerationService cellGeneration,
            CameraMover cameraMover,
            CameraSettings settings
        )
        {
            _controller = controller;
            _storage = storage;
            _cellGeneration = cellGeneration;
            _cameraMover = cameraMover;
            _settings = settings;

            counter.currentTurn
                .Where(x => x % 10 == 0)
                .Subscribe(_ => OnTimer())
                .AddTo(this);
        }

        private void OnTimer()
        {
            _storage.SetPlayerControl(false);
            var findHexAtDistanceFromConnected = _controller.FindRandomHexAtDistanceFromConnected(5);
            Vector3 worldPos = _controller.HexToWorld(findHexAtDistanceFromConnected);
            _controller.SetTile(findHexAtDistanceFromConnected, _cellGeneration.GetFullGrass(), increment: false);

            _cameraMover.FocusOn(
                worldPosition: worldPos,
                forTime: _settings.zoomTime,
                waitTime: _settings.waitTime,
                callback: () => { _storage.SetPlayerControl(true); }
            );
        }
    }
}