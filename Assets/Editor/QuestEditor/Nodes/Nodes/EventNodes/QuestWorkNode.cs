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
    public class QuestWorkNode : EventNode
    {
        static int ActionCount = 1;

        TextField _titleQuest;
        TextField _iconQuest;
        TextField _descriptionQuest;

        TextField _bindNodeID;

        Toggle _hideOnComplete;
        Toggle _visibility;

        EnumField _workType;

        public static QuestWorkNode Create(Vector2 position)
        {
            QuestWorkNode temp = new QuestWorkNode();
            temp.GUID = "QuestWork" + ActionCount;
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

            temp._workType = new EnumField("Work Type", EventWorkType.Add);
            temp.contentContainer.Add(temp._workType);

            temp._titleQuest = new TextField("Title key");
            temp.parent.contentContainer.Add(temp._titleQuest);

            temp._iconQuest = new TextField("Icon key");
            temp.contentContainer.Add(temp._iconQuest);

            temp._descriptionQuest = new TextField("Description key");
            temp.contentContainer.Add(temp._descriptionQuest);

            temp._bindNodeID = new TextField("BindQuest key");
            temp.contentContainer.Add(temp._bindNodeID);

            temp._hideOnComplete = new Toggle("Hide on Complete");
            temp.contentContainer.Add(temp._hideOnComplete);

            temp._visibility = new Toggle("Visibility");
            temp.contentContainer.Add(temp._visibility);

            return temp;
        }

        public static QuestWorkNode LoadNode(JSONNode data)
        {
            QuestWorkNode temp = new QuestWorkNode();

            temp.GUID = data["NodeData"]["ActionID"].Value;
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

            temp._workType = new EnumField("Work Type", (EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));
            temp.contentContainer.Add(temp._workType);

            temp._titleQuest = new TextField("Title key");
            temp._titleQuest.SetValueWithoutNotify(data["Title"].Value);
            temp.parent.contentContainer.Add(temp._titleQuest);

            temp._iconQuest = new TextField("Icon key");
            temp._titleQuest.SetValueWithoutNotify(data["Icon"].Value);
            temp.parent.contentContainer.Add(temp._iconQuest);

            temp._descriptionQuest = new TextField("Description key");
            temp._titleQuest.SetValueWithoutNotify(data["Description"].Value);
            temp.parent.contentContainer.Add(temp._descriptionQuest);

            temp._bindNodeID = new TextField("BindQuest key");
            temp.parent.contentContainer.Add(temp._bindNodeID);
            if (data["BindNode"] != null)
                temp._bindNodeID.SetValueWithoutNotify(data["BindNode"].Value);

            temp._hideOnComplete = new Toggle("Hide on Complete");
            temp._hideOnComplete.SetValueWithoutNotify(data["HideOnComplete"].AsBool);
            temp.parent.contentContainer.Add(temp._hideOnComplete);

            temp._visibility = new Toggle("Visibility");
            temp._visibility.SetValueWithoutNotify(data["SetMarkVisibility"].AsBool);
            temp.parent.contentContainer.Add(temp._visibility);



            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("QuestWork");

            baseNode["NodeData"].Add("Base", "QuestWork");
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());

            JSONNode node = new JSONClass();

            node.Add("Title", this._titleQuest.value);
            node.Add("Icon", this._iconQuest.value);
            node.Add("Description", this._descriptionQuest.value);
            node.Add("HideOnComplete", this._hideOnComplete.value.ToString());
            node.Add("SetMarkVisibility", this._visibility.value.ToString());

            if(!this._bindNodeID.value.IsNullOrEmpty())
                node.Add("BindNode", this._bindNodeID.value);

            baseNode["NodeData"].Add("NodeQuest", node);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}