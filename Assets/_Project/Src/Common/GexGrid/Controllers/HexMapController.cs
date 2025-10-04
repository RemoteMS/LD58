using System.Collections.Generic;
using _Project.Src.Common.Hex;
using _Project.Src.Common.HexSettings;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.GexGrid.Controllers
{
    public class HexMapController : BaseService
    {
        private readonly HexSetting _settings;
        private readonly HexMap _map;
        private readonly Dictionary<Hex, HexView> _views = new();

        public HexMapController(HexSetting settings)
        {
            _settings = settings;

            _map = new HexMap(settings);
            _map.onCellAdded.Subscribe(OnCellAdded).AddTo(this);

            SetTile(new Hex(0, 0, 0), new CellModel());
        }

        private void OnCellAdded(HexMap.AddedCell cell)
        {
            var controller = new CellController(cell.model);

            var viewObject = Object.Instantiate(_settings.hexPrefab);
            var view = viewObject.GetComponent<HexView>();
            if (!view)
            {
                Debug.LogError("HexView component not found on prefab!");
                return;
            }

            view.Bind(controller);

            view.transform.position = HexToWorld(cell.hex);

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
            _map.SetTile(hex, cellModel);
        }
    }
}