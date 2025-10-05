using UnityEngine;
using VContainer;
using System.Collections.Generic;
using UniRx;

namespace _Project.Src.Common.HandStack
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] private GameObject hexPrefab;
        [SerializeField] private float heightOffset = 0.1f;
        [SerializeField] private float randomRotationMax = 30f;
        [SerializeField] private int maxVisibleTiles = 100;

        private readonly List<GameObject> _tilePool = new();
        private readonly Stack<GameObject> _activeTiles = new();
        private Hand _hand;
        private Transform _tilesParent;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Inject(Hand hand)
        {
            _hand = hand;
            Debug.LogWarning("Hand injected");
            Initialize();
        }

        private void Initialize()
        {
            _tilesParent = new GameObject("TilesParent").transform;
            _tilesParent.SetParent(transform, false);

            _hand.count.Subscribe(UpdateStack).AddTo(_disposables);

            GenerateInitialPool();
        }

        private void GenerateInitialPool()
        {
            ClearPool();

            var count = _hand.count.Value;
            var tilesToCreate = Mathf.Min(count, maxVisibleTiles);

            for (var i = 0; i < tilesToCreate; i++)
            {
                var tile = CreateTile(i);
                _tilePool.Add(tile);
                tile.SetActive(false);
            }

            UpdateStack(count);
        }

        private GameObject CreateTile(int index)
        {
            var localPosition = Vector3.down * (index * heightOffset);
            var worldPosition = _tilesParent.TransformPoint(localPosition);
            var tile = Instantiate(hexPrefab, worldPosition, Quaternion.identity, _tilesParent);

            var randomRot = Random.Range(-randomRotationMax, randomRotationMax);
            tile.transform.Rotate(Vector3.up, randomRot);

            return tile;
        }

        private void UpdateStack(int newCount)
        {
            var visibleTiles = Mathf.Min(newCount, maxVisibleTiles);
            var currentActiveCount = _activeTiles.Count;

            while (currentActiveCount > visibleTiles)
            {
                if (_activeTiles.Count > 0)
                {
                    var tile = _activeTiles.Pop();
                    tile.SetActive(false);
                }

                currentActiveCount--;
            }

            while (currentActiveCount < visibleTiles && _tilePool.Count > currentActiveCount)
            {
                var tile = _tilePool[currentActiveCount];
                tile.transform.localPosition = Vector3.down * (currentActiveCount * heightOffset);
                tile.SetActive(true);
                _activeTiles.Push(tile);
                currentActiveCount++;
            }

            while (currentActiveCount < visibleTiles)
            {
                var tile = CreateTile(currentActiveCount);
                _tilePool.Add(tile);
                tile.SetActive(true);
                _activeTiles.Push(tile);
                currentActiveCount++;
            }
        }

        private void ClearPool()
        {
            foreach (var tile in _tilePool)
            {
                Destroy(tile);
            }

            _tilePool.Clear();
            _activeTiles.Clear();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            ClearPool();
            if (_tilesParent)
            {
                Destroy(_tilesParent.gameObject);
            }
        }
    }
}