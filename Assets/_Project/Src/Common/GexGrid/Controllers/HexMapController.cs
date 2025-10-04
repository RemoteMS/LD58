using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.GexGrid.Controllers
{
    public class HexMapController : BaseService
    {
        private HexMap _map;

        public HexMapController()
        {
            _map = new HexMap();

            _map.onCellAdded.Subscribe(OnCelAdded).AddTo(this);
        }

        private void OnCelAdded(HexMap.AddedCell cell)
        {
            // todo: crete view
        }
    }
}