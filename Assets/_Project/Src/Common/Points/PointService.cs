using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.Points
{
    public class PointService : BaseService
    {
        public IReadOnlyReactiveProperty<int> score => _score;
        private readonly ReactiveProperty<int> _score;


        public PointService()
        {
            _score = new ReactiveProperty<int>(0);
        }

        public void AddPoints(int count)
        {
            _score.Value += count;
        }
    }
}