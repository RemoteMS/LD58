using _Project.Src.Common.Hex;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Storages
{
    public class PlayerInputStorage : BaseService
    {
        public IReadOnlyReactiveProperty<CellModel> currentCellModelInHand => _currentCellModelInHand;
        private readonly ReactiveProperty<CellModel> _currentCellModelInHand;

        public IReadOnlyReactiveProperty<CellModel> secondCellModelInHand => _secondCellModelInHand;
        private readonly ReactiveProperty<CellModel> _secondCellModelInHand;

        public IReadOnlyReactiveProperty<CellModel> thirdCellModelInHand => _thirdCellModelInHand;
        private readonly ReactiveProperty<CellModel> _thirdCellModelInHand;

        public IReadOnlyReactiveProperty<bool> isHexOnAvailable => _isHexOnAvailable;
        private readonly ReactiveProperty<bool> _isHexOnAvailable;


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

            _currentCellModelInHand = new ReactiveProperty<CellModel>(null);
            _currentCellModelInHand.AddTo(this);

            _secondCellModelInHand = new ReactiveProperty<CellModel>(null);
            _secondCellModelInHand.AddTo(this);

            _thirdCellModelInHand = new ReactiveProperty<CellModel>(null);
            _thirdCellModelInHand.AddTo(this);

            _isHexOnAvailable = new ReactiveProperty<bool>(false);
            _isHexOnAvailable.AddTo(this);
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

        public void SetCurrentCellModel(CellModel value)
        {
            _currentCellModelInHand.Value = value;
        }

        public void SetNextCellModel(CellModel value)
        {
            _secondCellModelInHand.Value = value;
        }

        public void SetThirdCellModel(CellModel value)
        {
            _thirdCellModelInHand.Value = value;
        }

        public void SetHexOnAvailable(bool value)
        {
            _isHexOnAvailable.Value = value;
        }
    }
}