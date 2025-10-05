using System;
using _Project.Src.Common.Hex;
using UniRx;
using UnityEngine;

namespace _Project.Src.Common.Towers
{
    public class TowerController : IDisposable
    {
        private readonly Tower _data;
        private readonly CellModel _cellModel;

        public Vector3 position => _data.wordPos;

        private readonly CompositeDisposable _disposables = new();

        public TowerController(Tower data, CellModel cellModel)
        {
            _data = data;
            _cellModel = cellModel;

            Debug.LogWarning($"tower on cellModel == null - {cellModel == null}, data - {data == null},");

            _cellModel.isConnectedToCenter
                .Subscribe(x => { UnityEngine.Debug.LogWarning($"Tower on {_data.hex.qrs} connected - {x};"); }
                ).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}