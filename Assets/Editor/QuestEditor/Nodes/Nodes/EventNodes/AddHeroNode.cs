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
    public class AddHeroNode : EventNode
    {
        static int ActionCount = 1;

        TextField _heroID;
        TextField _objectID;
        EnumField _workType;

        public static AddHeroNode Create(Vector2 position)
        {
            AddHeroNode temp = new AddHeroNode();
            temp.GUID = "AddHeroNode" + ActionCount;
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

            temp._heroID = new TextField("Hero ID: ");
            temp._heroID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._heroID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._objectID);

            temp._workType = new EnumField("Work Type:", EventWorkType.Add);
            temp.contentContainer.Add(temp._workType);

            temp.ConditionSetup();

            return temp;
        }

        public static AddHeroNode LoadNode(JSONNode data)
        {
            AddHeroNode temp = new AddHeroNode();

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

            temp._heroID = new TextField("Hero ID: ");
            temp._heroID.SetValueWithoutNotify(data["NodeData"]["HeroID"].Value);
            temp.contentContainer.Add(temp._heroID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["To"].Value);
            temp.contentContainer.Add(temp._objectID);

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
            
            JSONNode baseNode = this.GetBaseNode("AddHeroNode");
            baseNode["NodeData"].Add("HeroID", this._heroID.value);
            baseNode["NodeData"].Add("To", this._objectID.value);
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());
            baseNode["NodeData"].Add("Base", "AddHero");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("HeroID", this._heroID.value);
            baseNode.Add("To", this._objectID.value);
            baseNode.Add("WorkType", this._workType.value.ToString());
            baseNode.Add("Base", "AddHero");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}