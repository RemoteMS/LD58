using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.GameProcessing;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace _Project.Src.Common.GexGrid.Controllers
{
    public class HexMapController : BaseService
    {
        private readonly HexSetting _settings;
        private readonly CellSettings _cellSettings;
        private readonly PlayerInputStorage _playerInputStorage;
        private readonly HexMap _map;
        private readonly GameTurnCounter _turnCounter;
        private readonly Dictionary<Hex, HexView> _views = new();

        public HexMapController(
            HexSetting settings,
            CellSettings cellSettings,
            PlayerInputStorage playerInputStorage,
            HexMap map,
            GameTurnCounter turnCounter
        )
        {
            _settings = settings;
            _cellSettings = cellSettings;
            _playerInputStorage = playerInputStorage;

            _map = map;
            _turnCounter = turnCounter;
            _map.onCellAdded.Subscribe(OnCellAdded).AddTo(this);

            SetTile(new Hex(0, 0, 0), new CellModel());
        }

        private void OnCellAdded(HexMap.AddedCell cell)
        {
            var controller = new CellController(cell.model, _cellSettings);

            var viewObject = Object.Instantiate(_settings.hexPrefab);
            var view = viewObject.GetComponent<HexView>();
            if (!view)
            {
                Debug.LogError("HexView component not found on prefab!");
                return;
            }

            view.Bind(controller);

            view.transform.position = HexToWorld(cell.hex);

            cell.model.rotation.Subscribe(rotation =>
            {
                view.transform.rotation = Quaternion.Euler(0, rotation * 60f, 0);
            }).AddTo(this);

            _views[cell.hex] = view;

            view.gameObject.name = $"{cell.hex.qrs}";

            cell.model.beforeDispose.Subscribe(_ =>
            {
                if (_views.ContainsKey(cell.hex))
                {
                    Object.Destroy(_views[cell.hex].gameObject);
                    _views.Remove(cell.hex);
                }
            }).AddTo(this);
        }

        public Vector3 HexToWorld(Hex hex)
        {
            return _map.HexToWorld(hex);
        }

        public Hex WorldToHex(Vector3 worldPos)
        {
            return _map.WorldToHex(worldPos);
        }

        public void SetTile(Hex hex, CellModel cellModel, bool incrementTurn = true)
        {
            if (_map.HasTile(hex))
                return;

            if (cellModel.rotation.Value != _playerInputStorage.currentHexRotation.Value)
            {
                cellModel.SetRotation(_playerInputStorage.currentHexRotation.Value);
            }

            _map.SetTile(hex, cellModel);

            if (incrementTurn)
                IncrementViewsTurnCount();
        }

        private void IncrementViewsTurnCount()
        {
            _turnCounter.Increase();
        }

        public void RemoveTile(Hex hex)
        {
            _map.RemoveTile(hex);
        }

        public void RotateTile(Hex hex, int rotation)
        {
            var cell = _map.GetTile(hex);
            if (cell == null)
            {
                throw new System.ArgumentException($"No tile exists at hex {hex.qrs}.");
            }

            cell.SetRotation(rotation);
        }

        public bool IsConnectedToCenter(Hex hex)
        {
            var cell = _map.GetTile(hex);
            return cell != null && cell.isConnectedToCenter.Value;
        }

        public bool IsHexOnAvailable(Hex hex)
        {
            return _map.IsHexOnAvailable(hex);
        }

        public Hex FindHexAtDistanceFromConnected(int distance)
        {
            if (distance <= 0)
            {
                Debug.LogError("Distance must be positive.");
                throw new System.ArgumentException("Distance must be positive.");
            }

            var existingHexes = _views.Keys.ToList();
            if (existingHexes.Count == 0)
            {
                throw new System.ArgumentException("Map is empty.");
            }

            foreach (var hex in existingHexes)
            {
                for (var i = 0; i < 6; i++)
                {
                    var direction = Hex.Direction(i);
                    var candidate = hex.Add(direction.Scale(distance));

                    if (_views.ContainsKey(candidate))
                    {
                        continue;
                    }

                    var isValid = true;
                    foreach (var existingHex in existingHexes)
                    {
                        if (candidate.Distance(existingHex) < distance)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        return candidate;
                    }
                }
            }

            throw new System.ArgumentException("Nothing found.");
        }

        public List<Hex> FindAllHexesAtDistanceFromConnected(int distance)
        {
            if (distance <= 0)
            {
                Debug.LogError("Distance must be positive.");
                throw new System.ArgumentException("Distance must be positive.");
            }

            var existingHexes = _views.Keys.ToList();
            if (existingHexes.Count == 0)
            {
                throw new System.ArgumentException("Map is empty.");
            }

            var result = new List<Hex>();
            var checkedCandidates = new HashSet<Hex>();

            foreach (var hex in existingHexes)
            {
                for (var i = 0; i < 6; i++)
                {
                    var direction = Hex.Direction(i);
                    var candidate = hex.Add(direction.Scale(distance));

                    if (_views.ContainsKey(candidate) || checkedCandidates.Contains(candidate))
                    {
                        continue;
                    }

                    checkedCandidates.Add(candidate);

                    var isValid = true;
                    foreach (var existingHex in existingHexes)
                    {
                        if (candidate.Distance(existingHex) < distance)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        result.Add(candidate);
                    }
                }
            }

            if (result.Count == 0)
            {
                throw new System.ArgumentException("No hexes found at the specified distance.");
            }

            return result;
        }

        public Hex FindRandomHexAtDistanceFromConnected(int distance)
        {
            var hexes = FindAllHexesAtDistanceFromConnected(distance);
            return hexes[Random.Range(0, hexes.Count)];
        }

        public List<Hex> GetAvailableNeighbors(Hex hex)
        {
            var availableNeighbors = new List<Hex>();

            for (var i = 0; i < 6; i++)
            {
                var neighbor = hex.Neighbor(i);
                if (!_map.HasTile(neighbor))
                {
                    availableNeighbors.Add(neighbor);
                }
            }

            return availableNeighbors;
        }

        public List<Hex> GetAllAvailableNeighbors()
        {
            var availableNeighbors = new HashSet<Hex>();
            var existingHexes = _map.GetAllCoords();

            foreach (var hex in existingHexes)
            {
                for (var i = 0; i < 6; i++)
                {
                    var neighbor = hex.Neighbor(i);
                    if (!availableNeighbors.Contains(neighbor) && !_map.HasTile(neighbor))
                    {
                        availableNeighbors.Add(neighbor);
                    }
                }
            }

            return availableNeighbors.ToList();
        }

        public List<Hex> GetAllAvailableNeighborsConnectedToCenter()
        {
            var availableNeighbors = _map.GetAllAvailableNeighborsConnectedToCenter();
            return availableNeighbors.ToList();
        }

        public CellModel GetTile(Hex hex)
        {
            return _map.GetTile(hex);
        }

        public (bool success, int neighborCount) CanPlaceTile(Hex hex, CellModel newTile, int rotation)
        {
            // Клонируем модель, чтобы не изменять оригинал
            var clone = newTile.Clone();
            clone.SetRotation(rotation);

            // Получаем занятых соседей
            var occupiedNeighbors = new List<Hex>();
            for (var i = 0; i < 6; i++)
            {
                var neighbor = hex.Neighbor(i);
                if (_map.HasTile(neighbor))
                {
                    occupiedNeighbors.Add(neighbor);
                }
            }

            int neighborCount = occupiedNeighbors.Count;

            if (neighborCount == 0)
            {
                return (true, neighborCount);
            }

            foreach (var neighborHex in occupiedNeighbors)
            {
                var neighborTile = GetTile(neighborHex);
                if (neighborTile == null) continue;

                var newTileSideIndex = GetSideIndexForNeighbor(hex, neighborHex);
                var newTileSide = clone.GetSideData(newTileSideIndex);

                var neighborSideIndex = GetSideIndexForNeighbor(neighborHex, hex);
                var neighborSide = neighborTile.GetSideData(neighborSideIndex);

                if (newTileSide.Type != neighborSide.Type)
                {
                    Debug.LogWarning(
                        $"Cannot place tile at {hex.qrs}: Side mismatch with neighbor {neighborHex.qrs} ({newTileSide.Type} != {neighborSide.Type})");
                    return (false, neighborCount);
                }
            }

            return (true, neighborCount);
        }

        private int GetSideIndexForNeighbor(Hex from, Hex to)
        {
            for (var i = 0; i < 6; i++)
            {
                if (from.Neighbor(i).Equals(to))
                {
                    return i;
                }
            }

            throw new ArgumentException($"Hex {to.qrs} is not a neighbor of {from.qrs}");
        }
    }
}