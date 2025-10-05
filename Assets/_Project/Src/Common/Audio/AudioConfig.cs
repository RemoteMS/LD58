using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace _Project.Src.Common.Audio
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/Audio Config")]
    public class AudioConfig : ScriptableObject
    {
        public SerializedDictionary<SoundType, SoundCollection> soundCollections;
    }
}