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
    public class ActionNode : BaseNode
    {
        static int ActionCount = 1;

        List<ActionChoicePort> choices;
        Toggle _showText;

        public static ActionNode Create(Vector2 position)
        {
            ActionNode temp = new ActionNode();
            temp.GUID = "Action" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.Context;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            var textField = new TextField("Action ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(textField);

            temp._showText = new Toggle("Show Text");
            temp._showText.SetValueWithoutNotify(true);
            temp.contentContainer.Add(temp._showText);

            var button = new Button(temp.AddChoicePort)
            {
                text = "Add Choice"
            };
            temp.titleButtonContainer.Add(button);

            temp.choices = new List<ActionChoicePort>();

            return temp;
        }

        public static ActionNode LoadNode(JSONNode data)
        {
            ActionNode temp = new ActionNode();

            temp.GUID = data["NodeData"]["ActionID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.Context;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(MyString.JSONToVector2(data["Node"]["Position"]),
                new Vector2(200, 150)));

            var textField = new TextField("Action ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.mainContainer.Add(textField);

            temp._showText = new Toggle("Show Text");

            if (data["NodeData"]["ShowText"] != null)
                temp._showText.SetValueWithoutNotify(data["NodeData"]["ShowText"].AsBool);
            else
                temp._showText.SetValueWithoutNotify(true);

            temp.contentContainer.Add(temp._showText);

            var button = new Button(temp.AddChoicePort)
            {
                text = "Add Choice"
            };
            temp.titleButtonContainer.Add(button);

            temp.choices = new List<ActionChoicePort>();
            if(data["NodeData"]["Actions"] != null)
            {
                JSONArray array = data["NodeData"]["Actions"].AsArray;
                for(int i = 0; i < array.Count; i++)
                {
                    temp.choices.Add(ActionChoicePort.LoadPort(temp, array[i]));
                }
            }
            
            return temp;
        }

        void AddChoicePort()
        {
            var outputPortName = $"ActionChoice{this.choices.Count}";
            this.choices.Add(ActionChoicePort.Create(this, outputPortName));

            this.RefreshPorts();
            this.RefreshExpandedState();
        }

        public void RemovePort(ActionChoicePort port)
        {
            if (this.choices.Contains(port))
                this.choices.Remove(port);
        }

        public override void SetupConnectedNode(BaseNode node, Port updatedPort)
        {
            ActionChoicePort port = this.choices.FirstOrDefault(pt => pt.EqualsPort(updatedPort));
            if (port == null)
            {
                Debug.LogError("Not found port in choices");
                return;
            }


            port.SetCallData(node.GUID);
            port.SetChoiceType(node.Type);

            return;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("Context");

            baseNode["NodeData"].Add("ActionID", this.GUID);
            baseNode["NodeData"].Add("ActionType", "Context");
            baseNode["NodeData"].Add("WorkType", "Add");
            baseNode["NodeData"].Add("Base", "ActionWork");
            baseNode["NodeData"].Add("ShowText", this._showText.value.ToString());

            if (this._showText.value)
                baseNode["NodeData"].Add("Text", this.GUID);

            if (this.choices.Count > 0)
            {
                JSONArray actchoices = new JSONArray();
                for (int i = 0; i < this.choices.Count; i++)
                {
                    actchoices.Add(this.choices[i].GetNode());
                }

                baseNode["NodeData"].Add("Actions", actchoices);
            }

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this.GUID);
            baseNode.Add("Base", "CallAction");

            return baseNode;
        }

        public override Port GetOuputPort(string portID)
        {
            ActionChoicePort port = this.choices.FirstOrDefault<ActionChoicePort>(por => por.GUID.Equals(portID));
            if (port != null)
                return port._port;

            return null;
        }
    }
}