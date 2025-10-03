using Cysharp.Threading.Tasks;
using UniRx;

namespace _Project.Src.Core.SceneManagement
{
    public interface ISceneLoader
    {
        IReadOnlyReactiveProperty<SceneLoadState> sceneLoaderLoaderState { get; }
        UniTask LoadGamePlay();
        UniTask LoadMainMenu();
    }
}