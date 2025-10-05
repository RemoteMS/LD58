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

        public CellModel GetCellBasedOnTurnProgression(int currentTurn)
        {
            // Прогрессия по ходам
            if (currentTurn > 30)
            {
                // Поздняя игра: города и разнообразие
                return GetWeightedRandomCellModel(new[]
                {
                    (SideType.Grass, 2),
                    (SideType.Forest, 3),
                    (SideType.City, 4),
                    (SideType.Sea, 1)
                });
            }
            else if (currentTurn > 20)
            {
                // Средняя игра: добавляем города
                return GetWeightedRandomCellModel(new[]
                {
                    (SideType.Grass, 3),
                    (SideType.Forest, 4),
                    (SideType.City, 2),
                    (SideType.Sea, 1)
                });
            }
            else if (currentTurn > 10)
            {
                // Ранняя-средняя игра: добавляем леса
                return GetWeightedRandomCellModel(new[]
                {
                    (SideType.Grass, 5),
                    (SideType.Forest, 3),
                    (SideType.Sea, 2)
                });
            }
            else if (currentTurn > 4)
            {
                // Очень ранняя игра: в основном трава, немного леса
                return GetWeightedRandomCellModel(new[]
                {
                    (SideType.Grass, 8),
                    (SideType.Forest, 2)
                });
            }
            else
            {
                // Самые первые ходы - только трава
                return GetFullGrass();
            }
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
            return new CellModel(0, new[]
            {
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
                new SideData(SideType.Grass),
            });
        }

        private CellModel GetWeightedRandomCellModel((SideType type, int weight)[] weightedTypes)
        {
            var sides = new SideData[6];

            for (int i = 0; i < 6; i++)
            {
                sides[i] = new SideData(GetWeightedRandomSideType(weightedTypes));
            }

            return new CellModel(0, sides);
        }

        private SideType GetWeightedRandomSideType((SideType type, int weight)[] weightedTypes)
        {
            int totalWeight = 0;
            foreach (var weightedType in weightedTypes)
            {
                totalWeight += weightedType.weight;
            }

            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var weightedType in weightedTypes)
            {
                currentWeight += weightedType.weight;
                if (randomValue < currentWeight)
                {
                    return weightedType.type;
                }
            }

            return SideType.Grass; // fallback
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