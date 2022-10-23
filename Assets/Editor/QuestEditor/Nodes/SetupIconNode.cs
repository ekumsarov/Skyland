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
    public class SetupIconNode : BaseNode
    {
        TextField _iconObjectID;
        TextField _iconID;
        TextField _objectID;
        EnumField _interactType;
        EnumField _layoutType;
        Toggle _isVisible;

        Port output;

        string MainEvent = string.Empty;

        public static SetupIconNode Create(Vector2 position)
        {
            SetupIconNode temp = new SetupIconNode();
            temp.GUID = "SetupIconNode";
            temp.title = temp.GUID;
            temp.Type = NodeType.Setup;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
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

            temp.output = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            temp.output.portName = "MainEvent";
            var portLabel = temp.output.contentContainer.Q<Label>("type");
            portLabel.text = "MainEvent";
            temp.outputContainer.Add(temp.output);

            temp.RefreshPorts();
            temp.RefreshExpandedState();

            return temp;
        }

        public static SetupIconNode LoadNode(JSONNode data)
        {
            SetupIconNode temp = new SetupIconNode();

            temp.GUID = "SetupIconNode";
            temp.title = temp.GUID;
            temp.Type = NodeType.Setup;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
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

            if(data["NodeData"]["IsActive"]!=null)
                temp._isVisible.SetValueWithoutNotify(data["NodeData"]["IsActive"].AsBool);

            temp.contentContainer.Add(temp._isVisible);

            if (data["NodeData"]["MainEvent"] != null)
                temp.MainEvent = data["NodeData"]["MainEvent"].Value;

            temp.output = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            temp.output.portName = "MainEvent";
            var portLabel = temp.output.contentContainer.Q<Label>("type");
            portLabel.text = "MainEvent";
            temp.outputContainer.Add(temp.output);

            temp.RefreshPorts();
            temp.RefreshExpandedState();

            return temp;
        }

        public override Port GetOuputPort(string portID)
        {
            return output;
        }

        public override void SetupConnectedNode(BaseNode node, Port updatedPort)
        {
            this.MainEvent = node.GUID;

            return;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("SetupIconNode");

            baseNode["NodeData"].Add("ID", this._iconObjectID.value);
            baseNode["NodeData"].Add("IconID", this._iconID.value);
            baseNode["NodeData"].Add("ObjectID", this._objectID.value);
            baseNode["NodeData"].Add("Type", this._interactType.value.ToString());
            baseNode["NodeData"].Add("Layout", this._layoutType.value.ToString());
            baseNode["NodeData"].Add("IsActive", this._isVisible.value.ToString());
            baseNode["NodeData"].Add("Base", "CreateIcon");

            if(!this.MainEvent.IsNullOrEmpty())
                baseNode["NodeData"].Add("MainEvent", this.MainEvent);

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this.GUID);
            baseNode.Add("Base", "CallAction");

            return baseNode;
        }
    }
}

    
