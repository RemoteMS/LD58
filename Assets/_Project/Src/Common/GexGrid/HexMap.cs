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

        // public HexMap()
        // {
        //     _cells = new();
        //
        //     var startHex = new Hex(0, 0, 0);
        //     _cells[startHex] = new CellModel();
        // }


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

            _onCellAdded.OnNext(new AddedCell { hex = hex, model = cell });
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
    }
}