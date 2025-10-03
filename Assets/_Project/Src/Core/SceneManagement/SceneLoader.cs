using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace _Project.Src.Core.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private readonly LifetimeScope _parent;

        public IReadOnlyReactiveProperty<SceneLoadState> sceneLoaderLoaderState => _sceneLoaderState;
        private readonly ReactiveProperty<SceneLoadState> _sceneLoaderState;

        public SceneLoader(LifetimeScope lifetimeScope)
        {
            _parent = lifetimeScope;

            _sceneLoaderState = new ReactiveProperty<SceneLoadState>(SceneLoadState.Loaded);
        }

        public async UniTask LoadGamePlay()
        {
            await LoadSceneAsync(ScenesConstants.Gameplay);
        }

        public async UniTask LoadMainMenu()
        {
            await LoadSceneAsync(ScenesConstants.MainMenu);
        }


        private async UniTask LoadSceneAsync(string sceneName)
        {
            _sceneLoaderState.Value = SceneLoadState.Loading;

            using (LifetimeScope.EnqueueParent(_parent))
            {
                await SceneManager.LoadSceneAsync(sceneName);
            }

            _sceneLoaderState.Value = SceneLoadState.Loaded;
        }

    }
}