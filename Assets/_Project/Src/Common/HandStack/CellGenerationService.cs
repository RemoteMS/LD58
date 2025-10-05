using System;
using _Project.Src.Common.CellDatas;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.Hex;
using _Project.Src.Core.DI.Classes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Src.Common.HandStack
{
    public class CellGenerationService : BaseService
    {
        private readonly HexMapController _mapController;
        private readonly CellSettings _cellSettings;

        public CellGenerationService(HexMapController mapController, CellSettings cellSettings)
        {
            _mapController = mapController;
            _cellSettings = cellSettings;
        }

        public CellModel GetRandomBestHexBasedNeighbors()
        {
            var available = _mapController.GetAllAvailableNeighborsConnectedToCenter();
            if (available == null || available.Count == 0)
            {
                Debug.LogWarning("Нет доступных ячеек для размещения.");
                throw new Exception("Нет доступных ячеек для размещения.");
            }

            var randomHex = available[Random.Range(0, available.Count)];


            var sides = GenerateSidesBasedOnNeighbors(randomHex);


            var model = new CellModel(0, sides);

            return model;
        }

        public void AddNewCell()
        {
            var available = _mapController.GetAllAvailableNeighborsConnectedToCenter();
            if (available == null || available.Count == 0)
            {
                Debug.LogWarning("Нет доступных ячеек для размещения.");
                return;
            }

            var randomHex = available[Random.Range(0, available.Count)];

            var sides = GenerateSidesBasedOnNeighbors(randomHex);

            var model = new CellModel(0, sides);

            _mapController.SetTile(randomHex, model);
        }


        public CellModel GetFullGrass()
        {
            return new CellModel(0, new []
            {
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
            });
        }
        
        private SideData[] GenerateSidesBasedOnNeighbors(GexGrid.Hex hex)
        {
            var sides = new SideData[6];

            for (var dir = 0; dir < 6; dir++)
            {
                var neighborHex = hex.Neighbor(dir);
                var neighbor = _mapController.WorldToHex(_mapController.HexToWorld(neighborHex)); // нормализация
                var neighborCell = GetCell(neighborHex);

                if (neighborCell != null)
                {
                    var oppositeDir = (dir + 3) % 6;
                    var neighborSide = neighborCell._sides[oppositeDir];
                    sides[dir] = new SideData(neighborSide.Type);
                }
                else
                {
                    sides[dir] = new SideData(GetRandomSideType());
                }
            }

            return sides;
        }

        private CellModel GetCell(GexGrid.Hex hex)
        {
            return _mapController.IsConnectedToCenter(hex) ? _mapController.GetTile(hex) : null;
        }

        private SideType GetRandomSideType()
        {
            var sideTypes = (SideType[])System.Enum.GetValues(typeof(SideType));
            return sideTypes[Random.Range(0, sideTypes.Length)];
        }
    }
}