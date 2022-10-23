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
    public class ActionWorkNode : EventNode
    {
        static int ActionCount = 1;

        TextField _actionID;
        TextField _textID;
        EnumField _workType;

        public static ActionWorkNode Create(Vector2 position)
        {
            ActionWorkNode temp = new ActionWorkNode();
            temp.GUID = "ActionWorkNode" + ActionCount;
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

            temp._actionID = new TextField("Action ID: ");
            temp._actionID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._actionID);

            temp._textID = new TextField("Text ID: ");
            temp._textID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._textID);

            temp._workType = new EnumField("Work Type:", EventWorkType.ChangeText);
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();

            return temp;
        }

        public static ActionWorkNode LoadNode(JSONNode data)
        {
            ActionWorkNode temp = new ActionWorkNode();

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

            temp._actionID = new TextField("Action ID: ");
            temp._actionID.SetValueWithoutNotify(data["NodeData"]["ActionID"].Value);
            temp.contentContainer.Add(temp._actionID);

            temp._textID = new TextField("Text ID: ");
            temp._textID.SetValueWithoutNotify(data["NodeData"]["Text"].Value);
            temp.contentContainer.Add(temp._textID);

            temp._workType = new EnumField("Work Type:", (EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("ActionWorkNode");

            baseNode["NodeData"].Add("ActionID", this._actionID.value);
            baseNode["NodeData"].Add("Text", this._textID.value);
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());
            baseNode["NodeData"].Add("Base", "ActionWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ActionID", this._actionID.value);
            baseNode.Add("Text", this._textID.value);
            baseNode.Add("WorkType", this._workType.value.ToString());
            baseNode.Add("Base", "ActionWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}