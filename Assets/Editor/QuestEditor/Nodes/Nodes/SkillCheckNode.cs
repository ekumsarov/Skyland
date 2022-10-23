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
    public class SkillCheckNode : BaseNode
    {
        static int ActionCount = 1;


        Port _successPort;
        Port _failPort;

        string SuccessID;
        string FailID;

        List<SkillCheckObjectNode> _skills;

        TextField _successSkillID;
        TextField _failSkillID;

        public static SkillCheckNode Create(Vector2 position)
        {
            SkillCheckNode temp = new SkillCheckNode();
            temp.GUID = "SkillCheck" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.SkillCheck;
            ActionCount += 1;

            temp._skills = new List<SkillCheckObjectNode>();

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            
            temp._successPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel = temp._successPort.contentContainer.Q<Label>("type");
            temp._successPort.contentContainer.Remove(portLabel);
            temp._successPort.portName = "Success" + temp.GUID;

            temp._failPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel2 = temp._failPort.contentContainer.Q<Label>("type");
            temp._failPort.contentContainer.Remove(portLabel2);
            temp._failPort.portName = "Fail" + temp.GUID;

            temp._successSkillID = new TextField("Success ID: ");
            temp._successSkillID.RegisterValueChangedCallback(evt =>
            {
                temp.SuccessID = evt.newValue;
                temp._successPort.portName = evt.newValue;
            });
            temp._successSkillID.SetValueWithoutNotify(temp.title);
            temp._successPort.contentContainer.Add(temp._successSkillID);

            temp._failSkillID = new TextField("Fail ID: ");
            temp._failSkillID.RegisterValueChangedCallback(evt =>
            {
                temp.FailID = evt.newValue;
                temp._failPort.portName = evt.newValue;
            });
            temp._failSkillID.SetValueWithoutNotify(temp.title);
            temp._failPort.contentContainer.Add(temp._failSkillID);

            var textField = new TextField("Action ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.contentContainer.Add(textField);

            var button = new Button(temp.AddSkillToCheck)
            {
                text = "Add Skill"
            };
            temp.titleButtonContainer.Add(button);

            temp.outputContainer.Add(temp._successPort);
            temp.outputContainer.Add(temp._failPort);

            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            return temp;
        }

        public static SkillCheckNode LoadNode(JSONNode data)
        {
            SkillCheckNode temp = new SkillCheckNode();

            temp.GUID = data["NodeData"]["ActionID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.SkillCheck;

            temp._skills = new List<SkillCheckObjectNode>();

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);

            temp._successPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel = temp._successPort.contentContainer.Q<Label>("type");
            temp._successPort.contentContainer.Remove(portLabel);
            temp._successPort.portName = "Success" + temp.GUID;

            temp._failPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            var portLabel2 = temp._failPort.contentContainer.Q<Label>("type");
            temp._failPort.contentContainer.Remove(portLabel2);
            temp._failPort.portName = "Fail" + temp.GUID;

            temp._successSkillID = new TextField("Success ID: ");
            temp._successSkillID.RegisterValueChangedCallback(evt =>
            {
                temp.SuccessID = evt.newValue;
                temp._successPort.portName = evt.newValue;
            });
            temp._successSkillID.SetValueWithoutNotify(data["NodeData"]["SuccessID"].Value);
            temp._successPort.contentContainer.Add(temp._successSkillID);


            temp._failSkillID = new TextField("Fail ID: ");
            temp._failSkillID.RegisterValueChangedCallback(evt =>
            {
                temp.FailID = evt.newValue;
                temp._failPort.portName = evt.newValue;
            });
            temp._failSkillID.SetValueWithoutNotify(data["NodeData"]["FailID"].Value);
            temp._failPort.contentContainer.Add(temp._failSkillID);

            var textField = new TextField("Action ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp.GUID = evt.newValue;
                temp.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.title);
            temp.mainContainer.Add(textField);

            var button = new Button(temp.AddSkillToCheck)
            {
                text = "Add Skill"
            };
            temp.titleButtonContainer.Add(button);

            temp.outputContainer.Add(temp._successPort);
            temp.outputContainer.Add(temp._failPort);

            JSONArray successArray = data["NodeData"]["SuccessCheck"].AsArray;
            for(int i = 0; i < successArray.Count; i++)
            {
                temp._skills.Add(SkillCheckObjectNode.Load(temp, successArray[i]));
            }

            JSONArray failArray = data["NodeData"]["BadCheck"].AsArray;
            for (int i = 0; i < failArray.Count; i++)
            {
                temp._skills.Add(SkillCheckObjectNode.Load(temp, failArray[i]));
            }

            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(MyString.JSONToVector2(data["Node"]["Position"]),
                new Vector2(200, 150)));

            return temp;
        }

        public void AddSkillToCheck()
        {
            this._skills.Add(SkillCheckObjectNode.Create(this));
        }

        public override void SetupConnectedNode(BaseNode node, Port updatedPort)
        {
            if (updatedPort == _successPort)
            {
                _successSkillID.SetValueWithoutNotify(node.GUID);
                SuccessID = node.GUID;
            }
            else if (updatedPort == _failPort)
            {
                _failSkillID.SetValueWithoutNotify(node.GUID);
                FailID = node.GUID;
            }
            else
                Debug.LogError("Something go wrong");
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("SkillCheck");

            baseNode["NodeData"].Add("ActionID", this.GUID);
            baseNode["NodeData"].Add("ActionType", "SkillCheck");
            baseNode["NodeData"].Add("WorkType", "Add");
            baseNode["NodeData"].Add("Base", "SkillCheckWork");

            JSONArray successArray = new JSONArray();
            JSONArray failArray = new JSONArray();

            for(int i = 0; i < this._skills.Count; i++)
            {
                if (this._skills[i].isSuccess)
                    successArray.Add(this._skills[i].GetNode());
                else
                    failArray.Add(this._skills[i].GetNode());
            }

            baseNode["NodeData"].Add("SuccessCheck", successArray);
            baseNode["NodeData"].Add("BadCheck", failArray);

            baseNode["NodeData"].Add("SuccessID", SuccessID);
            baseNode["NodeData"].Add("FailID", FailID);

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("ID", this.GUID);
            baseNode.Add("Base", "CallAction");

            return baseNode;
        }

        public override Port GetOuputPort(string portID)
        {
            return portID.Equals(_successPort.portName) ? _successPort : _failPort;
        }

        public void RemoveSkill(SkillCheckObjectNode skill)
        {
            this._skills.Remove(skill);
        }
    }

    public class SkillCheckObjectNode
    {
        SkillCheckNode parent;

        Button _removeButton;

        Label _topLine;
        Label _bottomLine;

        Toggle _skillCheckSuccess;

        EnumField _skillID;
        IntegerField _skillAmount;
        IntegerField _skillComplex;
        IntegerField _skillMax;

        public static SkillCheckObjectNode Create(SkillCheckNode parent)
        {
            SkillCheckObjectNode temp = new SkillCheckObjectNode();
            temp.parent = parent;

            temp._skillCheckSuccess = new Toggle("IsToSuccess");
            temp._skillCheckSuccess.SetValueWithoutNotify(true);

            temp._skillID = new EnumField("Skill ID:", SkillType.strenght);
            temp._skillComplex = new IntegerField("Skill Complex");
            temp._skillAmount = new IntegerField("Skill Amount");
            temp._skillMax = new IntegerField("Skill Max");
            temp._skillMax.SetValueWithoutNotify(0);

            temp._topLine = new Label("________");
            temp._bottomLine = new Label("________");

            temp._removeButton = new Button(temp.Clean)
            {
                text = "Remmove"
            };

            temp.parent.contentContainer.Add(temp._topLine);
            temp.parent.contentContainer.Add(temp._removeButton);
            temp.parent.contentContainer.Add(temp._skillCheckSuccess);
            temp.parent.contentContainer.Add(temp._skillID);
            temp.parent.contentContainer.Add(temp._skillAmount);
            temp.parent.contentContainer.Add(temp._skillComplex);
            temp.parent.contentContainer.Add(temp._skillMax);
            temp.parent.contentContainer.Add(temp._bottomLine);

            return temp;
        }

        public static SkillCheckObjectNode Load(SkillCheckNode parent, JSONNode data)
        {
            SkillCheckObjectNode temp = new SkillCheckObjectNode();
            temp.parent = parent;

            temp._skillCheckSuccess = new Toggle("IsToSuccess");
            temp._skillCheckSuccess.SetValueWithoutNotify(data["isSuccess"].AsBool);

            temp._skillID = new EnumField("Skill ID:", SkillType.strenght);
            temp._skillID.SetValueWithoutNotify((SkillType)Enum.Parse(typeof(SkillType), data["id"].Value));
            temp._skillComplex = new IntegerField("Skill Complex");
            temp._skillComplex.SetValueWithoutNotify(data["complex"].AsInt);
            temp._skillAmount = new IntegerField("Skill Amount");
            temp._skillAmount.SetValueWithoutNotify(data["amount"].AsInt);
            temp._skillMax = new IntegerField("Skill Max");
            temp._skillMax.SetValueWithoutNotify(data["max"].AsInt);

            temp._topLine = new Label("________");
            temp._bottomLine = new Label("________");

            temp._removeButton = new Button(temp.Clean)
            {
                text = "Remmove"
            };

            temp.parent.contentContainer.Add(temp._topLine);
            temp.parent.contentContainer.Add(temp._removeButton);
            temp.parent.contentContainer.Add(temp._skillCheckSuccess);
            temp.parent.contentContainer.Add(temp._skillID);
            temp.parent.contentContainer.Add(temp._skillAmount);
            temp.parent.contentContainer.Add(temp._skillComplex);
            temp.parent.contentContainer.Add(temp._skillMax);
            temp.parent.contentContainer.Add(temp._bottomLine);

            return temp;
        }

        public void Clean()
        {
            parent.contentContainer.Remove(_topLine);
            parent.contentContainer.Remove(_removeButton);
            parent.contentContainer.Remove(_skillCheckSuccess);
            parent.contentContainer.Remove(_skillID);
            parent.contentContainer.Remove(_skillAmount);
            parent.contentContainer.Remove(_skillComplex);
            parent.contentContainer.Remove(_skillMax);
            parent.contentContainer.Remove(_bottomLine);
        }

        public bool isSuccess
        {
            get { return this._skillCheckSuccess.value; }
        }

        public JSONNode GetNode()
        {
            JSONNode node = new JSONClass();

            node.Add("isSuccess", this._skillCheckSuccess.value.ToString());
            node.Add("id", this._skillID.value.ToString());
            node.Add("complex", this._skillComplex.value.ToString());
            node.Add("amount", this._skillAmount.value.ToString());
            node.Add("max", this._skillMax.value.ToString());

            return node;
        }
    }
}