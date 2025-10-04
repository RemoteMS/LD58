using System.Collections.Generic;
using _Project.Src.Core.DI.Classes;

namespace _Project.Src.Common.HandStack
{
    public class Hand : BaseService
    {
        private int _count = 70;
        
        // todo: change on tile Type
        private List<int> _hand;
        
        public Hand()
        {
            _hand = new();
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