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
    public class CreateIconNode : EventNode
    {
        static int ActionCount = 1;

        TextField _iconObjectID;
        TextField _iconID;
        TextField _objectID;
        EnumField _interactType;
        EnumField _layoutType;
        Toggle _isVisible;

        public static CreateIconNode Create(Vector2 position)
        {
            CreateIconNode temp = new CreateIconNode();
            temp.GUID = "CreateIconNode" + ActionCount;
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

            temp._iconObjectID = new TextField("Icon Object ID: ");
            temp._iconObjectID.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(temp._iconObjectID);

            temp._iconID = new TextField("Icon ID: ");
            temp._iconID.SetValueWithoutNotify("oprions");
            temp.contentContainer.Add(temp._iconID);

            temp._objectID = new TextField("Object ID: ");
            temp.contentContainer.Add(temp._objectID);

            temp._interactType = new EnumField("Icon Type:", IconInteractType.Object);
            temp.contentContainer.Add(temp._interactType);

            temp._layoutType = new EnumField("Layout Type:", IconInteractType.SubLocation);
            temp.contentContainer.Add(temp._layoutType);

            temp._isVisible = new Toggle("Active");
            temp._isVisible.SetValueWithoutNotify(true);
            temp.contentContainer.Add(temp._isVisible);

            temp.ConditionSetup();

            return temp;
        }

        public static CreateIconNode LoadNode(JSONNode data)
        {
            CreateIconNode temp = new CreateIconNode();

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

            temp._iconObjectID = new TextField("Icon Object ID: ");
            temp._iconObjectID.SetValueWithoutNotify(data["NodeData"]["ID"].Value);
            temp.contentContainer.Add(temp._iconObjectID);

            temp._iconID = new TextField("Icon ID: ");
            temp._iconID.SetValueWithoutNotify(data["NodeData"]["IconID"].Value);
            temp.contentContainer.Add(temp._iconID);

            temp._objectID = new TextField("Object ID: ");
            temp._objectID.SetValueWithoutNotify(data["NodeData"]["ObjectID"].Value);
            temp.contentContainer.Add(temp._objectID);

            temp._interactType = new EnumField("Icon Type:", (IconInteractType)Enum.Parse(typeof(IconInteractType), data["NodeData"]["Type"].Value));
            temp.contentContainer.Add(temp._interactType);

            temp._layoutType = new EnumField("Layout Type:", (IconInteractType)Enum.Parse(typeof(IconInteractType), data["NodeData"]["Layout"].Value));
            temp.contentContainer.Add(temp._layoutType);

            temp._isVisible = new Toggle("Active");

            if (data["NodeData"]["IsActive"] != null)
                temp._isVisible.SetValueWithoutNotify(data["NodeData"]["IsActive"].AsBool);

            temp.contentContainer.Add(temp._isVisible);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("CreateIconNode");

            baseNode["NodeData"].Add("ID", this._iconObjectID.value);
            baseNode["NodeData"].Add("IconID", this._iconID.value);
            baseNode["NodeData"].Add("ObjectID", this._objectID.value);
            baseNode["NodeData"].Add("Type", this._interactType.value.ToString());
            baseNode["NodeData"].Add("Layout", this._layoutType.value.ToString());
            baseNode["NodeData"].Add("IsActive", this._isVisible.value.ToString());
            baseNode["NodeData"].Add("Base", "CreateIcon");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this._iconObjectID.value);
            baseNode.Add("IconID", this._iconID.value);
            baseNode.Add("ObjectID", this._objectID.value);
            baseNode.Add("Type", this._interactType.value.ToString());
            baseNode.Add("Layout", this._layoutType.value.ToString());
            baseNode.Add("IsActive", this._isVisible.value.ToString());
            baseNode.Add("Base", "CreateIcon");

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}