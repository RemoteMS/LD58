using System;

namespace _Project.Src.Common.Towers
{
    public enum TowerPrefabType
    {
        Default
    }


    [Serializable]
    public class Tower
    {
        public TowerPrefabType type;
        public string name;

        public TowerData data;
    }
}