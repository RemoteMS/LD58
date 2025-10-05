using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Project.Src.Common.GameProcessing.Views
{
    public class TurnCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text value;

        [Inject]
        public void Inject(GameTurnCounter counter)
        {
            counter.currentTurn
                .DistinctUntilChanged()
                .Subscribe(x => { value.text = x.ToString(); })
                .AddTo(this);
        }
    }
}