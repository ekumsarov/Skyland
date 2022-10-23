using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActionWork : GameEvent
    {
        string WorkType;
        
        string To;

        string ActType;
        string ActionID;
        string Text;

        List<ActionButtonInfo> actions;

        SkyObject parent;

        Actions completeAction;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActionWork";

            WorkType = "Add";
            if (node["WorkType"] != null)
                WorkType = node["WorkType"].Value;

            ActType = "simple";
            if (node["ActionType"] != null)
                ActType = node["ActionType"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            ActionID = "Simple";
            if (node["ActionID"] != null)
                ActionID = node["ActionID"].Value;

            Text = string.Empty;
            if (node["Text"] != null)
                Text = node["Text"].Value;

            JSONArray arr = null;
            if (node["Actions"] != null)
                arr = node["Actions"].AsArray;

            if (arr != null)
            {
                actions = new List<ActionButtonInfo>();

                for (int i = 0; i < arr.Count; i++)
                {
                    string aID = "action" + i;
                    if (arr[i]["ID"] != null)
                        aID = arr[i]["ID"].Value;

                    ActionButtonInfo.ABIEventBuilder button = ActionButtonInfo.ABIEventBuilder.Create(aID);
                    if (arr[i]["CallData"] != null)
                        button.SetCallData(arr[i]["CallData"].Value);

                    if (arr[i]["Text"] != null)
                        button.SetText(arr[i]["Text"].Value);

                    if (arr[i]["Type"] != null)
                        button.SetType((ActionType)Enum.Parse(typeof(ActionType), arr[i]["Type"].Value));

                    if (arr[i]["Conditions"] != null)
                    {
                        JSONArray conditions = arr[i]["Conditions"].AsArray;
                        for(int j = 0; j < conditions.Count; j++)
                        {
                            JSONNode condition = arr[i]["Conditions"][j];

                            if(condition["isVisibleCondition"] != null && condition["isVisibleCondition"].AsBool == true)
                                button.SetAppearCondition(MakeCondition(condition));
                            else
                                button.SetAvailableCondition(MakeCondition(condition));
                        }
                    }
                        
                    actions.Add(button.GetButton());
                }
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            parent = GetObject(To);

            if (WorkType.Equals("Remove"))
                parent.RepmoveAction(ActionID);
            else if (WorkType.Equals("ChangeText"))
            {
                parent.ChangeActionText(ActionID, Text);
            }
            else
            {

                if(completeAction == null)
                {
                    completeAction = Actions.Get(ActType);

                    completeAction.Text = Text;
                    completeAction.ID = ActionID;

                    if (actions != null)
                        completeAction.list.AddRange(actions);
                }
                

                if (WorkType.Equals("Replace"))
                    parent.ReplaceAction(completeAction);
                else
                    parent.AddAction(completeAction);
            }

            End();
        }

        #region static
        public static ActionWork Create(string ActionID, string To, List<ActionButtonInfo> _actions, string _ActType = "Context", string _WorkType = "Add",
            string text = "MissionPangramm", Actions act = null)
        {
            ActionWork temp = new ActionWork();
            temp.ID = "ActionWork";

            temp.To = To;
            temp.WorkType = _WorkType;
            temp.ActionID = ActionID;
            temp.actions = _actions;
            temp.ActType = _ActType;
            temp.Text = text;
            temp.completeAction = act;

            return temp;
        }

        public static ActionWork CreateAddAction(string To, Actions act)
        {
            ActionWork temp = new ActionWork();
            temp.ID = "ActionWork";

            temp.To = To;
            temp.WorkType = "Add";
            temp.completeAction = act;

            return temp;
        }
        #endregion
    }
}