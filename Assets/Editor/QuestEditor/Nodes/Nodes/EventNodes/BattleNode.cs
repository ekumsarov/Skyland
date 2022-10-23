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
    public class BattleNode : EventNode
    {
        static int ActionCount = 1;

        TextField _playerArmy;
        TextField _enemyArmy;
        TextField _enemyObjectID;

        Port _successPort;
        Port _failPort;

        TextField _successSkillID;
        TextField _failSkillID;

        string SuccessID;
        string FailID;

        public static BattleNode Create(Vector2 position)
        {
            BattleNode temp = new BattleNode();
            temp.GUID = "BattleNode" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            temp._playerArmy = new TextField("Player: ");
            temp._playerArmy.SetValueWithoutNotify("Classic");
            temp.contentContainer.Add(temp._playerArmy);

            temp._enemyArmy = new TextField("Enemy: ");
            temp._enemyArmy.SetValueWithoutNotify("Classic");
            temp.contentContainer.Add(temp._enemyArmy);

            temp._enemyObjectID = new TextField("EnemyID: ");
            temp.contentContainer.Add(temp._enemyObjectID);

            ///////
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

            temp.outputContainer.Add(temp._successPort);
            temp.outputContainer.Add(temp._failPort);

            temp.ConditionSetup();

            temp.RefreshExpandedState();
            temp.RefreshPorts();

            return temp;
        }

        public static BattleNode LoadNode(JSONNode data)
        {
            BattleNode temp = new BattleNode();

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

            temp._playerArmy = new TextField("Player: ");
            temp._playerArmy.SetValueWithoutNotify(data["NodeData"]["Player"].Value);
            temp.contentContainer.Add(temp._playerArmy);

            temp._enemyArmy = new TextField("Enemy: ");
            temp._enemyArmy.SetValueWithoutNotify(data["NodeData"]["Enemy"].Value);
            temp.contentContainer.Add(temp._enemyArmy);

            temp._enemyObjectID = new TextField("EnemyID: ");
            temp._enemyObjectID.SetValueWithoutNotify(data["NodeData"]["EnemyObject"].Value);
            temp.contentContainer.Add(temp._enemyObjectID);

            /////////////
            ///
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
            temp._successSkillID.SetValueWithoutNotify(data["NodeData"]["Result"]["SuccessID"].Value);
            temp._successPort.contentContainer.Add(temp._successSkillID);


            temp._failSkillID = new TextField("Fail ID: ");
            temp._failSkillID.RegisterValueChangedCallback(evt =>
            {
                temp.FailID = evt.newValue;
                temp._failPort.portName = evt.newValue;
            });
            temp._failSkillID.SetValueWithoutNotify(data["NodeData"]["Result"]["FailID"].Value);
            temp._failPort.contentContainer.Add(temp._failSkillID);

            temp.outputContainer.Add(temp._successPort);
            temp.outputContainer.Add(temp._failPort);

            ////////
            temp.ConditionSetup();
            if (data["NodeData"]["Conditions"] != null)
                temp.LoadConditions(data["NodeData"]["Conditions"].AsArray);

            temp.RefreshExpandedState();
            temp.RefreshPorts();

            return temp;
        }

        public override Port GetOuputPort(string portID)
        {
            return portID.Equals(_successPort.portName) ? _successPort : _failPort;
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
            JSONNode baseNode = this.GetBaseNode("BattleNode");

            baseNode["NodeData"].Add("Base", "BattleEvent");

            baseNode["NodeData"].Add("Player", this._playerArmy.value);
            baseNode["NodeData"].Add("Enemy", this._enemyArmy.value);
            baseNode["NodeData"].Add("EnemyObject", this._enemyObjectID.value);

            JSONNode resultNode = new JSONClass();

            resultNode.Add("SuccessID", this.SuccessID);
            resultNode.Add("FailID", this.FailID);

            baseNode["NodeData"].Add("Result", resultNode);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode["NodeData"].Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("Base", "BattleEvent");

            baseNode.Add("Player", this._playerArmy.value);
            baseNode.Add("Enemy", this._enemyArmy.value);
            baseNode.Add("EnemyObject", this._enemyObjectID.value);

            JSONNode resultNode = new JSONClass();

            resultNode.Add("SuccessID", this.SuccessID);
            resultNode.Add("FailID", this.FailID);

            baseNode.Add("Result", resultNode);

            if (this._conditions != null && this._conditions.Count > 0)
                baseNode.Add("Conditions", this.GetConditionsNode());

            return baseNode;
        }
    }
}