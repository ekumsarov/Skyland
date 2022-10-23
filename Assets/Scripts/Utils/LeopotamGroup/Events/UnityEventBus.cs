// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;

namespace LeopotamGroup.Events {
    /// <summary>
    /// Event bus service, local for current scene.
    /// </summary>
    public class UnityEventBus : MonoBehaviourService<UnityEventBus> {
        protected EventBus EventBus;

        protected override void OnCreateService () {
            EventBus = new EventBus ();
        }

        protected override void OnDestroyService () {
            UnsubscribeAndClearAllEvents ();
        }

        /// <summary>
        /// Subscribe callback to be raised on specific event.
        /// </summary>
        /// <param name="eventAction">Callback.</param>
        public void Subscribe<T> (EventBus.EventHandler<T> eventAction) {
            EventBus.Subscribe (eventAction);
        }

        /// <summary>
        /// Unsubscribe callback.
        /// </summary>
        /// <param name="eventAction">Event action.</param>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void Unsubscribe<T> (EventBus.EventHandler<T> eventAction, bool keepEvent = false) {
            EventBus.Unsubscribe (eventAction, keepEvent);
        }

        /// <summary>
        /// Unsubscribe all callbacks from event.
        /// </summary>
        /// <param name="keepEvent">GC optimization - clear only callback list and keep event for future use.</param>
        public void UnsubscribeAll<T> (bool keepEvent = false) {
            EventBus.UnsubscribeAll<T> (keepEvent);
        }

        /// <summary>
        /// Unsubscribe all listeneres and clear all events.
        /// </summary>
        public void UnsubscribeAndClearAllEvents () {
            EventBus.UnsubscribeAndClearAllEvents ();
        }

        /// <summary>
        /// Publish event.
        /// </summary>
        /// <param name="eventMessage">Event message.</param>
        public void Publish<T> (T eventMessage) {
            EventBus.Publish (eventMessage);
        }
    }
}