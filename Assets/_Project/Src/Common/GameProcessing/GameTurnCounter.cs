using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.GameProcessing
{
    public class GameTurnCounter : BaseService
    {
        public IReadOnlyReactiveProperty<int> currentTurn => _currentTurn;
        private readonly ReactiveProperty<int> _currentTurn;


        public GameTurnCounter()
        {
            _currentTurn = new ReactiveProperty<int>(0);
            _currentTurn.AddTo(this);
        }

        public void Increase()
        {
            _currentTurn.Value++;
        }
    }
}