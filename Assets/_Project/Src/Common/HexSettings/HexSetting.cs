using UnityEngine;

namespace _Project.Src.Common.HexSettings
{
    [System.Serializable]
    public class HexSetting
    {
        public float x = 1;
        public float y = 1;
        public bool pointyTop = true;

        // todo: take it from array or something 
        public GameObject hexPrefab;
    }
}