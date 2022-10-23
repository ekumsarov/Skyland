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
    public class SetupMainEventNode : EventNode
    {
        static int ActionCount = 1;

        TextField _objectID;
        TextField _eventID;

        public static SetupMainEventNode Create(Vector2 position)
        {
            SetupMainEventNode temp = new SetupMainEventNode();
            temp.GUID = "SetupMainEventNode" + ActionCount;
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

            temp._eventID = new TextField("Event ID: ");
            temp._eventID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._eventID);

            temp.ConditionSetup();

            return temp;
        }

        public static SetupMainEventNode LoadNode(JSONNode data)
        {
            SetupMainEventNode temp = new SetupMainEventNode();

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

            temp._eventID = new TextField("Event ID: ");
            temp._eventID.SetValueWithoutNotify(data["NodeData"]["MainEvent"].Value);
            temp.contentContainer.Add(temp._eventID);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("SetupMainEventNode");

            baseNode["NodeData"].Add("ID", this._objectID.value);
            baseNode["NodeData"].Add("MainEvent", this._eventID.value);
            baseNode["NodeData"].Add("Base", "SetupMainEvent");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._objectID.value);
            baseNode.Add("MainEvent", this._eventID.value);
            baseNode.Add("Base", "SetupMainEvent");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}