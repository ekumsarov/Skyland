// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;

namespace LeopotamGroup.Pooling {
    /// <summary>
    /// Recycle instance after time (if it was spawned from pool).
    /// Dont use it on swarm spawns of prefab (use custom recycling instead).
    /// </summary>
    public sealed class RecycleAfterTime : MonoBehaviour {
        [SerializeField]
        float _timeout = 1f;

        float _endTime;

        void OnEnable () {
            _endTime = Time.time + _timeout;
        }

        void LateUpdate () {
            if (Time.time >= _endTime) {
                OnRecycle ();
            }
        }

        void OnRecycle () {
            var po = GetComponent<IPoolObject> ();
            if ((object) po != null) {
                po.PoolRecycle ();
            } else {
                gameObject.SetActive (false);
            }
        }
    }
}