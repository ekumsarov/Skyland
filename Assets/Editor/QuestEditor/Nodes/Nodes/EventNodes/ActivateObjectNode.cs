using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SimpleJSON;

namespace QuestEditor
{
    public class ActivateObjectNode : EventNode
    {
        static int ActionCount = 1;

        TextField _objectID;
        Toggle _activate;

        public static ActivateObjectNode Create(Vector2 position)
        {
            ActivateObjectNode temp = new ActivateObjectNode();
            temp.GUID = "ActivateObjectNode" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._objectID);

            temp._activate = new Toggle();
            temp.contentContainer.Add(temp._activate);

            temp.ConditionSetup();

            return temp;
        }

        public static ActivateObjectNode LoadNode(JSONNode data)
        {
            ActivateObjectNode temp = new ActivateObjectNode();

            temp.GUID = data["Node"]["GUID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(MyString.JSONToVector2(data["Node"]["Position"]),
                new Vector2(200, 150)));

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["ID"].Value);
            temp.contentContainer.Add(temp._objectID);

            temp._activate = new Toggle();
            temp._activate.SetValueWithoutNotify(data["NodeData"]["Activate"].AsBool);
            temp.contentContainer.Add(temp._activate);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("ActivateObjectNode");

            baseNode["NodeData"].Add("ID", this._objectID.value);
            baseNode["NodeData"].Add("Activate", this._activate.value.ToString());
            baseNode["NodeData"].Add("Base", "ActivateObject");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._objectID.value);
            baseNode.Add("Activate", this._activate.value.ToString());
            baseNode.Add("Base", "ActivateObject");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}