using UnityEngine;

namespace _Project.Src.Common.Hex
{
    public class HexView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        private CellController _controller;

        public void Bind(CellController controller)
        {
            _controller = controller;
            _controller.BindView(this);
        }

        public void UpdateColor(Color color)
        {
            if (_renderer)
            {
                _renderer.material.color = color;
            }
        }
    }
}