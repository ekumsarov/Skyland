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
    public class MainQuestWorkNode : EventNode
    {
        static int ActionCount = 1;

        List<QuestData> _questDates;

        TextField _titleQuest;
        TextField _iconQuest;
        TextField _descriptionQuest;

        EnumField _workType;

        public static MainQuestWorkNode Create(Vector2 position)
        {
            MainQuestWorkNode temp = new MainQuestWorkNode();
            temp.GUID = "MainQuestWork" + ActionCount;
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
            temp.contentContainer.Add(temp._titleQuest);

            temp._iconQuest = new TextField("Icon key");
            temp.contentContainer.Add(temp._iconQuest);

            temp._descriptionQuest = new TextField("Description key");
            temp.contentContainer.Add(temp._descriptionQuest);

            temp._questDates = new List<QuestData>();

            var buttonQuest = new Button(temp.AddQuest)
            {
                text = "Add Quest Node"
            };
            temp.titleButtonContainer.Add(buttonQuest);

            temp.ConditionSetup();

            return temp;
        }

        public static MainQuestWorkNode LoadNode(JSONNode data)
        {
            MainQuestWorkNode temp = new MainQuestWorkNode();

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

            temp._workType = new EnumField("Work Type", (EventWorkType)Enum.Parse(typeof(EventWorkType), data["NodeData"]["WorkType"].Value));
            temp.contentContainer.Add(temp._workType);

            temp._titleQuest = new TextField("Title key");
            temp._titleQuest.SetValueWithoutNotify(data["NodeData"]["MainQuest"]["Title"].Value);
            temp.contentContainer.Add(temp._titleQuest);

            temp._iconQuest = new TextField("Icon key");
            temp._iconQuest.SetValueWithoutNotify(data["NodeData"]["MainQuest"]["Icon"].Value);
            temp.contentContainer.Add(temp._iconQuest);

            temp._descriptionQuest = new TextField("Description key");
            temp._descriptionQuest.SetValueWithoutNotify(data["NodeData"]["MainQuest"]["Description"].Value);
            temp.contentContainer.Add(temp._descriptionQuest);

            temp._questDates = new List<QuestData>();

            JSONArray array = data["NodeData"]["Nodes"].AsArray;
            for(int i = 0; i < array.Count; i++)
            {
                temp._questDates.Add(QuestData.LoadNode(temp, array[i]));
            }

            var buttonQuest = new Button(temp.AddQuest)
            {
                text = "Add Quest Node"
            };
            temp.titleButtonContainer.Add(buttonQuest);

            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("MainQuestWork");

            baseNode["NodeData"].Add("Base", "QuestWork");
            baseNode["NodeData"].Add("WorkType", this._workType.value.ToString());

            baseNode["NodeData"].Add("ID", this._titleQuest.value);

            JSONNode node = new JSONClass();

            node.Add("Icon", this._iconQuest.value);
            node.Add("Description", this._descriptionQuest.value);
            node.Add("Title", this._titleQuest.value);

            JSONArray array = new JSONArray()
;            for(int i = 0; i < this._questDates.Count; i++)
            {
                array.Add(this._questDates[i].GetNode());
            }

            if(array.Count > 0)
                node.Add("Nodes", array);

            baseNode["NodeData"].Add("MainQuest", node);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("Base", "QuestWork");
            baseNode.Add("WorkType", this._workType.value.ToString());

            baseNode.Add("Icon", this._iconQuest.value);
            baseNode.Add("Description", this._descriptionQuest.value);
            baseNode.Add("Title", this._titleQuest.value);

            JSONArray array = new JSONArray();

            for (int i = 0; i < this._questDates.Count; i++)
            {
                array.Add(this._questDates[i].GetNode());
            }

            if (array.Count > 0)
                baseNode.Add("Nodes", array);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public void AddQuest()
        {
            this._questDates.Add(QuestData.Create(this));
        }

        public void RemoveQuestNode(QuestData node)
        {
            if (this._questDates.Contains(node))
                this._questDates.Remove(node);
        }
    }

    public class QuestData
    {
        MainQuestWorkNode parent;

        Button _removeButton;

        TextField _titleQuest;
        TextField _iconQuest;
        TextField _descriptionQuest;

        TextField _bindNodeID;

        Toggle _hideOnComplete;
        Toggle _visibility;

        Label _topLine;
        Label _bottomLine;

        public static QuestData Create(MainQuestWorkNode par)
        {
            QuestData temp = new QuestData();

            temp.parent = par;

            temp._topLine = new Label("________");
            temp.parent.contentContainer.Add(temp._topLine);

            temp._removeButton = new Button(temp.Remove)
            {
                text = "Remove"
            };
            temp.parent.titleButtonContainer.Add(temp._removeButton);

            temp._titleQuest = new TextField("Title key");
            temp.parent.contentContainer.Add(temp._titleQuest);

            temp._iconQuest = new TextField("Icon key");
            temp.parent.contentContainer.Add(temp._iconQuest);

            temp._descriptionQuest = new TextField("Description key");
            temp.parent.contentContainer.Add(temp._descriptionQuest);

            temp._bindNodeID = new TextField("BindQuest key");
            temp.parent.contentContainer.Add(temp._bindNodeID);

            temp._hideOnComplete = new Toggle("Hide on Complete");
            temp.parent.contentContainer.Add(temp._hideOnComplete);

            temp._visibility = new Toggle("Visibility");
            temp.parent.contentContainer.Add(temp._visibility);

            temp._bottomLine = new Label("________");
            temp.parent.contentContainer.Add(temp._bottomLine);

            return temp;
        }

        public static QuestData LoadNode(MainQuestWorkNode par, JSONNode data)
        {
            QuestData temp = new QuestData();

            temp.parent = par;

            temp._topLine = new Label("________");
            temp.parent.contentContainer.Add(temp._topLine);

            temp._removeButton = new Button(temp.Remove)
            {
                text = "Remove"
            };
            temp.parent.titleButtonContainer.Add(temp._removeButton);

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
            if(data["BindNode"] != null)
                temp._bindNodeID.SetValueWithoutNotify(data["BindNode"].Value);

            temp._hideOnComplete = new Toggle("Hide on Complete");
            temp._hideOnComplete.SetValueWithoutNotify(data["HideOnComplete"].AsBool);
            temp.parent.contentContainer.Add(temp._hideOnComplete);

            temp._visibility = new Toggle("Visibility");
            temp._visibility.SetValueWithoutNotify(data["SetMarkVisibility"].AsBool);
            temp.parent.contentContainer.Add(temp._visibility);

            temp._bottomLine = new Label("________");
            temp.parent.contentContainer.Add(temp._bottomLine);

            return temp;
        }

        public void Remove()
        {
            this.parent.Remove(this._topLine);
            this.parent.Remove(this._removeButton);
            this.parent.Remove(this._titleQuest);
            this.parent.Remove(this._iconQuest);
            this.parent.Remove(this._visibility);
            this.parent.Remove(this._hideOnComplete);
            this.parent.Remove(this._descriptionQuest);
            this.parent.Remove(this._bindNodeID);
            this.parent.Remove(this._bottomLine);

            this.parent.RemoveQuestNode(this);
        }

        public JSONNode GetNode()
        {
            JSONNode node = new JSONClass();

            node.Add("Title", this._titleQuest.value);
            node.Add("Icon", this._iconQuest.value);
            node.Add("Description", this._descriptionQuest.value);
            node.Add("HideOnComplete", this._hideOnComplete.value.ToString());
            node.Add("SetMarkVisibility", this._visibility.value.ToString());

            if(!this._bindNodeID.value.IsNullOrEmpty())
                node.Add("BindNode", this._bindNodeID.value);

            return node;
        }
    }
}