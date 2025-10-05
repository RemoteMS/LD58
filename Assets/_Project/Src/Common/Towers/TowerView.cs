using UnityEngine;

namespace _Project.Src.Common.Towers
{
    public class TowerView : MonoBehaviour
    {
        private TowerController _controller;

        public void Bind(TowerController controller)
        {
            _controller = controller;

            gameObject.transform.position = _controller.position;
        }
    }
}