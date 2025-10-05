using System;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.Engine
{
    public class ReactiveCollectionSynchronizerWithGameobjects<TSource, TTarget> : IDisposable
        where TSource : class
        where TTarget : MonoBehaviour
    {
        private readonly IReadOnlyReactiveCollection<TSource> _sourceCollection;
        private readonly Transform _targetTransform;
        private readonly CompositeDisposable _disposables = new();
        private readonly Func<TSource, TTarget> _targetFactory;

        public ReactiveCollectionSynchronizerWithGameobjects(
            IReadOnlyReactiveCollection<TSource> sourceCollection,
            Transform targetTransform,
            Func<TSource, TTarget> viewModelFactory)
        {
            _sourceCollection = sourceCollection ?? throw new ArgumentNullException(nameof(sourceCollection));
            _targetTransform = targetTransform   ?? throw new ArgumentNullException(nameof(targetTransform));
            _targetFactory = viewModelFactory    ?? throw new ArgumentNullException(nameof(viewModelFactory));

            Initialize();
            SubscribeToChanges();
        }

        private void Initialize()
        {
            ClearTarget(_targetTransform);

            foreach (var model in _sourceCollection)
            {
                var target = _targetFactory(model);
                target.transform.SetParent(_targetTransform, false);
            }
        }

        private void SubscribeToChanges()
        {
            _sourceCollection.ObserveAdd()
                .Subscribe(eventArgs =>
                {
                    var viewModel = _targetFactory(eventArgs.Value);
                    viewModel.transform.SetParent(_targetTransform, false);
                    viewModel.transform.SetSiblingIndex(eventArgs.Index);
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveRemove()
                .Subscribe(eventArgs =>
                {
                    if (eventArgs.Index < _targetTransform.childCount)
                    {
                        UnityEngine.Object.Destroy(_targetTransform.GetChild(eventArgs.Index).gameObject);
                    }
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveReplace()
                .Subscribe(eventArgs =>
                {
                    if (eventArgs.Index < _targetTransform.childCount)
                    {
                        UnityEngine.Object.Destroy(_targetTransform.GetChild(eventArgs.Index).gameObject);
                    }

                    var viewModel = _targetFactory(eventArgs.NewValue);
                    viewModel.transform.SetParent(_targetTransform, false);
                    viewModel.transform.SetSiblingIndex(eventArgs.Index);
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveReset()
                .Subscribe(_ =>
                {
                    ClearTarget(_targetTransform);
                    Initialize();
                })
                .AddTo(_disposables);

            _sourceCollection.ObserveMove()
                .Subscribe(eventArgs =>
                {
                    if (eventArgs.OldIndex < _targetTransform.childCount &&
                        eventArgs.NewIndex < _targetTransform.childCount)
                    {
                        var child = _targetTransform.GetChild(eventArgs.OldIndex);
                        child.SetSiblingIndex(eventArgs.NewIndex);
                    }
                })
                .AddTo(_disposables);
        }

        private void ClearTarget(Transform target)
        {
            while (target.childCount > 0)
            {
                UnityEngine.Object.Destroy(target.GetChild(0).gameObject);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}