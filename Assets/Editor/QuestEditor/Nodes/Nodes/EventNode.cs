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
    public class EventNode : BaseNode
    {
        protected List<ConditionNode> _conditions;
        protected int conditionCount = 0;

        public static BaseNode GetEvent(string eventID, Vector2 mousePosition)
        {
            
            switch (eventID)
            {
                case "FlagWork":
                    return FlagWorkNode.Create(mousePosition);

                case "CreateIcon":
                    return CreateIconNode.Create(mousePosition);

                case "QuestWork":
                    return QuestWorkNode.Create(mousePosition);

                case "MainQuestWork":
                    return MainQuestWorkNode.Create(mousePosition);

                case "ShowTooltip":
                    return ShowTooltipNode.Create(mousePosition);

                case "CallPackNode":
                    return CallPackNode.Create(mousePosition);

                case "ShowReward":
                    return ShowRewardNode.Create(mousePosition);

                case "AddStatNode":
                    return AddStatNode.Create(mousePosition);

                case "AddHero":
                    return AddHeroNode.Create(mousePosition);

                case "LootWorkNode":
                    return LootWorkNode.Create(mousePosition);

                case "BattleNode":
                    return BattleNode.Create(mousePosition);

                case "AddStatProductNode":
                    return AddStatProductNode.Create(mousePosition);

                case "SetupMainEventNode":
                    return SetupMainEventNode.Create(mousePosition);

                case "ActivateSubLocationNode":
                    return ActivateSubLocationNode.Create(mousePosition);

                case "ActivateObjectNode":
                    return ActivateObjectNode.Create(mousePosition);

                case "MapIconAdditionalEventNode":
                    return MapIconAdditionalEventNode.Create(mousePosition);

                case "MoveIconNode":
                    return MoveIconNode.Create(mousePosition);

                case "LockObjectNode":
                    return LockObjectNode.Create(mousePosition);

                case "ActionWorkNode":
                    return ActionWorkNode.Create(mousePosition);

                case "CallEventFunctionNode":
                    return CallEventFunctionNode.Create(mousePosition);

                case "AddEventNode":
                    return AddEventNode.Create(mousePosition);

                case "StatCreateNode":
                    return StatCreateNode.Create(mousePosition);

                case "ActionChoiceWorkNode":
                    return ActionChoiceWorkNode.Create(mousePosition);

                case "ActivateStatNode":
                    return ActivateStatNode.Create(mousePosition);

                case "CallActionNode":
                    return CallActionNode.Create(mousePosition);

                default:
                    return null;
            }
        }

        public static BaseNode LoadEvent(JSONNode data)
        {

            switch (data["Node"]["Type"].Value)
            {
                case "FlagWork":
                    return FlagWorkNode.LoadNode(data);

                case "CreateIconNode":
                    return CreateIconNode.LoadNode(data);

                case "QuestWork":
                    return QuestWorkNode.LoadNode(data);

                case "MainQuestWork":
                    return MainQuestWorkNode.LoadNode(data);

                case "ShowTooltip":
                    return ShowTooltipNode.LoadNode(data);

                case "LootWorkNode":
                    return LootWorkNode.LoadNode(data);

                case "CallPackNode":
                    return CallPackNode.LoadNode(data);

                case "AddHeroNode":
                    return AddHeroNode.LoadNode(data);

                case "BattleNode":
                    return BattleNode.LoadNode(data);

                case "AddStatNode":
                    return AddStatNode.LoadNode(data);

                case "AddStatProductNode":
                    return AddStatProductNode.LoadNode(data);

                case "SetupMainEventNode":
                    return SetupMainEventNode.LoadNode(data);

                case "ActivateSubLocationNode":
                    return ActivateSubLocationNode.LoadNode(data);

                case "ActivateObjectNode":
                    return ActivateObjectNode.LoadNode(data);

                case "MapIconAdditionalEventNode":
                    return MapIconAdditionalEventNode.LoadNode(data);

                case "MoveIconNode":
                    return MoveIconNode.LoadNode(data);

                case "LockObjectNode":
                    return LockObjectNode.LoadNode(data);

                case "ActionWorkNode":
                    return ActionWorkNode.LoadNode(data);

                case "CallEventFunctionNode":
                    return CallEventFunctionNode.LoadNode(data);

                case "AddEventNode":
                    return AddEventNode.LoadNode(data);

                case "StatCreateNode":
                    return StatCreateNode.LoadNode(data);

                case "ActionChoiceWorkNode":
                    return ActionChoiceWorkNode.LoadNode(data);

                case "ActivateStatNode":
                    return ActivateStatNode.LoadNode(data);

                case "CallActionNode":
                    return CallActionNode.LoadNode(data);

                default:
                    return null;
            }
        }

        public override void RemoveCondition(string ID)
        {
            if(this._conditions != null)
            {
                ConditionNode condition = this._conditions.FirstOrDefault(con => con.ID.Equals(ID));
                if (condition != null)
                    this._conditions.Remove(condition);

                condition = null;
            }
        }

        public void ConditionSetup()
        {
            if (this._conditions == null)
                this._conditions = new List<ConditionNode>();

            var button = new Button(() =>
            {
                this._conditions.Add(ConditionNode.Create("Condition" + conditionCount, this));
                this.conditionCount++;
            })
            {
                text = "Add Condition"
            };
            this.contentContainer.Add(button);
        }

        public void LoadConditions(JSONArray cons)
        {
            if (this._conditions == null)
                this._conditions = new List<ConditionNode>();

            for(int i = 0; i < cons.Count; i++)
            {
                this._conditions.Add(ConditionNode.LoadCondition(cons[i], this));
                this.conditionCount++;
            }
        }

        public JSONArray GetConditionsNode()
        {
            JSONArray node = new JSONArray();

            for(int i = 0; i < this._conditions.Count; i++)
            {
                node.Add(this._conditions[i].GetNode());
            }

            return node;
        }
    }
}