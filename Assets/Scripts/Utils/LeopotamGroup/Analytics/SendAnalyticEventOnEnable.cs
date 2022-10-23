// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Common;
using UnityEngine;

namespace LeopotamGroup.Analytics {
    /// <summary>
    /// Send analytic event on enable.
    /// </summary>
    public sealed class SendAnalyticEventOnEnable : MonoBehaviour {
        [SerializeField]
        string _category = "Category";

        [SerializeField]
        string _event = "Event";

        void OnEnable () {
            if (!string.IsNullOrEmpty (_category) && !string.IsNullOrEmpty (_event)) {
                Service<GoogleAnalyticsManager>.Get ().TrackEvent (_category, _event);
            }
        }
    }
}