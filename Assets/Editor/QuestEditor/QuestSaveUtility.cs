using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using SimpleJSON;

namespace QuestEditor
{
    public class QuestSaveUtility
    {
        private List<Edge> Edges => _graphView.edges.ToList();
        private List<BaseNode> Nodes => _graphView.nodes.ToList().Cast<BaseNode>().ToList();

        private List<Group> CommentBlocks =>
            _graphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

        private QuestEditorView _graphView;

        public static QuestSaveUtility GetInstance(QuestEditorView graphView)
        {
            return new QuestSaveUtility
            {
                _graphView = graphView
            };
        }

        public void SaveGraph(string fileName, string objID, int level)
        {
            _graphView.UpdateData();

            JSONNode baseNode = new JSONClass();

            JSONArray array = new JSONArray();
            foreach(var node in QEV.Editor.GetAllNodes())
            {
                if (node.Type == NodeType.Setup)
                    continue;

                array.Add(node.SerializeNode());
            }

            baseNode.Add("Nodes", array);

            JSONArray edges = new JSONArray();
            var connectedSockets = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedSockets.Count(); i++)
            {
                var edgeNode = new JSONClass();
                var outputNode = (connectedSockets[i].output.node as BaseNode);
                var inputNode = (connectedSockets[i].input.node as BaseNode);
                edgeNode.Add("BaseNodeGUID", outputNode.GUID);
                edgeNode.Add("PortName", connectedSockets[i].output.portName);
                edgeNode.Add("TargetNodeGUID", inputNode.GUID);
                edges.Add(edgeNode);
            }

            baseNode.Add("Edges", edges);
            JSONNode dataNode = new JSONClass();
            dataNode.Add("QuestID", fileName);
            dataNode.Add("Level", level.ToString());
            dataNode.Add("ObjectID", objID);
            baseNode.Add("Data", dataNode);

            SetupIconNode setupNode = QEV.Editor.GetAllNodes().FirstOrDefault(nod => nod.Type == NodeType.Setup) as SetupIconNode;
            if (setupNode != null)
            {
                baseNode.Add("SetupNode", setupNode.SerializeNode());
                dataNode["ObjectID"] = baseNode["SetupNode"]["NodeData"]["ID"].Value;
            }

            Debug.LogError(baseNode);

            File.WriteAllText(Application.dataPath + "/Resources/missions/mission" + level + "/EditorQuest/" + fileName + ".json", baseNode.ToString());
        }

        public void LoadNarrative(string fileName, int level)
        {
            QEV.Editor.ClearGraph();

            TextAsset pathString = Resources.Load("missions/mission" + level + "/EditorQuest/" + fileName) as TextAsset;
            JSONNode Data = null;

            if (pathString != null && !pathString.text.Equals(""))
            {
                Data = JSON.Parse(pathString.text);
            }

            if(Data == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Quest Data does not exist!", "OK");
                return;
            }

            QEV.Editor.ObjectID = Data["Data"]["ObjectID"].Value;

            if(Data["SetupNode"]!= null)
                QEV.Editor.LoadNode(Data["SetupNode"]);

            GenerateNodes(Data["Nodes"]);
            QEV.Editor.LoadEdges(Data["Edges"].AsArray);
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void GenerateNodes(JSONNode data)
        {
            JSONArray array = data.AsArray;
            for(int i = 0; i < array.Count; i++)
            {
                QEV.Editor.LoadNode(array[i]);
            }
        }
    }
}