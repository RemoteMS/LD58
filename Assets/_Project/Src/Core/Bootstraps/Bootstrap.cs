using _Project.Src.Common.Audio;
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
        private readonly AudioConfig _audioConfig;

        public Bootstrap(ISceneLoader loader, AudioConfig audioConfig)
        {
            _loader = loader;
            _audioConfig = audioConfig;

            CreateAudioManager();

            InitializeAudioManager();
        }

        private void CreateAudioManager()
        {
            if (AudioManager.Instance)
            {
                Debug.LogWarning("AudioManager already exists!");
                return;
            }

            var audioManagerGameObject = new GameObject("AudioManager");
            audioManagerGameObject.AddComponent<AudioManager>();
            Object.DontDestroyOnLoad(audioManagerGameObject);

            Debug.Log("AudioManager created successfully");
        }

        private void InitializeAudioManager()
        {
            if (_audioConfig.soundCollections.Count > 0)
            {
                AudioManager.Instance.InitializeWithCollections(_audioConfig.soundCollections);
                Debug.Log($"AudioManager initialized with {_audioConfig.soundCollections} collections");
            }
            else
            {
                Debug.LogError("No sound collections provided to Bootstrap!");
            }
        }

        public async UniTaskVoid Run()
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Bootstrap is running in Editor");

            if (AudioManager.Instance && AudioManager.Instance.IsInitialized)
            {
                if (AudioManager.Instance.HasCollection(SoundType.UI))
                {
                    AudioManager.Instance.PlaySound(SoundType.UI, 0.5f);
                }
                else
                {
                    Debug.LogWarning("UI sound collection not found for testing");
                }
            }

            await UniTask.CompletedTask;
            return;
#else
            Debug.LogWarning($"Bootstrap is running in not Editor");
            await _loader.LoadMainMenu();
#endif

            Debug.LogWarning($"Bootstrap is running");
            await _loader.LoadGamePlay();
        }
    }
}