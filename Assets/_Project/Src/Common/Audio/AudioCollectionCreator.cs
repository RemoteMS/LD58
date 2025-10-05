namespace _Project.Src.Common.Audio
{
    using UnityEngine;

    namespace _Project.Src.Common.Audio
    {
        public static class AudioCollectionCreator
        {

            public static SoundCollection CreateCollection(SoundType name, AudioClip[] clips,
                bool preventRepeat = true)
            {
                return new SoundCollection
                {
                    clips = clips,
                    preventRepeat = preventRepeat
                };
            }


            public static SoundCollection CreateCollectionFromResources(SoundType name, string resourcesPath,
                bool preventRepeat = true)
            {
                var clips = Resources.LoadAll<AudioClip>(resourcesPath);
                if (clips.Length == 0)
                {
                    Debug.LogWarning($"No audio clips found at path: {resourcesPath}");
                }

                return CreateCollection(name, clips, preventRepeat);
            }
        }
    }
}