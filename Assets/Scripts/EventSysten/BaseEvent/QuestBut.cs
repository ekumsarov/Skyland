using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents
{
    public class QuestBut : GameEvent
    {
        string _id;
        string To;
        string Ico;
        string TooltipStr;
        string EventID;

        bool MakeAction;
        string ActType;
        string Title;
        string Description;
        string Reward;
        List<ActionButtonInfo> actions;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "QuestButton";

            this._id = "";
            if (node["ID"] != null)
                this._id = node["ID"].Value;

            this.To = "Manager";
            if (node["To"] != null)
                this.To = node["To"].Value;
            
            this.Ico = "";
            if (node["Ico"] != null)
                this.Ico = node["Ico"].Value;

            this.EventID = this._id;
            if (node["EventID"] != null)
                this.Ico = node["EventID"].Value;

            this.TooltipStr = "";
            if (node["TooltipStr"] != null)
                this.TooltipStr = node["TooltipStr"].Value;

            this.MakeAction = false;
            if (node["MakeAction"] != null)
                this.MakeAction = node["MakeAction"].AsBool;

            if (this.MakeAction)
            {
                this.ActType = "quest";
                if (node["ActionType"] != null)
                    this.ActType = node["ActionType"].Value;

                this.Title = null;
                if (node["Title"] != null)
                    this.Title = node["Title"].Value;

                this.Description = null;
                if (node["Des"] != null)
                    this.Description = node["Des"].Value;

                this.Reward = null;
                if (node["Reward"] != null)
                    this.Reward = node["Reward"].Value;
                
                JSONArray arr = null;
                if (node["Actions"] != null)
                    arr = node["Actions"].AsArray;

                if (arr != null)
                {
                    actions = new List<ActionButtonInfo>();

                    for (int i = 0; i < arr.Count; i++)
                    {
                        string aID = "action" + i;
                        if (arr[i]["Text"] != null)
                            aID = arr[i]["Text"].Value;

                        ActionButtonInfo.ABIEventBuilder button = ActionButtonInfo.ABIEventBuilder.Create(aID);
                        if (arr[i]["CallID"] != null)
                            button.SetCallData(arr[i]["CallID"].Value);

                        if (arr[i]["type"] != null)
                            button.SetType((ActionType)Enum.Parse(typeof(ActionType), arr[i]["type"].Value));

                        if (arr[i]["Condition"] != null)
                            button.SetAvailableCondition(MakeCondition(arr[i]["Condition"]));

                        actions.Add(button.GetButton());
                    }
                }
            }
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            SkyObject par = GetObject(To);

            QuestButton.Create(this._id, par, this.Ico, this.TooltipStr, this.EventID);
            
            if(this.MakeAction)
            {
                Actions act = Actions.Get(ActType);
                act.Text = this.Reward == null ? LocalizationManager.Get("QuestTemplate", LocalizationManager.Get(this.Title), LocalizationManager.Get(this.Description)) 
                    : LocalizationManager.Get("QuestTemplate", LocalizationManager.Get(this.Title), LocalizationManager.Get(this.Description)) + LocalizationManager.Get("QuestTemplateReward", LocalizationManager.Get(this.Reward));
                act.ID = this._id;

                if (actions != null)
                    act.list.AddRange(actions);

                par.AddAction(act);

            }

            End();
        }

        #region static
        public static QuestBut Create(string _id, string to = "Manager", string actionType = "quest", string ico = "", string TooltipStr = "", string Text = "MissionPangramm", List<ActionButtonInfo> actButs = null)
        {
            /* TODO:
             * remake this part
             */

            QuestBut temp = new QuestBut();
            temp.ID = "QuestBut";

            temp._id = _id;
            temp.To = to;
            temp.ActType = actionType;
            temp.Ico = ico;
            temp.TooltipStr = TooltipStr;
            temp.Title = Text;

            if(actButs != null)
                temp.actions = actButs;

            return temp;
        }
        #endregion
    }
}