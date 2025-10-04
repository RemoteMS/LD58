using _Project.Src.Common.Hex;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Storages
{
    public class PlayerInputStorage : BaseService
    {
        public IReadOnlyReactiveProperty<CellModel> currentCellModel => _currentCellModel;
        private readonly ReactiveProperty<CellModel> _currentCellModel;

        public IReadOnlyReactiveProperty<GexGrid.Hex> currentHex => _currentHex;
        private readonly ReactiveProperty<GexGrid.Hex> _currentHex;

        public IReadOnlyReactiveProperty<Vector3> currentHexPosition => _currentHexPosition;
        private readonly ReactiveProperty<Vector3> _currentHexPosition;

        public IReadOnlyReactiveProperty<int> currentHexRotation => _currentHexRotation;
        private readonly ReactiveProperty<int> _currentHexRotation;

        public PlayerInputStorage()
        {
            _currentHex = new ReactiveProperty<GexGrid.Hex>();
            _currentHex.AddTo(this);

            _currentHexPosition = new ReactiveProperty<Vector3>();
            _currentHexPosition.AddTo(this);

            _currentHexRotation = new ReactiveProperty<int>(0);
            _currentHexRotation.AddTo(this);
            
            _currentCellModel = new ReactiveProperty<CellModel>(null);
            _currentCellModel.AddTo(this);
        }

        public void RotateHexCounterclockwise()
        {
            _currentHexRotation.Value = (_currentHexRotation.Value + 1) % 6;
        }

        public void RotateHexClockwise()
        {
            _currentHexRotation.Value = (_currentHexRotation.Value - 1 + 6) % 6;
        }

        public void SetCurrentHex(GexGrid.Hex hex)
        {
            _currentHex.Value = hex;
        }

        public void SetCurrentHexPosition(Vector3 position)
        {
            _currentHexPosition.Value = position;
        }
    }
}