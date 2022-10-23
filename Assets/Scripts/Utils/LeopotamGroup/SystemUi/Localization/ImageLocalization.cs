// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Localization;
using LeopotamGroup.SystemUi.Atlases;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Localization {
    /// <summary>
    /// Localization helper for System UI Sprite.
    /// </summary>
    [RequireComponent (typeof (Image))]
    public sealed class ImageLocalization : MonoBehaviour {
        [SerializeField]
        string _token = null;

        [SerializeField]
        SpriteAtlas _atlas = null;

        Image _image;

        void OnEnable () {
            OnLocalize ();
        }

        [Preserve]
        void OnLocalize () {
            if (!string.IsNullOrEmpty (_token) && (object) _atlas != null) {
                if ((object) _image == null) {
                    _image = GetComponent<Image> ();
                }
                _image.sprite = _atlas.Get (Localizer.Get (_token));
            }
        }
    }
}