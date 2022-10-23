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
    public class ActionChoiceWorkNode : EventNode
    {
        static int ActionCount = 1;

        private TextField _actionIDView;
        private TextField _actionChoiceIDView;
        private TextField _textIDView;
        private TextField _callDataView;
        private TextField _objectIDView;
        private EnumField _typeView;
        private EnumField _workType;

        public static ActionChoiceWorkNode Create(Vector2 position)
        {
            ActionChoiceWorkNode temp = new ActionChoiceWorkNode();
            temp.GUID = "ActionChoiceWorkNode" + ActionCount;
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

            temp._actionIDView = new TextField("Action ID: ");
            temp._actionIDView.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._actionIDView);

            temp._actionChoiceIDView = new TextField("Choice ID: ");
            temp._actionChoiceIDView.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._actionChoiceIDView);

            temp._callDataView = new TextField("CallData: ");
            temp._callDataView.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._callDataView);

            temp._textIDView = new TextField("Text ID:");
            temp._textIDView.SetValueWithoutNotify(String.Empty);
            temp.contentContainer.Add(temp._textIDView);

            temp._objectIDView = new TextField("On Object ID: ");
            temp._objectIDView.SetValueWithoutNotify("self");
            temp.contentContainer.Add(temp._objectIDView);

            temp._typeView = new EnumField("Call Type:", NodeType.Close);
            temp.contentContainer.Add(temp._typeView);

            temp._workType = new EnumField("Work Type:", EventWorkType.Add);
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();

            return temp;
        }

        public static ActionChoiceWorkNode LoadNode(JSONNode data)
        {
            ActionChoiceWorkNode temp = new ActionChoiceWorkNode();

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

            temp._actionIDView = new TextField("Action ID: ");
            temp._actionIDView.SetValueWithoutNotify(data["NodeData"]["ActionID"].Value);
            temp.contentContainer.Add(temp._actionIDView);

            temp._actionChoiceIDView = new TextField("Choice ID: ");
            temp._actionChoiceIDView.SetValueWithoutNotify(data["NodeData"]["ChoiceID"].Value);
            temp.contentContainer.Add(temp._actionChoiceIDView);

            temp._callDataView = new TextField("CallData: ");
            temp._callDataView.SetValueWithoutNotify(data["NodeData"]["Choice"]["CallData"].Value);
            temp.contentContainer.Add(temp._callDataView);

            temp._textIDView = new TextField("Text ID:");
            temp._textIDView.SetValueWithoutNotify(data["NodeData"]["Choice"]["Text"].Value);
            temp.contentContainer.Add(temp._textIDView);

            temp._objectIDView = new TextField("On Object ID: ");
            temp._objectIDView.SetValueWithoutNotify(data["NodeData"]["To"].Value);
            temp.contentContainer.Add(temp._objectIDView);

            temp._typeView = new EnumField("Call Type:", NodeType.Close);
            if (data["NodeData"]["Choice"]["Type"] != null)
                temp._typeView.SetValueWithoutNotify((NodeType)Enum.Parse(typeof(NodeType), data["NodeData"]["Choice"]["Type"].Value));

            temp.contentContainer.Add(temp._typeView);

            temp._workType = new EnumField("Work Type:", EventWorkType.Add);
            if (data["NodeData"]["WorkType"] != null)
                temp._workType.SetValueWithoutNotify((EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));

            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {

            JSONNode baseNode = this.GetBaseNode("ActionChoiceWorkNode");
            baseNode["NodeData"].Add("ActionID", this._actionIDView.value);
            baseNode["NodeData"].Add("ChoiceID", this._actionChoiceIDView.value);
            baseNode["NodeData"].Add("To", this._objectIDView.value);
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());
            baseNode["NodeData"].Add("Base", "ActChoiceWork");

            JSONNode choiceNode = new JSONClass();
            choiceNode.Add("CallData", this._callDataView.value);

            if (!this._textIDView.value.IsNullOrEmpty())
                choiceNode.Add("Text", this._textIDView.value);

            choiceNode.Add("Type", this._typeView.value.ToString());

            baseNode["NodeData"].Add("Choice", choiceNode);


            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ActionID", this._actionIDView.value);
            baseNode.Add("ChoiceID", this._actionChoiceIDView.value);
            baseNode.Add("To", this._objectIDView.value);
            baseNode.Add("WorkType", this._workType.value.ToString());
            baseNode.Add("Base", "ActChoiceWork");

            JSONNode choiceNode = new JSONClass();
            choiceNode.Add("CallData", this._callDataView.value);

            if (!this._textIDView.value.IsNullOrEmpty())
                choiceNode.Add("Text", this._textIDView.value);

            choiceNode.Add("Type", this._typeView.value.ToString());

            baseNode.Add("Choice", choiceNode);


            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}