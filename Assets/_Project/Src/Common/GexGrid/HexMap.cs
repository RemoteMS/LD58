using System;
using System.Collections.Generic;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.GexGrid
{
    public class HexMap : BaseService
    {
        public struct AddedCell
        {
            public Hex hex;
            public CellModel model;
        }

        private readonly Dictionary<Hex, CellModel> _cells;

        public IObservable<AddedCell> onCellAdded => _onCellAdded;
        private readonly Subject<AddedCell> _onCellAdded = new();

        private Layout _layout;

        public HexMap(HexSetting settings)
        {
            _cells = new();

            _layout = new Layout(
                settings.pointyTop ? Layout.Pointy : Layout.Flat,
                new Point(settings.x, settings.y),
                new Point(0,          0)
            );
        }

        public bool HasTile(Hex hex) => _cells.ContainsKey(hex);

        public void SetTile(Hex hex, CellModel cell)
        {
            _cells[hex] = cell;

            UpdateConnectedToCenterFlags();
            _onCellAdded.OnNext(new AddedCell { hex = hex, model = cell });
        }

        public void RemoveTile(Hex hex)
        {
            if (_cells.TryGetValue(hex, out var cellModel))
            {
                cellModel.Dispose();
                _cells.Remove(hex);

                UpdateConnectedToCenterFlags();
            }
        }

        public CellModel GetTile(Hex hex) => _cells.GetValueOrDefault(hex);

        public IEnumerable<Hex> GetAllCoords() => _cells.Keys;

        public Hex WorldToHex(Vector3 worldPos)
        {
            var point = new Point(worldPos.x, worldPos.z);
            return _layout.PixelToHexRounded(point);
        }

        public Vector3 HexToWorld(Hex hex)
        {
            var x = hex.q                  * 1.5f;
            var z = (hex.r + hex.q * 0.5f) * Mathf.Sqrt(3);
            return new Vector3(x, 0, z);
        }

        private void UpdateConnectedToCenterFlags()
        {
            foreach (var cell in _cells.Values)
            {
                cell.SetConnectedToCenter(false);
            }

            var center = new Hex(0, 0, 0);
            if (!_cells.ContainsKey(center))
            {
                return;
            }

            var visited = new HashSet<Hex>();
            var queue = new Queue<Hex>();
            queue.Enqueue(center);
            visited.Add(center);

            if (_cells.TryGetValue(center, out var centerCell))
            {
                centerCell.SetConnectedToCenter(true);
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                for (var i = 0; i < 6; i++)
                {
                    var neighbor = current.Neighbor(i);
                    if (_cells.ContainsKey(neighbor) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        if (_cells.TryGetValue(neighbor, out var neighborCell))
                        {
                            neighborCell.SetConnectedToCenter(true);
                        }
                    }
                }
            }
        }
    }
}