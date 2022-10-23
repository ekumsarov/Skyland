using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace QuestEditor
{
    public class NodeSearch : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow _window;
        private QuestEditorView _graphView;

        private Texture2D _indentationIcon;

        public void Configure(EditorWindow window, QuestEditorView graphView)
        {
            _window = window;
            _graphView = graphView;

            //Transparent 1px indentation icon as a hack
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                
                new SearchTreeEntry(new GUIContent("Action Node", _indentationIcon))
                {
                    level = 2, userData = "Context"
                },
                new SearchTreeEntry(new GUIContent("SkillCheck Node", _indentationIcon))
                {
                    level = 2, userData = "SkillCheck"
                },
                new SearchTreeEntry(new GUIContent("Pack Node", _indentationIcon))
                {
                    level = 1, userData = "PackNode"
                },
                new SearchTreeGroupEntry(new GUIContent("Event Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Flag Node", _indentationIcon))
                {
                    level = 2, userData = "FlagWork"
                },
                new SearchTreeEntry(new GUIContent("Action Choice", _indentationIcon))
                {
                    level = 2, userData = "ActionChoiceWorkNode"
                },
                new SearchTreeEntry(new GUIContent("Activate Stat", _indentationIcon))
                {
                    level = 2, userData = "ActivateStatNode"
                },
                new SearchTreeEntry(new GUIContent("AddStat", _indentationIcon))
                {
                    level = 2, userData = "AddStatNode"
                },
                new SearchTreeEntry(new GUIContent("AddStatProduction", _indentationIcon))
                {
                    level = 2, userData = "AddStatProductNode"
                },
                new SearchTreeEntry(new GUIContent("Add Event", _indentationIcon))
                {
                    level = 2, userData = "AddEventNode"
                },
                new SearchTreeEntry(new GUIContent("Call Action Node", _indentationIcon))
                {
                    level = 2, userData = "CallActionNode"
                },
                new SearchTreeEntry(new GUIContent("Create Icon Node", _indentationIcon))
                {
                    level = 2, userData = "CreateIcon"
                },
                new SearchTreeEntry(new GUIContent("Call Event Function", _indentationIcon))
                {
                    level = 2, userData = "CallEventFunctionNode"
                },
                new SearchTreeEntry(new GUIContent("SetupMainEvent", _indentationIcon))
                {
                    level = 2, userData = "SetupMainEventNode"
                },
                new SearchTreeEntry(new GUIContent("MapIconAdditionalEventNode", _indentationIcon))
                {
                    level = 2, userData = "MapIconAdditionalEventNode"
                },
                new SearchTreeEntry(new GUIContent("MoveIconNode", _indentationIcon))
                {
                    level = 2, userData = "MoveIconNode"
                },
                new SearchTreeEntry(new GUIContent("ActionWorkNode", _indentationIcon))
                {
                    level = 2, userData = "ActionWorkNode"
                },
                new SearchTreeEntry(new GUIContent("LockObjectNode", _indentationIcon))
                {
                    level = 2, userData = "LockObjectNode"
                },
                new SearchTreeEntry(new GUIContent("ActivateSubLocationNode", _indentationIcon))
                {
                    level = 2, userData = "ActivateSubLocationNode"
                },
                new SearchTreeEntry(new GUIContent("ActivateObject", _indentationIcon))
                {
                    level = 2, userData = "ActivateObjectNode"
                },
                new SearchTreeEntry(new GUIContent("Main Quest Node", _indentationIcon))
                {
                    level = 2, userData = "MainQuestWork"
                },
                new SearchTreeEntry(new GUIContent("Quest Node", _indentationIcon))
                {
                    level = 2, userData = "QuestWork"
                },
                new SearchTreeEntry(new GUIContent("Tooltip Node", _indentationIcon))
                {
                    level = 2, userData = "ShowTooltip"
                },
                new SearchTreeEntry(new GUIContent("Call Pack", _indentationIcon))
                {
                    level = 2, userData = "CallPackNode"
                },
                new SearchTreeEntry(new GUIContent("ShowReward Node", _indentationIcon))
                {
                    level = 2, userData = "ShowReward"
                },
                new SearchTreeEntry(new GUIContent("Stat Create", _indentationIcon))
                {
                    level = 2, userData = "StatCreateNode"
                },
                new SearchTreeEntry(new GUIContent("LootWorkNode", _indentationIcon))
                {
                    level = 2, userData = "LootWorkNode"
                },
                new SearchTreeEntry(new GUIContent("AddHero", _indentationIcon))
                {
                    level = 2, userData = "AddHero"
                },
                new SearchTreeEntry(new GUIContent("BattleNode", _indentationIcon))
                {
                    level = 2, userData = "BattleNode"
                },
                new SearchTreeEntry(new GUIContent("Setup Node", _indentationIcon))
                {
                    level = 1, userData = "SetupIconNode"
                },
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            //Editor window-based mouse position
            var mousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
                context.screenMousePosition - _window.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);
            QEV.Editor.CreateNode(SearchTreeEntry.userData as string, graphMousePosition);
            /*switch (SearchTreeEntry.userData)
            {
                case "fdsf":
                    QEV.Editor.CreateActionNode(graphMousePosition);
                    return true;
            }*/
            return true;
        }
    }
}