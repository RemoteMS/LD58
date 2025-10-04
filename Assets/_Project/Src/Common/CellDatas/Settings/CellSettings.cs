using System.Collections.Generic;
using UnityEngine;

namespace _Project.Src.Common.CellDatas.Settings
{
    [System.Serializable]
    public struct SideMaterialPair
    {
        public SideType type;
        public Material material;
    }

    [System.Serializable]
    public class CellSettings
    {
        // todo: change on dictionary
        [SerializeField] private List<SideMaterialPair> materials = new();

        public Material GetMaterialBy(SideType type)
        {
            foreach (var pair in materials)
            {
                if (pair.type == type)
                    return pair.material;
            }

            throw new KeyNotFoundException($"Material for SideType '{type}' not found in CellSettings.");
        }
    }
}