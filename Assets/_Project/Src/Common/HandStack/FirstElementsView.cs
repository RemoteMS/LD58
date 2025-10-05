using _Project.Src.Common.PlayerInputs.Storages;
using UnityEngine;
using VContainer;

namespace _Project.Src.Common.HandStack
{
    public class FirstElementsView : MonoBehaviour
    {
        private PlayerInputStorage _storage;

        [Inject]
        public void Inject(PlayerInputStorage storage)
        {
            _storage = storage;
        }
    }


    public class FirstElementsViewGO : MonoBehaviour
    {
    }
}