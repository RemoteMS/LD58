using _Project.Src.Common.HexSettings;
using _Project.Src.Common.PlayerInputs.Storages;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.PlayerInputs.Views
{
    public class PlayerView : BaseService
    {
        private readonly GameObject _pointer;

        public PlayerView(PlayerInputStorage storage, HexSetting setting)
        {
            _pointer = Object.Instantiate(setting.hexPrefab);

            storage.currentHex.Subscribe().AddTo(this);
            storage.currentHexPosition.Subscribe(x => { _pointer.transform.position = x; }).AddTo(this);

#if UNITY_EDITOR
            var debug = new GameObject("[DEBUG] PlayerView");
            var pvm = debug.AddComponent<PlayerViewMono>();
            pvm.Setup(storage);
#endif
        }


        public override void Dispose()
        {
            base.Dispose();

            if (_pointer)
            {
                Object.Destroy(_pointer);
            }
        }
    }

    public class PlayerViewMono : MonoBehaviour
    {
        [SerializeField] private string qrs;
        [SerializeField] private Vector3 position;

        public void Setup(PlayerInputStorage storage)
        {
            storage.currentHex.Subscribe(x =>
            {
                qrs = $"{x.qrs}";
                // Debug.LogWarning($"new value qrs - {qrs}. [{Time.time}]");
            }).AddTo(this);

            storage.currentHexPosition.Subscribe(x =>
            {
                position = x;
                // Debug.LogWarning($"new value position - {position}. [{Time.time}]");
            }).AddTo(this);
        }
    }
}