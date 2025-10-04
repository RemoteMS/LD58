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

        public IReadOnlyReactiveProperty<Material> material1 => _material1;
        private readonly ReactiveProperty<Material> _material1;

        public IReadOnlyReactiveProperty<Material> material2 => _material2;
        private readonly ReactiveProperty<Material> _material2;

        public IReadOnlyReactiveProperty<Material> material3 => _material3;
        private readonly ReactiveProperty<Material> _material3;

        public IReadOnlyReactiveProperty<Material> material4 => _material4;
        private readonly ReactiveProperty<Material> _material4;

        public IReadOnlyReactiveProperty<Material> material5 => _material5;
        private readonly ReactiveProperty<Material> _material5;

        public IReadOnlyReactiveProperty<Material> material6 => _material6;
        private readonly ReactiveProperty<Material> _material6;

        public CellController(CellModel model, CellSettings cellSettings)
        {
            _model = model;

            _material1 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[0].Type));
            _material2 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[1].Type));
            _material3 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[2].Type));
            _material4 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[3].Type));
            _material5 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[4].Type));
            _material6 = new ReactiveProperty<Material>(cellSettings.GetMaterialBy(model._sides[5].Type));

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