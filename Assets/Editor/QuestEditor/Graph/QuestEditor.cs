using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using SimpleJSON;

namespace QuestEditor
{
    public class QuestEditor : EditorWindow
    {
        private string _fileName = "New Quest";
        private int _missionLevel = 1;
        private string _objectID = "Manager";

        private QuestEditorView _graphView;
        private List<BaseNode> _allNodes => this._graphView.nodes.ToList().Cast<BaseNode>().ToList();

        private bool HasIconNode = false;

        [MenuItem("QuestEditor/Open Editor")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<QuestEditor>();
            window.titleContent = new GUIContent("Quest Editor");
            QEV.Editor = window;
            QEV.Editor.HasIconNode = false;
        }

        private TextField _objectIDField;

        private void OnEnable()
        {
            this.HasIconNode = false;

            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }

        private void ConstructGraphView()
        {
            _graphView = new QuestEditorView(this)
            {
                name = "Quest Editor",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("Quest ID:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

            var missionIntField = new IntegerField("Level:");
            missionIntField.SetValueWithoutNotify(_missionLevel);
            missionIntField.MarkDirtyRepaint();
            missionIntField.RegisterValueChangedCallback(evt => _missionLevel = evt.newValue);

            _objectIDField = new TextField("Object ID:");
            _objectIDField.SetValueWithoutNotify(_objectID);
            _objectIDField.MarkDirtyRepaint();
            _objectIDField.RegisterValueChangedCallback(evt => _objectID = evt.newValue);

            toolbar.Add(fileNameTextField);
            toolbar.Add(missionIntField);
            toolbar.Add(_objectIDField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });

            toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });
            toolbar.Add(new Button(() => _graphView.UpdateData()) { text = "Update Data" });
            // toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) {text = "New Node",});
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var saveUtility = QuestSaveUtility.GetInstance(_graphView);
                if (save)
                    saveUtility.SaveGraph(_fileName, _objectID, _missionLevel);
                else
                    saveUtility.LoadNarrative(_fileName, _missionLevel);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        public void RemovePort(Node node, Port port)
        {
            var targetEdge = this._graphView.edges.ToList()
                .Where(x => x.output.portName == port.portName && x.output.node == port.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                this._graphView.RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(port);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        public void ConnectPorts(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _graphView.Add(tempEdge);
        }

        public void RemoveElement(GraphElement item)
        {
            this._graphView.RemoveElement(item);
        }

        public void RemoveEdge(Edge edge)
        {
            this._graphView.RemoveElement(edge);
        }

        public void CreateNode(string type, Vector2 mousePosition)
        {
            if(type.Equals("SetupIconNode"))
            {
                if (this.HasIconNode)
                    return;

                SetupIconNode node = SetupIconNode.Create(mousePosition);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                this.HasIconNode = true;
            }
            else if(type.Equals("Context"))
            {
                ActionNode node = ActionNode.Create(mousePosition);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
            }
            else if(type.Equals("SkillCheck"))
            {
                SkillCheckNode node = SkillCheckNode.Create(mousePosition);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
            }
            else if (type.Equals("PackNode"))
            {
                PackNode node = PackNode.Create(mousePosition);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
            }
            else
            {
                BaseNode node = EventNode.GetEvent(type, mousePosition);
                if (node == null)
                    return;

                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
            }
        }

        public void LoadNode(SimpleJSON.JSONNode data)
        {
            if(data["Node"]["Type"].Value.Equals("SetupIconNode"))
            {
                SetupIconNode node = SetupIconNode.LoadNode(data);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                this.HasIconNode = true;
            }
            else if(data["Node"]["Type"].Value.Equals("Context"))
            {
                ActionNode node = ActionNode.LoadNode(data);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
            else if(data["Node"]["Type"].Value.Equals("SkillCheck"))
            {
                SkillCheckNode node = SkillCheckNode.LoadNode(data);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
            else if (data["Node"]["Type"].Value.Equals("PackNode"))
            {
                PackNode node = PackNode.LoadNode(data);
                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                node.RefreshPorts();
                node.RefreshExpandedState();
            }
            else
            {
                BaseNode node = EventNode.LoadEvent(data);
                if (node == null)
                    return;

                //this._allNodes.Add(node);
                this._graphView.AddElement(node);
                node.RefreshPorts();
                node.RefreshExpandedState();

            }
        }

        public List<BaseNode> GetAllNodes()
        {
            return _allNodes;
        }

        public void LoadEdges(JSONArray data)
        {
            for(int i = 0; i < data.Count; i++)
            {
                BaseNode baseNode = this._allNodes.FirstOrDefault<BaseNode>(nod => nod.GUID.Equals(data[i]["BaseNodeGUID"]));
                BaseNode targetNoe = this._allNodes.FirstOrDefault<BaseNode>(nod => nod.GUID.Equals(data[i]["TargetNodeGUID"]));

                if(baseNode == null)
                {
                    Debug.LogError("Not found nodes");
                    Debug.LogError(data[i]["BaseNodeGUID"].Value);
                    continue;
                }

                if (targetNoe == null)
                {
                    Debug.LogError("Not found nodes");
                    Debug.LogError(data[i]["TargetNodeGUID"].Value);
                    continue;
                }

                Port output = baseNode.GetOuputPort(data[i]["PortName"]);
                Port input = (Port)targetNoe.inputContainer[0];

                if(output == null || input == null)
                {
                    Debug.LogError("Not found ports");
                    continue;
                }

                this.ConnectPorts(output, input);
            }
        }

        public void ClearGraph()
        {
            foreach(var node in this._allNodes)
            {
                this._graphView.RemoveElement(node);
            }

            foreach (var node in this._graphView.edges.ToList())
            {
                this._graphView.RemoveElement(node);
            }
        }

        public string ObjectID
        {
            set
            {
                this._objectIDField.SetValueWithoutNotify(value);
                this._objectID = value;
            }
        }

        public int Level
        {
            get { return this._missionLevel; }
        }
    }

    public static class QEV // QuestEditor Access
    {
        static QuestEditor instance = null;
        public static QuestEditor Editor
        {
            get
            {
                if (QEV.instance == null)
                    QuestEditor.CreateGraphViewWindow();

                return QEV.instance;
            }
            set { QEV.instance = value; }
        }
    }

    public enum SkillType
    {
        strenght,
        dexterity,
        stamina,
        intelligence,
        charisma
    }

    public enum NodeType
    {
        Context,
        SkillCheck,
        Action,
        Pack,
        Event,
        Close,
        Setup,
        Activation
    }

    public enum ConditionType
    {
        NoType,
        Stat,
        Flag,
        Quest,
        Loot,
        Daypart
    }

    public enum EventWorkType
    {
        Add,
        Remove,
        Replace,
        On,
        Off,
        ChangeText
    }
}