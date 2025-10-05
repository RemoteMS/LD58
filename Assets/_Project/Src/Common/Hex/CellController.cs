using _Project.Src.Common.CellDatas.Settings;
using _Project.Src.Core.DI.Classes;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.Hex
{
    public class CellController : BaseService
    {
        private readonly CellModel _model;
        private HexView _view;

        private readonly ReactiveProperty<GameObject> _tile0;
        private readonly ReactiveProperty<GameObject> _tile1;
        private readonly ReactiveProperty<GameObject> _tile2;
        private readonly ReactiveProperty<GameObject> _tile3;
        private readonly ReactiveProperty<GameObject> _tile4;
        private readonly ReactiveProperty<GameObject> _tile5;

        public IReadOnlyReactiveProperty<GameObject> tile0 => _tile0;
        public IReadOnlyReactiveProperty<GameObject> tile1 => _tile1;
        public IReadOnlyReactiveProperty<GameObject> tile2 => _tile2;
        public IReadOnlyReactiveProperty<GameObject> tile3 => _tile3;
        public IReadOnlyReactiveProperty<GameObject> tile4 => _tile4;
        public IReadOnlyReactiveProperty<GameObject> tile5 => _tile5;

        public CellController(CellModel model, CellSettings cellSettings)
        {
            _model = model;

            _tile0 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[0].Type));
            _tile1 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[1].Type));
            _tile2 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[2].Type));
            _tile3 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[3].Type));
            _tile4 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[4].Type));
            _tile5 = new ReactiveProperty<GameObject>(cellSettings.GetPartBy(model._sides[5].Type));

            _model.beforeDispose.Subscribe().AddTo(this);
        }


        public void BindView(HexView view)
        {
            _view = view;
        }
    }
}