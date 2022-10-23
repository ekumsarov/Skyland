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
    public class CallEventNode : EventNode
    {
        static int ActionCount = 1;

        TextField _packID;
        TextField _objectID;
        IntegerField _days;
        EnumField _dayPart;
        IntegerField _ticks;

        public static CallEventNode Create(Vector2 position)
        {
            CallEventNode temp = new CallEventNode();
            temp.GUID = "CallEventNode" + ActionCount;
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

            temp._packID = new TextField("Pack ID: ");
            temp._packID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._packID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._objectID);

            temp._days = new IntegerField("Days delay");
            temp._days.SetValueWithoutNotify(-1);
            temp.contentContainer.Add(temp._days);

            temp._dayPart = new EnumField("Day part", DayPart.Afternoon);
            temp.contentContainer.Add(temp._dayPart);

            temp._ticks = new IntegerField("Ticks delay");
            temp._ticks.SetValueWithoutNotify(-1);
            temp.contentContainer.Add(temp._ticks);

            temp.ConditionSetup();

            return temp;
        }

        public static CallEventNode LoadNode(JSONNode data)
        {
            CallEventNode temp = new CallEventNode();

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

            temp._packID = new TextField("Hero ID: ");
            temp._packID.SetValueWithoutNotify(data["NodeData"]["ID"].Value);
            temp.contentContainer.Add(temp._packID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["To"].Value);
            temp.contentContainer.Add(temp._objectID);

            temp._days = new IntegerField("Days delay");

            if (data["NodeData"]["DayDelay"]["day"] != null)
                temp._days.SetValueWithoutNotify(data["NodeData"]["DayDelay"]["day"].AsInt);
            else
                temp._days.SetValueWithoutNotify(-1);

            temp.contentContainer.Add(temp._days);

            temp._dayPart = new EnumField("Day part", DayPart.Afternoon);
            if (data["NodeData"]["DayDelay"]["part"] != null)
                temp._dayPart.SetValueWithoutNotify((DayPart)Enum.Parse(typeof(DayPart), data["NodeData"]["DayDelay"]["part"].Value));

            temp.contentContainer.Add(temp._dayPart);

            temp._ticks = new IntegerField("Ticks delay");

            if (data["NodeData"]["Ticks"] != null)
                temp._ticks.SetValueWithoutNotify(data["NodeData"]["Ticks"].AsInt);
            else
                temp._ticks.SetValueWithoutNotify(-1);

            temp.contentContainer.Add(temp._ticks);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {

            JSONNode baseNode = this.GetBaseNode("CallEventNode");
            baseNode["NodeData"].Add("ID", this._packID.value);
            baseNode["NodeData"].Add("To", this._objectID.value);
            baseNode["NodeData"].Add("Base", "CallEvent");

            if (this._days.value > 0)
            {
                JSONNode dayDelay = new JSONClass();

                dayDelay.Add("day", this._days.value.ToString());
                dayDelay.Add("part", this._dayPart.value.ToString());

                baseNode["NodeData"].Add("DayDelay", dayDelay);
            }

            if (this._ticks.value > 0)
            {
                baseNode["NodeData"].Add("Ticks", this._ticks.value.ToString());
            }

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._packID.value);
            baseNode.Add("To", this._objectID.value);
            baseNode.Add("Base", "CallPack");

            if (this._days.value > 0)
            {
                JSONNode dayDelay = new JSONClass();

                dayDelay.Add("day", this._days.value.ToString());
                dayDelay.Add("part", this._dayPart.value.ToString());

                baseNode.Add("DayDelay", dayDelay);
            }

            if (this._ticks.value > 0)
            {
                baseNode.Add("Ticks", this._ticks.value.ToString());
            }

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}