using _Project.Src.Common.PlayerInputs.Storages;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Views
{
    public class PlayerViewMono : MonoBehaviour
    {
        [SerializeField] private int currentHexRotation;
        [SerializeField] private string qrs;
        [SerializeField] private Vector3 position;

        public void Setup(PlayerInputStorage storage)
        {
            qrs = storage.currentHex.Value.qrs;
            storage.currentHex.Subscribe(x => { qrs = $"{x.qrs}"; }).AddTo(this);

            position = storage.currentHexPosition.Value;
            storage.currentHexPosition.Subscribe(x => { position = x; }).AddTo(this);

            currentHexRotation = storage.currentHexRotation.Value;
            storage.currentHexRotation.Subscribe(x => { currentHexRotation = x; }).AddTo(this);
        }
    }
}