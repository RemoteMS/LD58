using System;
using UniRx;

namespace _Project.Src.Common.Hex
{
    public class CellModel : IDisposable
    {
        public IObservable<Unit> beforeDispose => _beforeDispose;
        private readonly Subject<Unit> _beforeDispose = new();

        public void Dispose()
        {
            _beforeDispose.OnNext(Unit.Default);
        }
    }
}