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

        public IReadOnlyReactiveProperty<bool> isConnectedToCenter => _isConnectedToCenter;
        private readonly ReactiveProperty<bool> _isConnectedToCenter = new(false);

        private readonly CompositeDisposable _disposables = new();

        public CellModel()
        {
            _isConnectedToCenter.Subscribe(x =>
            {
                if (x)
                {
                    _color.Value = Color.yellow;
                }
                else
                {
                    _color.Value = Color.green;
                }
            }).AddTo(_disposables);
        }

        public void SetColor(Color color)
        {
            _color.Value = color;
        }

        public void SetConnectedToCenter(bool isConnected)
        {
            _isConnectedToCenter.Value = isConnected;
        }

        public void Dispose()
        {
            _disposables.Dispose();

            _beforeDispose.OnNext(Unit.Default);
            _beforeDispose.Dispose();
            _color.Dispose();
            _isConnectedToCenter.Dispose();
        }
    }
}