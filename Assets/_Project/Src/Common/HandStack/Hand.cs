using System;
using System.Collections.Generic;
using _Project.Src.Common.CellDatas;
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
            return new CellModel(0, new[]
            {
                new SideData(GetRandomSideType()),
                new SideData(GetRandomSideType()),
                new SideData(GetRandomSideType()),
                new SideData(GetRandomSideType()),
                new SideData(GetRandomSideType()),
                new SideData(GetRandomSideType()),
            });
        }

        private Random random = new Random();

        private SideType GetRandomSideType()
        {
            var sideTypes = (SideType[])Enum.GetValues(typeof(SideType));
            return sideTypes[random.Next(0, sideTypes.Length)];
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