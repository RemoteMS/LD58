using UnityEngine;

namespace _Project.Src.Common.CellDatas
{
    [System.Serializable]
    public struct SideData
    {
        [SerializeField] private SideType type;

        public SideType Type
        {
            get => type;
            set => type = value;
        }

        public SideData(SideType type = SideType.Def)
        {
            this.type = type;
        }
    }
}