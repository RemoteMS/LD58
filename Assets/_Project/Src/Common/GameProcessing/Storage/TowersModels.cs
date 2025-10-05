using _Project.Src.Common.Towers;
using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.GameProcessing.Storage
{
    public class TowersModels : BaseService
    {
        public IReadOnlyReactiveCollection<Tower> towers => _towers;
        private readonly ReactiveCollection<Tower> _towers;

        public TowersModels()
        {
            _towers = new ReactiveCollection<Tower>();

            _towers.AddTo(this);
        }

        public void Add(Tower tower)
        {
            _towers.Add(tower);
        }

    }
}