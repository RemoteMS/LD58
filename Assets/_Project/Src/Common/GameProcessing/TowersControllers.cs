using _Project.Src.Common.Engine;
using _Project.Src.Common.GameProcessing.Storage;
using _Project.Src.Common.GexGrid.Controllers;
using _Project.Src.Common.HandStack;
using _Project.Src.Common.Towers;
using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.GameProcessing
{
    public class TowersControllers : BaseService
    {
        private ReactiveCollectionSynchronizer<Tower, TowerController> _synchronizer;

        public IReadOnlyReactiveCollection<TowerController> towerControllers => _towerControllers;
        private readonly ReactiveCollection<TowerController> _towerControllers = new();

        public TowersControllers(TowersModels models, HexMapController controller, Hand hand)
        {
            _synchronizer = new ReactiveCollectionSynchronizer<Tower, TowerController>(
                models.towers,
                _towerControllers,
                tower => new TowerController(tower, controller.GetTile(tower.hex), hand));
        }


        public override void Dispose()
        {
            foreach (var towerController in towerControllers)
            {
                towerController?.Dispose();
            }

            _towerControllers.Dispose();
            base.Dispose();
        }
    }
}