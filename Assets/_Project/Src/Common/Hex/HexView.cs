using UniRx;
using UnityEngine;

namespace _Project.Src.Common.Hex
{
    public class HexView : MonoBehaviour
    {
        [SerializeField] private HexViewObjectsAnchors _anchors;
        [SerializeField] private GameObject _rendererContainer;
        private CellController _controller;

        public void Bind(CellController controller)
        {
            _controller = controller;
            _controller.BindView(this);

            _controller.tile0.Subscribe(x => SetGameObject(0, x)).AddTo(this);
            _controller.tile1.Subscribe(x => SetGameObject(1, x)).AddTo(this);
            _controller.tile2.Subscribe(x => SetGameObject(2, x)).AddTo(this);
            _controller.tile3.Subscribe(x => SetGameObject(3, x)).AddTo(this);
            _controller.tile4.Subscribe(x => SetGameObject(4, x)).AddTo(this);
            _controller.tile5.Subscribe(x => SetGameObject(5, x)).AddTo(this);
        }

        public void EnableRendererContainer()
        {
            _rendererContainer.SetActive(true);
        }

        public void DisableRendererContainer()
        {
            _rendererContainer.SetActive(false);
        }

        public void SetGameObject(int index, GameObject prefab)
        {
            var anchor = GetAnchorByIndex(index);
            if (!anchor)
            {
                Debug.LogError($"Anchor {index} is not assigned in HexViewObjectsAnchors!");
                return;
            }

            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }

            if (prefab)
            {
                var instantiated = Instantiate(prefab, anchor);
                instantiated.transform.localPosition = Vector3.zero;
            }
        }

        private Transform GetAnchorByIndex(int index)
        {
            switch (index)
            {
                case 0: return _anchors.anchor0;
                case 1: return _anchors.anchor1;
                case 2: return _anchors.anchor2;
                case 3: return _anchors.anchor3;
                case 4: return _anchors.anchor4;
                case 5: return _anchors.anchor5;
                default:
                    Debug.LogError($"Invalid anchor index: {index}");
                    return null;
            }
        }
    }
}