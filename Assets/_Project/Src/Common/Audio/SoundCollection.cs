using UnityEngine;

namespace _Project.Src.Common.Audio
{
    [System.Serializable]
    public class Sounds
    {
    }

    [System.Serializable]
    public class SoundCollection
    {
        public AudioClip[] clips;
        public bool preventRepeat = true;

        [HideInInspector] public int lastPlayedIndex = -1;
    }

    public enum SoundType
    {
        UI,
        TilePlace,
        Environment,
        Music,
        OnErrorSetTile,
    }
}