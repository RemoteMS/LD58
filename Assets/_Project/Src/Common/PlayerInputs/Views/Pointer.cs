using _Project.Src.Common.Hex;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Views
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private HexViewObjectsAnchors _anchors;
        [SerializeField] private GameObject _rendererContainer; // Аналогично HexView для активации/деактивации

        public void Bind(PointerBinder pointerBinder)
        {
            pointerBinder.material1.Subscribe(x => SetGameObject(0, x)).AddTo(this); // Индекс 0 для anchor0
            pointerBinder.material2.Subscribe(x => SetGameObject(1, x)).AddTo(this); // Индекс 1 для anchor1
            pointerBinder.material3.Subscribe(x => SetGameObject(2, x)).AddTo(this); // Индекс 2 для anchor2
            pointerBinder.material4.Subscribe(x => SetGameObject(3, x)).AddTo(this); // Индекс 3 для anchor3
            pointerBinder.material5.Subscribe(x => SetGameObject(4, x)).AddTo(this); // Индекс 4 для anchor4
            pointerBinder.material6.Subscribe(x => SetGameObject(5, x)).AddTo(this); // Индекс 5 для anchor5
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
            // Получаем якорь
            var anchor = GetAnchorByIndex(index);
            if (!anchor)
            {
                Debug.LogError($"Anchor {index} is not assigned in HexViewObjectsAnchors!");
                return;
            }

            // Очищаем существующие объекты в anchor (если есть)
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }

            // Если prefab не null, инстанцируем новый объект
            if (prefab)
            {
                var instantiated = Instantiate(prefab, anchor);
                instantiated.transform.localPosition = Vector3.zero; // Устанавливаем локальную позицию
                Debug.Log($"Instantiated object '{prefab.name}' for pointer{index} at anchor{index}");
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