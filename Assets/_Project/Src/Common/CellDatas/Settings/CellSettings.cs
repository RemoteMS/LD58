using System.Collections.Generic;
using UnityEngine;

namespace _Project.Src.Common.CellDatas.Settings
{
    [System.Serializable]
    public struct SidePair
    {
        public SideType type;
        public GameObject tilePartPrefab;
    }

    [System.Serializable]
    public class CellSettings
    {
        public AYellowpaper.SerializedCollections.SerializedDictionary<SideType, GameObject> tilePartPrefabs;


        public GameObject GetPartBy(SideType type)
        {
            return tilePartPrefabs[type];
        }
    }
}