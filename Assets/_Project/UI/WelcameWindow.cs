using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public class WelcameWindow : MonoBehaviour
    {
        public GameObject container;
        public Button button;

        private void Awake()
        {
            button.OnClickAsObservable()
                .Subscribe(x => container.SetActive(false)).AddTo(this);
        }
    }
}