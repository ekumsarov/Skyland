// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;

namespace LeopotamGroup.EditorHelpers.UnityEditors {
    /// <summary>
    /// Helper for custom flags selector.
    /// </summary>
    [CustomPropertyDrawer (typeof (EnumFlagsAttribute))]
    sealed class EnumFlagsAttributeInspector : PropertyDrawer {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            property.intValue = EditorGUI.MaskField (position, label, property.intValue, property.enumNames);
        }
    }
}