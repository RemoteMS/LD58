using System.Collections.Generic;
using System.Linq;
using _Project.Src.Common.HexSettings;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

namespace _Project.Src.Common.GexGrid.Controllers
{
    public class AvailableHexesController : BaseService
    {
        private readonly HexMapController _mapController;
        private readonly HexSetting _settings;
        private readonly HexMap _map;
        private readonly ObjectPool<GameObject> _pool;

        private readonly Dictionary<Hex, GameObject> _neighborViews = new();

        public AvailableHexesController(
            HexMapController mapController,
            HexSetting settings,
            HexMap map)
        {
            _mapController = mapController;
            _settings = settings;
            _map = map;

            if (!_settings.emptyAvailableHexPrefab)
            {
                Debug.LogError("emptyAvailableHexPrefab is not set in HexSetting!");
                return;
            }

            _pool = new ObjectPool<GameObject>(
                createFunc: () => Object.Instantiate(_settings.emptyAvailableHexPrefab),
                actionOnGet: obj =>
                {
                    if (obj)
                    {
                        obj.SetActive(true);
                    }
                },
                actionOnRelease: obj =>
                {
                    if (obj)
                    {
                        obj.SetActive(false);
                    }
                },
                actionOnDestroy: obj =>
                {
                    if (obj)
                    {
                        Object.Destroy(obj);
                    }
                },
                collectionCheck: true,
                defaultCapacity: 50,
                maxSize: 10000
            );

            _map.onCellAdded.Subscribe(OnCellAdded).AddTo(this);

            UpdateAvailableNeighbors();
        }

        private void OnCellAdded(HexMap.AddedCell cell)
        {
            UpdateAvailableNeighbors();
        }


        private void UpdateAvailableNeighbors()
        {
            var newNeighbors = _mapController.GetAllAvailableNeighborsConnectedToCenter();

            var toRemove = _neighborViews.Keys.Where(hex => !newNeighbors.Contains(hex)).ToList();
            foreach (var hex in toRemove)
            {
                if (_neighborViews.TryGetValue(hex, out var view) && view)
                {
                    _pool.Release(view);
                    _neighborViews.Remove(hex);
                }
                else
                {
                    _neighborViews.Remove(hex);
                }
            }


            foreach (var hex in newNeighbors)
            {
                if (!_neighborViews.ContainsKey(hex))
                {
                    var view = _pool.Get();
                    if (view)
                    {
                        view.transform.position = _mapController.HexToWorld(hex);
                        view.transform.rotation = Quaternion.identity;
                        _neighborViews.Add(hex, view);
                    }
                }
            }
        }

        public override void Dispose()
        {
            foreach (var view in _neighborViews.Values.ToList())
            {
                if (view)
                {
                    _pool.Release(view);
                }
            }

            _neighborViews.Clear();
            _pool.Dispose();

            base.Dispose();
        }
    }
}