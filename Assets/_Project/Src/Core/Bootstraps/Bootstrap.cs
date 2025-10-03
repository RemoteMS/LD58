using _Project.Src.Core.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Src.Core.Bootstraps
{
    // Loaded by ProjectRootScope.cs
    public interface IBootstrap
    {
        UniTaskVoid Run();
    }

    public class Bootstrap : IBootstrap
    {
        private readonly ISceneLoader _loader;

        public Bootstrap(ISceneLoader loader)
        {
            _loader = loader;
            Debug.LogWarning($"Bootstrap is initializing");
        }

        public async UniTaskVoid Run()
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Bootstrap is running in Editor");
            await UniTask.CompletedTask;
            return;
#else
            Debug.LogWarning($"Bootstrap is running in not Editor");
            await _loader.LoadMainMenu();
#endif


            Debug.LogWarning($"Bootstrap is running");
            await _loader.LoadGamePlay();

            // await UniTask.CompletedTask;
        }
    }
}