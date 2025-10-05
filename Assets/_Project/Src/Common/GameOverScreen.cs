using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.SceneManagement;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Project.Src.Common
{
    public class GameOverScreen : MonoBehaviour
    {
        public GameObject panel;
        public Button restartButton;


        [Inject]
        public void Inject(PlayerInputStorage storage, ISceneLoader sceneloader)
        {
            storage.gameOver.Subscribe(x => HandleGameOver()).AddTo(this);

            restartButton.OnClickAsObservable().Subscribe(x => { sceneloader.LoadGamePlay(); }).AddTo(this);
        }

        private void HandleGameOver()
        {
            UnityEngine.Debug.LogWarning($"GameOverScreen gameover");
            panel.SetActive(true);
        }
    }
}