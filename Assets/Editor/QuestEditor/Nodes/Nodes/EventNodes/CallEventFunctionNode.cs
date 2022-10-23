using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SimpleJSON;
using Lodkod;

namespace QuestEditor
{
    public class CallEventFunctionNode : EventNode
    {
        static int ActionCount = 1;

        TextField _eventID;
        TextField _objectID;
        TextField _functionID;

        public static CallEventFunctionNode Create(Vector2 position)
        {
            CallEventFunctionNode temp = new CallEventFunctionNode();
            temp.GUID = "CallEventFunctionNode" + ActionCount;
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

            temp._eventID = new TextField("Event ID:");
            temp._eventID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._eventID);

            temp._objectID = new TextField("Object ID:");
            temp._objectID.SetValueWithoutNotify("self");
            temp.contentContainer.Add(temp._objectID);

            temp._functionID = new TextField("Function:");
            temp._functionID.SetValueWithoutNotify("Play");
            temp.contentContainer.Add(temp._functionID);

            temp.ConditionSetup();

            return temp;
        }

        public static CallEventFunctionNode LoadNode(JSONNode data)
        {
            CallEventFunctionNode temp = new CallEventFunctionNode();

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

            temp._eventID = new TextField("Event ID: ");
            temp._eventID.SetValueWithoutNotify(data["NodeData"]["ID"]["Event"].Value);
            temp.contentContainer.Add(temp._eventID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["To"].Value);
            temp.contentContainer.Add(temp._objectID);

            temp._functionID = new TextField("Function:");
            temp._functionID.SetValueWithoutNotify(data["NodeData"]["ID"]["Action"].Value);
            temp.contentContainer.Add(temp._functionID);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {

            JSONNode baseNode = this.GetBaseNode("CallEventFunctionNode");
            
            baseNode["NodeData"].Add("To", this._objectID.value);
            baseNode["NodeData"].Add("Base", "CallEvent");

            JSONNode eventNode = new JSONClass();

            eventNode.Add("Event", this._eventID.value);
            eventNode.Add("Action", this._functionID.value);

            baseNode["NodeData"].Add("ID", eventNode);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("To", this._objectID.value);
            baseNode.Add("Base", "CallEvent");

            JSONNode eventNode = new JSONClass();

            eventNode.Add("Event", this._eventID.value);
            eventNode.Add("Action", this._functionID.value);

            baseNode.Add("ID", eventNode);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}