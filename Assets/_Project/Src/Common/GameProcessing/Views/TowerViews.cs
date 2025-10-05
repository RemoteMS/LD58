using _Project.Src.Common.Engine;
using _Project.Src.Common.GameProcessing.Settings;
using _Project.Src.Common.Towers;
using _Project.Src.Common.Towers.Settings;
using _Project.Src.Core.DI.Classes;
using UnityEngine;

namespace _Project.Src.Common.GameProcessing.Views
{
    public class TowerViews : BaseService
    {
        private readonly ReactiveCollectionSynchronizerWithGameobjects<TowerController, TowerView> _towers;

        private Transform _towersParent;

        public TowerViews(TowersControllers controllers, TowerSpawnerSettings settings)
        {
            _towersParent = new GameObject("Towers").transform;

            _towers = new ReactiveCollectionSynchronizerWithGameobjects<TowerController, TowerView>
            (
                controllers.towerControllers,
                _towersParent,
                (controller) =>
                {
                    var towerGo = Object.Instantiate(settings.settings.towerPrefab1);

                    if (towerGo.TryGetComponent<TowerView>(out var view))
                    {
                        view.Bind(controller);
                        return view;
                    }

                    var towerView = towerGo.AddComponent<TowerView>();
                    
                    view.Bind(controller);
                    return towerView;
                }
            );
        }
    }
}