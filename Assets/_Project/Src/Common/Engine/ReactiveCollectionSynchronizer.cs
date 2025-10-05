using System;
using UniRx;

namespace _Project.Src.Common.Engine
{
    public class ReactiveCollectionSynchronizer<TSource, TTarget> : IDisposable
        where TSource : class
        where TTarget : class
    {
        private readonly IReadOnlyReactiveCollection<TSource> _sourceCollection;
        private readonly ReactiveCollection<TTarget> _targetCollection;
        private readonly CompositeDisposable _disposables = new();
        private readonly Func<TSource, TTarget> _targetFactory;

        public ReactiveCollectionSynchronizer(
            IReadOnlyReactiveCollection<TSource> sourceCollection,
            ReactiveCollection<TTarget> targetCollection,
            Func<TSource, TTarget> viewModelFactory)
        {
            _sourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
            _targetCollection = targetCollection ?? throw new ArgumentNullException(nameof(targetCollection));
            _targetFactory = viewModelFactory    ?? throw new ArgumentNullException(nameof(viewModelFactory));

            Initialize();

            SubscribeToChanges();
        }

        private void Initialize()
        {
            _targetCollection.Clear();
            foreach (var model in _sourceCollection)
            {
                _targetCollection.Add(_targetFactory(model));
            }
        }

        private void SubscribeToChanges()
        {
            _sourceCollection.ObserveAdd()
                .Subscribe(eventArgs =>
                {
                    var viewModel = _targetFactory(eventArgs.Value);
                    _targetCollection.Insert(eventArgs.Index, viewModel);
                })
                .AddTo(_disposables);


            _sourceCollection.ObserveRemove()
                .Subscribe(eventArgs =>
                {
                    if (eventArgs.Index < _targetCollection.Count)
                    {
                        _targetCollection.RemoveAt(eventArgs.Index);
                    }
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveReplace()
                .Subscribe(eventArgs => { _targetCollection[eventArgs.Index] = _targetFactory(eventArgs.NewValue); })
                .AddTo(_disposables);

            _sourceCollection.ObserveReset()
                .Subscribe(_ =>
                {
                    _targetCollection.Clear();
                    Initialize();
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveMove()
                .Subscribe(eventArgs => { _targetCollection.Move(eventArgs.OldIndex, eventArgs.NewIndex); })
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}