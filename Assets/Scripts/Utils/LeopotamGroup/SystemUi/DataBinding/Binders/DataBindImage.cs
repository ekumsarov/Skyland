// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.DataBinding.Binders {
    /// <summary>
    /// Data binding of sprite for any Image component.
    /// </summary>
    [RequireComponent (typeof (Image))]
    public sealed class DataBindImage : AbstractBinderBase {
        Image _target;

        protected override void ProcessBindedData (object data) {
            if ((object) _target == null) {
                _target = GetComponent<Image> ();
            }
            _target.sprite = data as Sprite;
        }
    }
}