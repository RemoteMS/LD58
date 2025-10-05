using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Src.Common.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Settings")] [SerializeField]
        private int initialPoolSize = 10;

        [SerializeField] private SerializedDictionary<SoundType, SoundCollection> soundCollections;

        private Dictionary<SoundType, SoundCollection> _collectionsDictionary;
        private Queue<AudioSource> _audioSourcePool;
        private List<AudioSource> _activeAudioSources;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioPool();

                // Если коллекции заданы в инспекторе, инициализируемся с ними
                if (soundCollections != null && soundCollections.Count > 0)
                {
                    InitializeWithCollections(soundCollections);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;

            for (int i = _activeAudioSources.Count - 1; i >= 0; i--)
            {
                var audioSource = _activeAudioSources[i];
                if (!audioSource.isPlaying)
                {
                    ReturnAudioSourceToPool(audioSource);
                    _activeAudioSources.RemoveAt(i);
                }
            }
        }

        private void InitializeAudioPool()
        {
            _audioSourcePool = new Queue<AudioSource>();
            _activeAudioSources = new List<AudioSource>();

            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewAudioSource();
            }
        }

        /// <summary>
        /// Инициализация менеджера с коллекциями звуков (можно вызывать из кода)
        /// </summary>
        public void InitializeWithCollections(SerializedDictionary<SoundType, SoundCollection> collections)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("AudioManager already initialized!");
                return;
            }

            _collectionsDictionary = new Dictionary<SoundType, SoundCollection>();

            // Копируем коллекции в словарь
            foreach (var kvp in collections)
            {
                _collectionsDictionary[kvp.Key] = kvp.Value;
            }

            // Сохраняем ссылку на исходные коллекции
            soundCollections = collections;

            _isInitialized = true;
            Debug.Log($"AudioManager initialized with {collections.Count} collections");
        }

        /// <summary>
        /// Добавление отдельной коллекции после инициализации
        /// </summary>
        public void AddCollection(SoundType soundType, SoundCollection collection)
        {
            if (!_isInitialized)
            {
                Debug.LogError("AudioManager not initialized! Call InitializeWithCollections first.");
                return;
            }

            _collectionsDictionary[soundType] = collection;
            soundCollections[soundType] = collection;
        }

        private AudioSource CreateNewAudioSource()
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            _audioSourcePool.Enqueue(audioSource);
            return audioSource;
        }

        private AudioSource GetAudioSourceFromPool()
        {
            if (_audioSourcePool.Count == 0)
            {
                CreateNewAudioSource();
            }

            return _audioSourcePool.Dequeue();
        }

        private void ReturnAudioSourceToPool(AudioSource audioSource)
        {
            audioSource.clip = null;
            _audioSourcePool.Enqueue(audioSource);
        }

        public void PlaySound(SoundType soundType, float volume = 1f, float pitch = 1f)
        {
            if (!_isInitialized)
            {
                Debug.LogError("AudioManager not initialized!");
                return;
            }

            if (!_collectionsDictionary.ContainsKey(soundType))
            {
                Debug.LogWarning($"Sound collection '{soundType}' not found!");
                return;
            }

            var collection = _collectionsDictionary[soundType];

            if (collection.clips == null || collection.clips.Length == 0)
            {
                Debug.LogWarning($"Sound collection '{soundType}' is empty!");
                return;
            }

            AudioClip clipToPlay = GetRandomClipFromCollection(collection);
            if (clipToPlay == null) return;

            var audioSource = GetAudioSourceFromPool();
            audioSource.clip = clipToPlay;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();

            _activeAudioSources.Add(audioSource);
        }

        public void PlaySoundAtPosition(SoundType soundType, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!_isInitialized)
            {
                Debug.LogError("AudioManager not initialized!");
                return;
            }

            if (!_collectionsDictionary.ContainsKey(soundType))
            {
                Debug.LogWarning($"Sound collection '{soundType}' not found!");
                return;
            }

            var collection = _collectionsDictionary[soundType];

            if (collection.clips == null || collection.clips.Length == 0)
            {
                Debug.LogWarning($"Sound collection '{soundType}' is empty!");
                return;
            }

            AudioClip clipToPlay = GetRandomClipFromCollection(collection);
            if (clipToPlay == null) return;

            AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
        }

        private AudioClip GetRandomClipFromCollection(SoundCollection collection)
        {
            if (collection.clips.Length == 1)
            {
                return collection.clips[0];
            }

            if (collection.preventRepeat && collection.clips.Length > 1)
            {
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, collection.clips.Length);
                } while (randomIndex == collection.lastPlayedIndex);

                collection.lastPlayedIndex = randomIndex;
                return collection.clips[randomIndex];
            }
            else
            {
                return collection.clips[Random.Range(0, collection.clips.Length)];
            }
        }

        public void StopAllSounds()
        {
            if (!_isInitialized) return;

            foreach (var audioSource in _activeAudioSources)
            {
                audioSource.Stop();
                ReturnAudioSourceToPool(audioSource);
            }

            _activeAudioSources.Clear();
        }

        public bool HasCollection(SoundType soundType)
        {
            return _isInitialized && _collectionsDictionary.ContainsKey(soundType);
        }

        public int GetCollectionSize(SoundType soundType)
        {
            return _isInitialized && _collectionsDictionary.TryGetValue(soundType, out var collection)
                ? (collection.clips?.Length ?? 0)
                : 0;
        }

        /// <summary>
        /// Проверяет, инициализирован ли AudioManager
        /// </summary>
        public bool IsInitialized => _isInitialized;
    }
}