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
    public class LootWorkNode : EventNode
    {
        static int ActionCount = 1;

        TextField _lootID;
        TextField _objectID;
        EnumField _workType;
        EnumField _lootType;

        public static LootWorkNode Create(Vector2 position)
        {
            LootWorkNode temp = new LootWorkNode();
            temp.GUID = "LootWorkNode" + ActionCount;
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

            temp._lootID = new TextField("Loot ID: ");
            temp._lootID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._lootID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify("self");
            temp.contentContainer.Add(temp._objectID);

            temp._workType = new EnumField("Work Type:", EventWorkType.Add);
            temp.contentContainer.Add(temp._workType);

            temp._lootType = new EnumField("loot Type:", LS.LootType.Extra);
            temp.contentContainer.Add(temp._lootType);

            temp.ConditionSetup();

            return temp;
        }

        public static LootWorkNode LoadNode(JSONNode data)
        {
            LootWorkNode temp = new LootWorkNode();

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

            temp._lootID = new TextField("Loot ID: ");
            temp._lootID.SetValueWithoutNotify(data["NodeData"]["LootID"].Value);
            temp.contentContainer.Add(temp._lootID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["To"].Value);
            temp.contentContainer.Add(temp._objectID);

            temp._workType = new EnumField("Work Type:", (EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));
            temp.contentContainer.Add(temp._workType);

            temp._lootType = new EnumField("Loot Type:", (LS.LootType)Enum.Parse(typeof(LS.LootType), data["NodeData"]["type"].Value));
            temp.contentContainer.Add(temp._lootType);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("LootWorkNode");

            baseNode["NodeData"].Add("LootID", this._lootID.value);
            baseNode["NodeData"].Add("To", this._objectID.value);
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());
            baseNode["NodeData"].Add("type", this._lootType.value.ToString());
            baseNode["NodeData"].Add("Base", "LootWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("LootID", this._lootID.value);
            baseNode.Add("To", this._objectID.value);
            baseNode.Add("WorkType", this._workType.value.ToString());
            baseNode.Add("type", this._lootType.value.ToString());
            baseNode.Add("Base", "LootWork");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}