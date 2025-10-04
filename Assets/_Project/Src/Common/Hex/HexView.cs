using UniRx;
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

            _controller.material1.Subscribe(x => { SetMaterialAt(1, x); }).AddTo(this);
            _controller.material2.Subscribe(x => { SetMaterialAt(2, x); }).AddTo(this);
            _controller.material3.Subscribe(x => { SetMaterialAt(3, x); }).AddTo(this);
            _controller.material4.Subscribe(x => { SetMaterialAt(4, x); }).AddTo(this);
            _controller.material5.Subscribe(x => { SetMaterialAt(5, x); }).AddTo(this);
            _controller.material6.Subscribe(x => { SetMaterialAt(6, x); }).AddTo(this);
        }

        public void UpdateColor(Color color)
        {
            if (_renderer)
            {
                _renderer.material.color = color;
            }
        }

        public void SetMaterials(Material[] newMaterials)
        {
            if (!_renderer || newMaterials == null) return;

            _renderer.materials = newMaterials;
        }

        public void SetMaterialAt(int index, Material material)
        {
            if (!_renderer || !material) return;

            var mats = _renderer.materials;
            if (index < 0 || index >= mats.Length) return;

            mats[index] = material;
            _renderer.materials = mats;
        }
    }
}