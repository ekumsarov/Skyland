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
    public class ShowTooltipNode : EventNode
    {
        static int ActionCount = 1;

        TextField _tooltipID;

        EnumField _fitField;
        EnumField _timeModeField;
        EnumField _fillModeField;
        EnumField _objectModeField;

        FloatField _existTimeField;
        IntegerField _lineLengt;

        TextField _targetObject;
        Vector2Field _pointField;

        public static ShowTooltipNode Create(Vector2 position)
        {
            ShowTooltipNode temp = new ShowTooltipNode();
            temp.GUID = "ShowTooltip" + ActionCount;
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

            temp._tooltipID = new TextField("Tooltip ID: ");
            temp._tooltipID.SetValueWithoutNotify(temp.title);
            temp._tooltipID.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            temp.contentContainer.Add(temp._tooltipID);

            temp._fitField = new EnumField("Fit Type:", TooltipFit.Auto);
            temp.contentContainer.Add(temp._fitField);

            temp._timeModeField = new EnumField("Time Mode:", TooltipTimeMode.Click);
            temp.contentContainer.Add(temp._timeModeField);

            temp._fillModeField = new EnumField("Fill Mode:", TooltipFillMode.Instantly);
            temp.contentContainer.Add(temp._fillModeField);

            temp._objectModeField = new EnumField("Object Mode:", TooltipObject.Game);
            temp.contentContainer.Add(temp._objectModeField);

            temp._existTimeField = new FloatField("Exist Time");
            temp.contentContainer.Add(temp._existTimeField);

            temp._lineLengt = new IntegerField("Line Lenght");
            temp.contentContainer.Add(temp._lineLengt);

            temp._targetObject = new TextField("On Object");
            temp.contentContainer.Add(temp._targetObject);

            temp._pointField = new Vector2Field("On Point");
            temp.contentContainer.Add(temp._pointField);

            temp.ConditionSetup();

            return temp;
        }

        public static ShowTooltipNode LoadNode(JSONNode data)
        {
            ShowTooltipNode temp = new ShowTooltipNode();

            temp.GUID = data["NodeData"]["text"].Value;
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
            
            temp._tooltipID = new TextField("Tooltip ID: ");
            temp._tooltipID.SetValueWithoutNotify(data["NodeData"]["text"].Value);
            temp._tooltipID.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            temp.contentContainer.Add(temp._tooltipID);

            temp._fitField = new EnumField("Fit Type:", TooltipFit.Auto);
            temp._fitField.SetValueWithoutNotify((TooltipFit)Enum.Parse(typeof(TooltipFit), data["NodeData"]["Fit"].Value));
            temp.contentContainer.Add(temp._fitField);

            temp._timeModeField = new EnumField("Time Mode:", TooltipTimeMode.Click);
            temp._timeModeField.SetValueWithoutNotify((TooltipTimeMode)Enum.Parse(typeof(TooltipTimeMode), data["NodeData"]["TimeMode"].Value));
            temp.contentContainer.Add(temp._timeModeField);

            temp._fillModeField = new EnumField("Fill Mode:", TooltipFillMode.Instantly);
            temp._fillModeField.SetValueWithoutNotify((TooltipFillMode)Enum.Parse(typeof(TooltipFillMode), data["NodeData"]["FillMode"].Value));
            temp.contentContainer.Add(temp._fillModeField);

            temp._objectModeField = new EnumField("Object Mode:", TooltipObject.Game);
            temp._objectModeField.SetValueWithoutNotify((TooltipObject)Enum.Parse(typeof(TooltipObject), data["NodeData"]["ObjectMode"].Value));
            temp.contentContainer.Add(temp._objectModeField);

            temp._existTimeField = new FloatField("Exist Time");
            temp._existTimeField.SetValueWithoutNotify(data["NodeData"]["exTime"].AsFloat);
            temp.contentContainer.Add(temp._existTimeField);

            temp._lineLengt = new IntegerField("Line Lenght");
            temp._lineLengt.SetValueWithoutNotify(data["NodeData"]["LenghtSize"].AsInt);
            temp.contentContainer.Add(temp._lineLengt);

            temp._targetObject = new TextField("On Object");
            temp.contentContainer.Add(temp._targetObject);
            if (data["NodeData"]["ID"] != null)
                temp._targetObject.SetValueWithoutNotify(data["NodeData"]["ID"].Value);

            temp._pointField = new Vector2Field("On Point");
            temp.contentContainer.Add(temp._pointField);
            if (data["NodeData"]["point"] != null)
                temp._pointField.SetValueWithoutNotify(MyString.JSONToVector2(data["NodeData"]["point"]));

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("ShowTooltip");

            baseNode["NodeData"].Add("text", this._tooltipID.value);
            baseNode["NodeData"].Add("Base", "ShowTooltip");

            baseNode["NodeData"].Add("Fit", this._fitField.value.ToString());
            baseNode["NodeData"].Add("TimeMode", this._timeModeField.value.ToString());
            baseNode["NodeData"].Add("FillMode", this._fillModeField.value.ToString());
            baseNode["NodeData"].Add("ObjectMode", this._objectModeField.value.ToString());

            baseNode["NodeData"].Add("exTime", this._existTimeField.value.ToString());
            baseNode["NodeData"].Add("LenghtSize", this._lineLengt.value.ToString());

            if(!this._targetObject.value.IsNullOrEmpty())
                baseNode["NodeData"].Add("ID", this._targetObject.value);

            baseNode["NodeData"].Add("point", MyString.Vector2ToString(this._pointField.value));

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("text", this._tooltipID.value);
            baseNode.Add("Base", "ShowTooltip");

            baseNode.Add("Fit", this._fitField.value.ToString());
            baseNode.Add("TimeMode", this._timeModeField.value.ToString());
            baseNode.Add("FillMode", this._fillModeField.value.ToString());
            baseNode.Add("ObjectMode", this._objectModeField.value.ToString());

            baseNode.Add("exTime", this._existTimeField.value.ToString());
            baseNode.Add("LenghtSize", this._lineLengt.value.ToString());

            if (!this._targetObject.value.IsNullOrEmpty())
                baseNode.Add("ID", this._targetObject.value);

            baseNode.Add("point", MyString.Vector2ToString(this._pointField.value));
            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}