using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Common.Hex;
using _Project.Src.Common.PlayerInputs.Storages;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Project.Src.Common.HandStack
{
    public class FirstCardsInHadView : MonoBehaviour
    {
        [SerializeField] private HexView firstCardView;
        private CellController _firstCardController;

        [SerializeField] private HexView secondCardView;
        private CellController _secondController;

        [SerializeField] private HexView thirdCardView;
        private CellController _thirdController;

        [SerializeField] private float heightOffset = 3f;
        private readonly CompositeDisposable _disposables = new();

        [SerializeField] private bool moveHexes = true;

        [Inject]
        public void Inject(PlayerInputStorage storage, CellSettings settings)
        {
            if (moveHexes)
                InitializePositions();

            storage.currentCellModelInHand
                .Subscribe(x => HandleCellModel(x, firstCardView, settings, ref _firstCardController))
                .AddTo(_disposables);

            storage.secondCellModelInHand
                .Subscribe(x => HandleCellModel(x, secondCardView, settings, ref _secondController))
                .AddTo(_disposables);

            storage.thirdCellModelInHand
                .Subscribe(x => HandleCellModel(x, thirdCardView, settings, ref _thirdController))
                .AddTo(_disposables);
        }

        private void InitializePositions()
        {
            if (firstCardView)
            {
                firstCardView.transform.localPosition = Vector3.down * (0 * heightOffset);
            }


            if (secondCardView && secondCardView)
            {
                secondCardView.transform.localPosition = Vector3.down * (1 * heightOffset);
            }


            if (thirdCardView)
            {
                thirdCardView.transform.localPosition = Vector3.down * (2 * heightOffset);
            }
        }

        private void HandleCellModel(
            CellModel cellModel,
            HexView view,
            CellSettings settings,
            ref CellController controller
        )
        {
            controller?.Dispose();
            controller = null;

            if (cellModel != null)
            {
                view.EnableRendererContainer();
                controller = new CellController(cellModel, settings);
                controller.BindView(view);
                view.Bind(controller);
            }
            else
            {
                view.DisableRendererContainer();
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _firstCardController?.Dispose();
            _secondController?.Dispose();
            _thirdController?.Dispose();
        }
    }
}