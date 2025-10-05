using System;
using UnityEngine;

namespace _Project.Src.Common.Towers
{
    public enum TowerPrefabType
    {
        Default
    }


    [Serializable]
    public class Tower
    {
        public GexGrid.Hex hex;
        public Vector3 wordPos;

        public TowerPrefabType type;
        public string name;

        public TowerData data;
    }
}