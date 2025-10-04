using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Storages
{
    public class PlayerInputStorage : BaseService
    {
        public IReadOnlyReactiveProperty<GexGrid.Hex> currentHex => _currentHex;
        private readonly ReactiveProperty<GexGrid.Hex> _currentHex;

        public IReadOnlyReactiveProperty<Vector3> currentHexPosition => _currentHexPosition;
        private readonly ReactiveProperty<Vector3> _currentHexPosition;

        public PlayerInputStorage()
        {
            _currentHex = new ReactiveProperty<GexGrid.Hex>();
            _currentHex.AddTo(this);

            _currentHexPosition = new ReactiveProperty<Vector3>();
            _currentHexPosition.AddTo(this);
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