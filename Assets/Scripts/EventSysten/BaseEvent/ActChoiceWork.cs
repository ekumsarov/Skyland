using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActChoiceWork : GameEvent
    {
        string WorkType;

        string To;

        string ActionID;
        bool OnClear;
        bool restore;
        string ActID;

        ActionButtonInfo actions;

        SkyObject parent;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActChoiceWork";

            WorkType = "Add";
            if (node["WorkType"] != null)
                WorkType = node["WorkType"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            ActionID = null;
            if (node["ActionID"] != null)
                ActionID = node["ActionID"].Value;

            ActID = null;
            if (node["ChoiceID"] != null)
                ActID = node["ChoiceID"].Value;

            if (node["Choice"] != null)
            {
                ActionButtonInfo.ABIEventBuilder button = ActionButtonInfo.ABIEventBuilder.Create(ActID);
                if (node["Choice"]["CallData"] != null)
                    button.SetCallData(node["Choice"]["CallData"].Value);

                if (node["Choice"]["Text"] != null)
                    button.SetText(node["Choice"]["Text"].Value);

                if (node["Choice"]["Type"] != null)
                    button.SetType((ActionType)Enum.Parse(typeof(ActionType), node["Choice"]["Type"].Value));

                if (node["Choice"]["Conditions"] != null)
                {
                    JSONArray conditions = node["Choice"]["Conditions"].AsArray;
                    for (int j = 0; j < conditions.Count; j++)
                    {
                        JSONNode condition = node["Choice"]["Conditions"][j];

                        if (condition["isVisibleCondition"] != null && condition["isVisibleCondition"].AsBool == true)
                            button.SetAppearCondition(MakeCondition(condition));
                        else
                            button.SetAvailableCondition(MakeCondition(condition));
                    }
                }

                actions = button.GetButton();
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (ActID.IsNullOrEmpty() || ActionID.IsNullOrEmpty())
                return false;

            return base.CanActive();
        }

        public override void Start()
        {
            parent = GetObject(To);

            if (WorkType.Equals("Remove") && ActID != null)
                parent.RepmoveActionChoice(ActionID, ActID);
            else
            {
                if (WorkType.Equals("Replace") && ActID != null)
                    parent.ReplaceActionChoice(ActionID, ActID, actions);
                else
                    parent.AddActionChoice(ActionID, actions);
            }

            End();
        }

        #region static
        public static ActChoiceWork Create(string To, string ActionID, string chID, ActionButtonInfo _actionsChoice = null, string _ActType = "Add")
        {
            ActChoiceWork temp = new ActChoiceWork();
            temp.ID = "ActChoiceWork";

            temp.To = To;
            temp.ActionID = ActionID;
            temp.ActID = chID;
            temp.actions = _actionsChoice;
            temp.WorkType = _ActType;

            return temp;
        }
        #endregion
    }

}