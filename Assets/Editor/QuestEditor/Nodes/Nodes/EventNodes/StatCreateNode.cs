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
    public class StatCreateNode : EventNode
    {
        static int ActionCount = 1;

        TextField _statID;
        TextField _iconID;

        FloatField _curValue;
        FloatField _maxValue;

        EnumField _representerType;

        Toggle _isProduct;
        Toggle _isNegative;
        Toggle _isMainStat;

        public static StatCreateNode Create(Vector2 position)
        {
            StatCreateNode temp = new StatCreateNode();
            temp.GUID = "StatCreateNode" + ActionCount;
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

            temp._statID = new TextField("Stat ID: ");
            temp._statID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._statID);

            temp._iconID = new TextField("Icon ID: ");
            temp._iconID.SetValueWithoutNotify("BaseIcon");
            temp.contentContainer.Add(temp._iconID);

            temp._curValue = new FloatField("Start Value");
            temp._curValue.SetValueWithoutNotify(0);
            temp.contentContainer.Add(temp._curValue);

            temp._maxValue = new FloatField("Max Value");
            temp._maxValue.SetValueWithoutNotify(0);
            temp.contentContainer.Add(temp._maxValue);

            temp._representerType = new EnumField("Representer Type:", Represent.Type.Simple);
            temp.contentContainer.Add(temp._representerType);

            temp._isProduct = new Toggle("IsProduct");
            temp._isProduct.SetValueWithoutNotify(true);
            temp.contentContainer.Add(temp._isProduct);

            temp._isNegative = new Toggle("Can be negative:");
            temp._isNegative.SetValueWithoutNotify(false);
            temp.contentContainer.Add(temp._isNegative);

            temp._isMainStat = new Toggle("MainStat:");
            temp._isMainStat.SetValueWithoutNotify(true);
            temp.contentContainer.Add(temp._isMainStat);

            temp.ConditionSetup();

            return temp;
        }

        public static StatCreateNode LoadNode(JSONNode data)
        {
            StatCreateNode temp = new StatCreateNode();

            temp.GUID = data["Node"]["GUID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;

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

            temp._iconID = new TextField("Icon ID: ");
            temp._iconID.SetValueWithoutNotify(data["NodeData"]["IconID"].Value);
            temp.contentContainer.Add(temp._iconID);

            temp._curValue = new FloatField("Start Value");
            temp._curValue.SetValueWithoutNotify(0);
            if (data["NodeData"]["CurValue"] != null)
                temp._curValue.SetValueWithoutNotify(data["NodeData"]["CurValue"].AsFloat);
            temp.contentContainer.Add(temp._curValue);

            temp._maxValue = new FloatField("Start Value");
            temp._maxValue.SetValueWithoutNotify(0);
            if (data["NodeData"]["MaxValue"] != null)
                temp._maxValue.SetValueWithoutNotify(data["NodeData"]["MaxValue"].AsFloat);
            temp.contentContainer.Add(temp._maxValue);

            temp._representerType = new EnumField("Representer Type:", Represent.Type.Simple);
            if (data["NodeData"]["Representer"] != null)
                temp._representerType.SetValueWithoutNotify((Represent.Type)Enum.Parse(typeof(Represent.Type), data["NodeData"]["Representer"].Value));
            temp.contentContainer.Add(temp._representerType);

            temp._isProduct = new Toggle("IsProduct");
            temp._isProduct.SetValueWithoutNotify(true);
            if (data["NodeData"]["IsProduct"] != null)
                temp._isProduct.SetValueWithoutNotify(data["NodeData"]["IsProduct"].AsBool);
            temp.contentContainer.Add(temp._isProduct);

            temp._isNegative = new Toggle("Can be negative:");
            temp._isNegative.SetValueWithoutNotify(true);
            if (data["NodeData"]["Negative"] != null)
                temp._isNegative.SetValueWithoutNotify(data["NodeData"]["Negative"].AsBool);
            temp.contentContainer.Add(temp._isNegative);

            temp._isMainStat = new Toggle("MainStat:");
            temp._isMainStat.SetValueWithoutNotify(true);
            if (data["NodeData"]["MainStat"] != null)
                temp._isMainStat.SetValueWithoutNotify(data["NodeData"]["MainStat"].AsBool);
            temp.contentContainer.Add(temp._isMainStat);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("StatCreateNode");

            baseNode["NodeData"].Add("ID", this._statID.value);
            baseNode["NodeData"].Add("IconID", this._iconID.value);
            baseNode["NodeData"].Add("CurValue", this._curValue.value.ToString());
            baseNode["NodeData"].Add("MaxValue", this._maxValue.value.ToString());
            baseNode["NodeData"].Add("Representer", this._representerType.value.ToString());
            baseNode["NodeData"].Add("IsProduct", this._isProduct.value.ToString());
            baseNode["NodeData"].Add("Negative", this._isNegative.value.ToString());
            baseNode["NodeData"].Add("MainStat", this._isMainStat.value.ToString());
            baseNode["NodeData"].Add("Base", "StatCreate");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._statID.value);
            baseNode.Add("IconID", this._iconID.value);
            baseNode.Add("CurValue", this._curValue.value.ToString());
            baseNode.Add("MaxValue", this._maxValue.value.ToString());
            baseNode.Add("Representer", this._representerType.value.ToString());
            baseNode.Add("IsProduct", this._isProduct.value.ToString());
            baseNode.Add("Negative", this._isNegative.value.ToString());
            baseNode.Add("MainStat", this._isMainStat.value.ToString());
            baseNode.Add("Base", "StatCreate");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}