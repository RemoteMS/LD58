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

        public readonly SideData[] _sides;
        public IReadOnlyReactiveProperty<int> rotation => _rotation;
        private readonly ReactiveProperty<int> _rotation = new(0); // Поворот (0–5, по часовой стрелке)

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

        public CellModel Clone()
        {
            var clonedSides = new SideData[6];
            Array.Copy(_sides, clonedSides, 6);
            return new CellModel(_rotation.Value, clonedSides);
        }

        public void SetConnectedToCenter(bool isConnected)
        {
            _isConnectedToCenter.Value = isConnected;
        }

        public void SetSideData(int sideIndex, SideData sideData)
        {
            if (sideIndex < 0 || sideIndex >= 6)
            {
                throw new ArgumentException("Side index must be between 0 and 5.");
            }

            _sides[sideIndex] = sideData;
        }

        public SideData GetSideData(int sideIndex)
        {
            if (sideIndex < 0 || sideIndex >= 6)
            {
                throw new ArgumentException("Side index must be between 0 and 5.");
            }

            // Поворот по часовой стрелке: сдвиг влево (вычитаем rotation)
            var rotatedIndex = (sideIndex - _rotation.Value + 6) % 6;
            return _sides[rotatedIndex];
        }

        public SideData GetSideForNeighbor(GexGrid.Hex currentHex, GexGrid.Hex neighbor)
        {
            for (var i = 0; i < 6; i++)
            {
                if (currentHex.Neighbor(i).Equals(neighbor))
                {
                    return GetSideData(i);
                }
            }

            throw new ArgumentException("The provided hex is not a neighbor.");
        }

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