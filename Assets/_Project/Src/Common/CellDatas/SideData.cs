using UnityEngine;

namespace _Project.Src.Common.CellDatas
{
    [System.Serializable]
    public struct SideData
    {
        [SerializeField] private Color color;
        [SerializeField] private SideType type;

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public SideType Type
        {
            get => type;
            set => type = value;
        }

        public SideData(Color color, SideType type = SideType.Default)
        {
            this.color = color;
            this.type = type;
        }
    }
}