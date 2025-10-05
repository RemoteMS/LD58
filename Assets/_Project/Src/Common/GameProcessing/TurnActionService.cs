using System;
using System.Collections.Generic;
using _Project.Src.Core.DI.Classes;
using UniRx;

namespace _Project.Src.Common.GameProcessing
{
    public class TurnActionService : BaseService
    {
        private readonly GameTurnCounter _counter;
        private readonly CompositeDisposable _turnSubs = new();
        private readonly Dictionary<string, IDisposable> _eventSubscriptions = new();

        public TurnActionService(GameTurnCounter counter)
        {
            _counter = counter;
            _turnSubs.AddTo(this);
        }

        public string AddEventOnTurn(int turn, Action action, string eventId = null)
        {
            eventId ??= Guid.NewGuid().ToString();
            var subscription = _counter.currentTurn
                .Where(x => x == turn)
                .Take(1) // Works only once
                .Subscribe(_ => HandleAction(action, eventId))
                .AddTo(_turnSubs);

            _eventSubscriptions[eventId] = subscription;
            return eventId;
        }

        // Adding a recurring event at a specific interval
        public string AddRecurringEvent(int startTurn, int interval, Action action, string eventId = null)
        {
            eventId ??= Guid.NewGuid().ToString();
            var subscription = _counter.currentTurn
                .Where(x => x >= startTurn && (x - startTurn) % interval == 0)
                .Subscribe(_ => HandleAction(action, eventId))
                .AddTo(_turnSubs);

            _eventSubscriptions[eventId] = subscription;
            return eventId;
        }

        // Добавление события с пользовательским условием
        public string AddConditionalEvent(Func<int, bool> condition, Action action, string eventId = null)
        {
            eventId ??= Guid.NewGuid().ToString();
            var subscription = _counter.currentTurn
                .Where(condition)
                .Subscribe(_ => HandleAction(action, eventId))
                .AddTo(_turnSubs);

            _eventSubscriptions[eventId] = subscription;
            return eventId;
        }

        // Удаление события по его ID
        public void RemoveEvent(string eventId)
        {
            if (_eventSubscriptions.TryGetValue(eventId, out var subscription))
            {
                subscription.Dispose();
                _eventSubscriptions.Remove(eventId);
            }
        }

        private void HandleAction(Action action, string eventId)
        {
            action?.Invoke();
            if (_eventSubscriptions.ContainsKey(eventId) && !IsRecurringEvent(eventId))
            {
                RemoveEvent(eventId);
            }
        }

        // Check whether the event is recurring (can be refined if necessary)
        private bool IsRecurringEvent(string eventId)
        {
            // Here you can add logic to distinguish between one-time and recurring events.
            // For example, store the event type in a dictionary or use a different approach.
            return true; // Replace with real logic
        }

        public override void Dispose()
        {
            base.Dispose();

            _turnSubs.Dispose();
            _eventSubscriptions.Clear();
        }
    }
}