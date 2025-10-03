using _Project.Src.Core.DI.Classes;
using _Project.Src.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace _Project.Src.Core.Inputs
{
    public class InputProvider : BaseService, IStartable, ITickable
    {
        private readonly ISceneLoader _loader;

        public InputProvider(ISceneLoader loader)
        {
            _loader = loader;
        }

        void IStartable.Start()
        {
        }

        void ITickable.Tick()
        {
            if (Input.GetKeyUp(KeyCode.G) && SceneManager.GetActiveScene().name != ScenesConstants.Gameplay)
            {
                _loader.LoadGamePlay();
            }

            if (Input.GetKeyUp(KeyCode.M) && SceneManager.GetActiveScene().name != ScenesConstants.MainMenu)
            {
                _loader.LoadMainMenu();
            }
        }
    }
}