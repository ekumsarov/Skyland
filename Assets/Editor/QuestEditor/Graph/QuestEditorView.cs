using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace QuestEditor
{
    public class QuestEditorView : GraphView
    {
        private NodeSearch _searchWindow;

        public QuestEditorView(QuestEditor editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("NarrativeGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow(editorWindow);
        }

        private void AddSearchWindow(QuestEditor editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearch>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        public void RemovePort(Node node, Port port)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == port.portName && x.output.node == port.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(port);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        public void UpdateData()
        {
            foreach (var edge in this.edges.ToList().Where(edg => edg.input != null && edg.output != null))
            {
                BaseNode pack = edge.output.node as BaseNode;
                BaseNode inPack = edge.input.node as BaseNode;
                if (pack == null || inPack == null)
                    Debug.LogError("Shit");
                else
                {
                    pack.SetupConnectedNode(inPack, edge.output);
                }
            }

            /*foreach (var port in this.ports.ToList().Where(prt => prt.direction == Direction.Output))
            {
                if(port != null && !port.connected)
                {
                    ActionNode node = port.node as ActionNode;
                    if(node != null)
                    {
                        node.SetupConnectedNode(port);
                    }
                }
            }*/
        }
    }
}