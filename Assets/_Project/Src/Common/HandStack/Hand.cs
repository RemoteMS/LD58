using System;
using _Project.Src.Common.CellDatas;
using _Project.Src.Common.Hex;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;
using Random = System.Random;

namespace _Project.Src.Common.HandStack
{
    public class Hand : BaseService
    {
        private readonly PlayerInputStorage _storage;
        private readonly CellGenerationService _cellGeneration;
        public IReadOnlyReactiveProperty<int> count => _count;
        private readonly ReactiveProperty<int> _count;

        public IReadOnlyReactiveProperty<CellModel> firstInQueue => _firstInQueue;
        private ReactiveProperty<CellModel> _firstInQueue = new(null);

        public Hand(HandSettings settings, PlayerInputStorage storage, CellGenerationService cellGeneration)
        {
            _storage = storage;
            _cellGeneration = cellGeneration;

            _count = new ReactiveProperty<int>(settings.initialCount);

            storage.SetCurrentCellModel(GetRandomCellModel());
            storage.SetNextCellModel(GetRandomCellModel());
            storage.SetThirdCellModel(GetRandomCellModel());
        }

        public CellModel TakeHexFromHand()
        {
            if (count.Value == 0)
            {
                Debug.LogError($"count.Value is 0");
                throw new ArgumentException("count.Value is 0");
            }

            var value = _storage.currentCellModelInHand.Value;

            _storage.SetCurrentCellModel(_storage.secondCellModelInHand.Value);
            _storage.SetNextCellModel(_storage.thirdCellModelInHand.Value);

            if (count.Value > 3)
            {
                _storage.SetThirdCellModel(GetBest());
            }
            else
            {
                _storage.SetThirdCellModel(null);
            }

            return value;
        }

        private CellModel GetBest()
        {
            // return GetRandomCellModel();
            return _cellGeneration.GetRandomBestHexBasedNeighbors();
        }

        public CellModel GetRandomCellModel()
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
            var temp = TakeHexFromHand();
            _count.Value -= 1;
            return temp;
        }

        public void AddToHandEnd(CellModel model)
        {
            if (_storage.thirdCellModelInHand.Value == null)
            {
                _storage.SetThirdCellModel(model);
                _count.Value += 1;
                ShiftHandSlots(); // Переносим элементы после добавления
            }
            else
            {
                Debug.LogWarning("Cannot add to hand: thirdCellModelInHand is already occupied.");
            }
        }

        public void ShiftHandSlots()
        {
            bool shifted;
            do
            {
                shifted = false;

                if (_storage.secondCellModelInHand.Value == null && _storage.thirdCellModelInHand.Value != null)
                {
                    _storage.SetNextCellModel(_storage.thirdCellModelInHand.Value);
                    _storage.SetThirdCellModel(null);
                    shifted = true;
                }

                if (_storage.currentCellModelInHand.Value == null && _storage.secondCellModelInHand.Value != null)
                {
                    _storage.SetCurrentCellModel(_storage.secondCellModelInHand.Value);
                    _storage.SetNextCellModel(null);
                    shifted = true;
                }
            } while (shifted);
        }

        public void TakeFirstAndRemoveFromStack()
        {
            // Метод пока не реализован, оставлен как заглушка
        }
    }
}