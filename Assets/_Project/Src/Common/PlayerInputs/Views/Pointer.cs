using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Views
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        public void Bind(PointerBinder pointerBinder)
        {
            pointerBinder.material1.Subscribe(x => { SetMaterialAt(1, x); }).AddTo(this);
            pointerBinder.material2.Subscribe(x => { SetMaterialAt(2, x); }).AddTo(this);
            pointerBinder.material3.Subscribe(x => { SetMaterialAt(3, x); }).AddTo(this);
            pointerBinder.material4.Subscribe(x => { SetMaterialAt(4, x); }).AddTo(this);
            pointerBinder.material5.Subscribe(x => { SetMaterialAt(5, x); }).AddTo(this);
            pointerBinder.material6.Subscribe(x => { SetMaterialAt(6, x); }).AddTo(this);
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