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
    public class FlagWorkNode : EventNode
    {
        static int ActionCount = 1;

        TextField _flagID;
        EnumField _workType;

        public static FlagWorkNode Create(Vector2 position)
        {
            FlagWorkNode temp = new FlagWorkNode();
            temp.GUID = "FlagWorkNode" + ActionCount;
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

            temp._flagID = new TextField("Flag ID: ");
            temp._flagID.SetValueWithoutNotify(temp.title);
            temp._flagID.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            temp.contentContainer.Add(temp._flagID);

            temp._workType = new EnumField("Work Type:", EventWorkType.Add);
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();

            return temp;
        }

        public static FlagWorkNode LoadNode(JSONNode data)
        {
            FlagWorkNode temp = new FlagWorkNode();

            temp.GUID = data["NodeData"]["FlagWorkID"].Value;
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

            temp._flagID = new TextField("Flag ID: ");
            temp._flagID.SetValueWithoutNotify(data["NodeData"]["FlagWorkID"].Value);
            temp._flagID.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            temp.contentContainer.Add(temp._flagID);

            temp._workType = new EnumField("Work Type:", (EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("FlagWork");

            baseNode["NodeData"].Add("FlagWorkID", this._flagID.value);
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());
            baseNode["NodeData"].Add("Base", "FlagWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("FlagWorkID", this._flagID.value);
            baseNode.Add("WorkType", this._workType.value.ToString());
            baseNode.Add("Base", "FlagWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}