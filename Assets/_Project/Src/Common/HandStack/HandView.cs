using UnityEngine;
using VContainer;
using System.Collections.Generic;
using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using LitMotion;
using UniRx;

namespace _Project.Src.Common.HandStack
{
    public class HandView : MonoBehaviour
    {
        [SerializeField] private GameObject hexPrefab;
        [SerializeField] private float heightOffset = 0.1f;
        [SerializeField] private float randomRotationMax = 30f;
        [SerializeField] private int maxVisibleTiles = 100;

        private Quaternion _rotation;
        private float _settingHexRotationSpeed;

        private readonly List<IStackHex> _tilePool = new();
        private readonly Stack<IStackHex> _activeTiles = new();
        private Hand _hand;
        private Transform _tilesParent;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void Inject(Hand hand, CellSettings cellSettings, HexSetting hexSetting, PlayerInputStorage storage)
        {
            _rotation = Quaternion.identity;
            _settingHexRotationSpeed = hexSetting.hexRotationSpeed;

            _hand = hand;
            Initialize();

            storage.invertedCameraRotation.Subscribe(SetRotation).AddTo(_disposables);

            storage.currentHexRotation.Subscribe(RotateAllOnIndex).AddTo(_disposables);
        }

        private void RotateAllOnIndex(int index)
        {
            foreach (var tile in _tilePool)
            {
                RotateOnIndex(index, tile);
            }
        }

        private void RotateOnIndex(int index, IStackHex hex)
        {
            if (index < 0 || index > 5)
            {
                Debug.LogError($"Invalid rotation index: {index}. Must be between 0 and 5.");
                return;
            }

            if (hex.HexAnimationRotationIsActive())
            {
                hex.CancelHexAnimationRotation();
            }

            var targetAngle = index * 60f;
            var targetRotation = Quaternion.Euler(0, targetAngle, 0);

            var motionHandle = LMotion.Create(_rotation, targetRotation, _settingHexRotationSpeed)
                .WithEase(Ease.InOutQuad)
                .Bind(x =>
                {
                    _rotation = x;
                    if (hex.tileRenderer)
                        hex.tileRenderer.transform.rotation = _rotation;
                });

            hex.SetMotionHandle(ref
                motionHandle
            );
        }

        private void SetRotation(Quaternion quaternion)
        {
            foreach (var tile in _tilePool)
                tile.SetCamaraRotatorRotation(quaternion);
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
                tile.Deactivate();
            }

            UpdateStack(count);
        }

        private StackHex CreateTile(int index)
        {
            var localPosition = Vector3.down * (index * heightOffset);
            var worldPosition = _tilesParent.TransformPoint(localPosition);
            var tile = Instantiate(hexPrefab, worldPosition, Quaternion.identity, _tilesParent);

            tile.name = $"Tile_{index}";

            var stackHex = tile.GetComponent<StackHex>();

            return stackHex;
        }

        private void UpdateStack(int newCount)
        {
            newCount -= 3;

            var visibleTiles = Mathf.Min(newCount, maxVisibleTiles);
            var currentActiveCount = _activeTiles.Count;

            while (currentActiveCount > visibleTiles)
            {
                if (_activeTiles.Count > 0)
                {
                    var tile = _activeTiles.Pop();
                    tile.Deactivate();
                }

                currentActiveCount--;
            }

            while (currentActiveCount < visibleTiles && _tilePool.Count > currentActiveCount)
            {
                var tile = _tilePool[currentActiveCount];
                tile.SetContainerLocalPosition(Vector3.down * (currentActiveCount * heightOffset));
                tile.Activate();
                _activeTiles.Push(tile);
                currentActiveCount++;
            }

            while (currentActiveCount < visibleTiles)
            {
                var tile = CreateTile(currentActiveCount);
                _tilePool.Add(tile);
                tile.Deactivate();
                _activeTiles.Push(tile);
                currentActiveCount++;
            }
        }

        private void ClearPool()
        {
            foreach (var tile in _tilePool)
            {
                Destroy(tile.gameObject);
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