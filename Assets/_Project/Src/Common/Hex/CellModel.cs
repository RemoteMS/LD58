using System;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.Hex
{
    public class CellModel : IDisposable
    {
        public IObservable<Unit> beforeDispose => _beforeDispose;
        private readonly Subject<Unit> _beforeDispose = new();

        public IReadOnlyReactiveProperty<Color> color => _color;
        private readonly ReactiveProperty<Color> _color = new(Color.white);

        public void SetColor(Color color)
        {
            _color.Value = color;
        }

        public void Dispose()
        {
            _beforeDispose.OnNext(Unit.Default);
            _beforeDispose.Dispose();
            _color.Dispose();
        }
    }
}