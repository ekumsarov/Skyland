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
    public class AddStatNode : EventNode
    {
        static int ActionCount = 1;

        TextField _statID;
        IntegerField _valueField;

        public static AddStatNode Create(Vector2 position)
        {
            AddStatNode temp = new AddStatNode();
            temp.GUID = "AddStatNode" + ActionCount;
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

            temp._statID = new TextField("Stat ID:");
            temp._statID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._statID);

            temp._valueField = new IntegerField("Value:");
            temp._valueField.SetValueWithoutNotify(0);
            temp.contentContainer.Add(temp._valueField);

            temp.ConditionSetup();

            return temp;
        }

        public static AddStatNode LoadNode(JSONNode data)
        {
            AddStatNode temp = new AddStatNode();

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

            temp._statID = new TextField("Stat ID: ");
            temp._statID.SetValueWithoutNotify(data["NodeData"]["ID"].Value);
            temp.contentContainer.Add(temp._statID);

            temp._valueField = new IntegerField("Value:");
            temp._valueField.SetValueWithoutNotify(data["NodeData"]["value"].AsInt);
            temp.contentContainer.Add(temp._valueField);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("AddStatNode");

            baseNode["NodeData"].Add("ID", this._statID.value);
            baseNode["NodeData"].Add("value", this._valueField.value.ToString());
            baseNode["NodeData"].Add("Base", "AddStat");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._statID.value);
            baseNode.Add("value", this._valueField.value.ToString());
            baseNode.Add("Base", "AddStat");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}