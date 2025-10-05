using System;
using _Project.Src.Common.CellDatas;
using UniRx;

namespace _Project.Src.Common.Hex
{
    public class CellModel : IDisposable
    {
        public IObservable<Unit> beforeDispose => _beforeDispose;
        private readonly Subject<Unit> _beforeDispose = new();

        public IReadOnlyReactiveProperty<bool> isConnectedToCenter => _isConnectedToCenter;
        private readonly ReactiveProperty<bool> _isConnectedToCenter = new(false);

        // Data array for 6 sides of a hexagon
        public readonly SideData[] _sides;
        public IReadOnlyReactiveProperty<int> rotation => _rotation;
        private readonly ReactiveProperty<int> _rotation = new(0); // Поворот (0–5)

        public IReadOnlyReactiveProperty<bool> containsTower => _containsTower;
        private readonly ReactiveProperty<bool> _containsTower = new(false);

        private void SetContainsTower(bool val)
        {
            _containsTower.Value = val;
        }

        public CellModel()
        {
            _sides = new SideData[6];

            for (var i = 0; i < 6; i++)
            {
                _sides[i] = new SideData(SideType.Grass);
            }
        }

        public CellModel(int rotation, SideData[] data)
        {
            _sides = data;

            _rotation = new ReactiveProperty<int>(rotation);
        }


        public void SetConnectedToCenter(bool isConnected)
        {
            _isConnectedToCenter.Value = isConnected;
        }

        /// <summary>
        /// Sets the data for the specified side (without taking rotation into account).
        /// </summary>
        public void SetSideData(int sideIndex, SideData sideData)
        {
            if (sideIndex < 0 || sideIndex >= 6)
            {
                throw new ArgumentException("Side index must be between 0 and 5.");
            }

            _sides[sideIndex] = sideData;
        }

        /// <summary>
        /// Receives data from the side, taking into account the rotation.
        /// </summary>
        public SideData GetSideData(int sideIndex)
        {
            if (sideIndex < 0 || sideIndex >= 6)
            {
                throw new ArgumentException("Side index must be between 0 and 5.");
            }

            // We take into account the rotation (cyclic shift)
            var rotatedIndex = (sideIndex + _rotation.Value) % 6;
            return _sides[rotatedIndex];
        }

        /// <summary>
        /// Receives data from the party viewing the specified neighbor.
        /// </summary>
        public SideData GetSideForNeighbor(GexGrid.Hex currentHex, GexGrid.Hex neighbor)
        {
            // Find directions to a neighbor
            for (var i = 0; i < 6; i++)
            {
                if (currentHex.Neighbor(i).Equals(neighbor))
                {
                    return GetSideData(i);
                }
            }

            throw new ArgumentException("The provided hex is not a neighbor.");
        }

        /// <summary>
        /// Sets the rotation (0–5, where 1 = 60 degrees counterclockwise).
        /// </summary>
        public void SetRotation(int rotation)
        {
            if (rotation < 0 || rotation >= 6)
            {
                throw new ArgumentException("Rotation must be between 0 and 5.");
            }

            _rotation.Value = rotation;
        }

        public string GetSideTypes()
        {
            return
                $"[{_sides[0].Type}, {_sides[1].Type}, {_sides[2].Type}, {_sides[3].Type}, {_sides[4].Type}, {_sides[5].Type}]";
        }

        public void Dispose()
        {
            _beforeDispose.OnNext(Unit.Default);
            _beforeDispose.Dispose();

            _isConnectedToCenter.Dispose();
            _rotation.Dispose();
        }
    }
}