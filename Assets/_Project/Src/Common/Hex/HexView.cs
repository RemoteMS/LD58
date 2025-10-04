using UnityEngine;

namespace _Project.Src.Common.Hex
{
    public class HexView : MonoBehaviour
    {
        private CellController _controller;

        public void Bind(CellController controller)
        {
            _controller = controller;
        }
    }
}