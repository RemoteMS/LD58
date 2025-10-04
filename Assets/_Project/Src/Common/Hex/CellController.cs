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
            
            _model.color.Subscribe(color => _view?.UpdateColor(color)).AddTo(this);
        }

        public void BindView(HexView view)
        {
            _view = view;
            
            _view.UpdateColor(_model.color.Value);
        }
    }
}