using System.Collections.Generic;
using _Project.Src.Common.Hex;
using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.HandStack
{
    public class Hand : BaseService
    {
        public IReadOnlyReactiveProperty<int> count => _count;
        private readonly ReactiveProperty<int> _count;


        private List<int> _hand;

        public Hand(HandSettings settings)
        {
            _count = new ReactiveProperty<int>(settings.initialCount);
            _hand = new();
        }

        public CellModel TakeHexFromHand()
        {
            return new CellModel();
        }

        public CellModel TakeHexFromHandAndReduceCount()
        {
            _count.Value -= 1;
            return TakeHexFromHand();
        }

        // todo: 
        public void AddToHandEnd()
        {
        }

        public void TakeFirstAndRemoveFromStack()
        {
        }
    }
}