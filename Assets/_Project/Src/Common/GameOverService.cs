using _Project.Src.Common.HandStack;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common
{
    public class GameOverService : BaseService
    {
        private readonly PlayerInputStorage _storage;

        public GameOverService(PlayerInputStorage storage, Hand hand)
        {
            _storage = storage;

            hand.count
                .Where(x => 0 == x)
                .Subscribe(x => HandleGameOver())
                .AddTo(this);
        }

        private void HandleGameOver()
        {
            _storage.SetPlayerControl(false);
            _storage.SetGameOver();
            UnityEngine.Debug.LogWarning($"gameover");
        }
    }
}