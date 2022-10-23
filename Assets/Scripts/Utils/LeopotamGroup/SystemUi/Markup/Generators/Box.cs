// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Serialization;
using LeopotamGroup.SystemUi.Widgets;
using UnityEngine;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class BoxNode {
        /// <summary>
        /// Create "box" node. If children supported - GameObject container for them should be returned.
        /// </summary>
        /// <param name="widget">Ui widget.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static RectTransform Create (RectTransform widget, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            widget.name = "box";
#endif
            MarkupUtils.SetSize (widget, node);
            MarkupUtils.SetRotation (widget, node);
            MarkupUtils.SetOffset (widget, node);
            MarkupUtils.SetHidden (widget, node);

            if (MarkupUtils.ValidateInteractive (widget, node, container.DragTreshold)) {
                widget.gameObject.AddComponent<NonVisualWidget> ();
            }

            return widget;
        }
    }
}