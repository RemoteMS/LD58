using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.Hex
{
    public class CellController : BaseService
    {
        private readonly CellModel _model;
        private HexView _view;

        public CellController(CellModel model)
        {
            _model = model;

            _model.beforeDispose.Subscribe().AddTo(this);
        }
    }
}