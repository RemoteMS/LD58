using System.Collections.Generic;
using System.Linq;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Src.Common.GexGrid.Controllers
{
    public class HexMapController : BaseService
    {
        private readonly HexSetting _settings;
        private readonly CellSettings _cellSettings;
        private readonly HexMap _map;
        private readonly Dictionary<Hex, HexView> _views = new();

        public HexMapController(HexSetting settings, CellSettings cellSettings)
        {
            _settings = settings;
            _cellSettings = cellSettings;

            _map = new HexMap(settings);
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

        public void SetTile(Hex hex, CellModel cellModel)
        {
            if (!_map.HasTile(hex))
                _map.SetTile(hex, cellModel);
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
    }
}