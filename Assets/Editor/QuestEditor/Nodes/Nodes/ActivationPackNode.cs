using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using SimpleJSON;

namespace QuestEditor
{
    public class ActivationPackNode : BaseNode
    {
        static int ActionCount = 1;

        List<ActivationPackPort> ports;

        public static ActivationPackNode Create(Vector2 position)
        {
            ActivationPackNode temp = new ActivationPackNode();
            temp.GUID = "ActivationPackNode" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.Activation;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            var textField = new TextField("Pack ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(textField);

            var button = new Button(temp.AddPort)
            {
                text = "Add Port"
            };
            temp.titleButtonContainer.Add(button);

            temp.ports = new List<ActivationPackPort>();

            return temp;
        }

        public static ActivationPackNode LoadNode(JSONNode data)
        {
            ActivationPackNode temp = new ActivationPackNode();

            temp.GUID = data["Node"]["GUID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.Activation;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(MyString.JSONToVector2(data["Node"]["Position"]),
                new Vector2(200, 150)));

            var textField = new TextField("Pack ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.mainContainer.Add(textField);

            var button = new Button(temp.AddPort)
            {
                text = "Add Port"
            };
            temp.titleButtonContainer.Add(button);

            temp.ports = new List<ActivationPackPort>();
            if (data["NodeData"]["Ports"] != null)
            {
                JSONArray array = data["NodeData"]["Ports"].AsArray;
                for (int i = 0; i < array.Count; i++)
                {
                    var outputPortName = array[i].Value;
                    temp.ports.Add(new ActivationPackPort(outputPortName, temp));
                }
            }

            temp.RefreshPorts();
            temp.RefreshExpandedState();

            return temp;
        }

        public void AddPort()
        {
            var outputPortName = $"PackPort{this.ports.Count}";
            this.ports.Add(new ActivationPackPort(outputPortName, this));

            this.RefreshPorts();
            this.RefreshExpandedState();
        }

        public void RemovePort(ActivationPackPort port)
        {
            if (this.ports.Contains(port))
                this.ports.Remove(port);

            QEV.Editor.RemovePort(this, port.port);
            //this.outputContainer.Remove(port.port);
        }

        public override void SetupConnectedNode(BaseNode node, Port updatedPort)
        {
            ActivationPackPort port = this.ports.FirstOrDefault(pt => pt.port == updatedPort);
            if (port == null)
            {
                Debug.LogError("Not found port in choices");
                return;
            }


            port.port.portName = this.GUID + node.GUID;
            port.eventNode = node;

            return;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("ActivationPackNode");

            baseNode["NodeData"].Add("ActionID", this.GUID);
            baseNode["NodeData"].Add("Base", "PushPack");



            JSONArray array = new JSONArray();
            if (this.ports.Count > 0)
            {
                JSONArray actchoices = new JSONArray();
                for (int i = 0; i < this.ports.Count; i++)
                {
                    actchoices.Add(this.ports[i].eventNode.GetEvent());
                    array.Add(this.ports[i].port.portName);
                }

                baseNode["NodeData"].Add("Events", actchoices);
            }

            baseNode["NodeData"].Add("Ports", array);

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this.GUID);
            baseNode.Add("To", "self");
            baseNode.Add("Base", "CallPack");

            return baseNode;
        }

        public override Port GetOuputPort(string portID)
        {
            ActivationPackPort port = this.ports.FirstOrDefault<ActivationPackPort>(por => por.port.portName.Equals(portID));
            if (port != null)
                return port.port;

            return null;
        }
    }

    public class ActivationPackPort
    {
        public Port port;
        public BaseNode eventNode;

        public ActivationPackPort(string name, ActivationPackNode parent)
        {
            port = parent.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            port.portName = name;
            var portLabel = port.contentContainer.Q<Label>("type");
            port.contentContainer.Remove(portLabel);

            var deleteButton = new Button(() => parent.RemovePort(this))
            {
                text = "Remove"
            };
            port.contentContainer.Add(deleteButton);

            parent.outputContainer.Add(port);
        }
    }
}