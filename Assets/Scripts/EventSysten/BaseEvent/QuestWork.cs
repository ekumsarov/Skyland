using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents
{
    public class QuestWork : GameEvent
    {
        //
        // On & Off in this Event means Complete or Fail Quest
        //

        string _id;

        MainQuest mQuest = null;
        QuestNode nQuest = null;

        string ActionType = "Add";

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "QuestWork";

            this._id = "";
            if (node["Title"] != null)
                this._id = node["Title"].Value;

            if (node["WorkType"] != null)
                this.ActionType = node["WorkType"].Value;

            if (node["MainQuest"] != null)
            {
                mQuest = MainQuest.Create(this._id, node["MainQuest"]);
                return;
            }

            if(node["NodeQuest"] != null)
            {
                nQuest = QuestNode.Create(this._id, node["NodeQuest"]);
                return;
            }

            if(node["Title"]!= null)
            {
                mQuest = MainQuest.Create(node["Title"].Value);
                mQuest.SetIcon(node["Icon"].Value);
                mQuest.SetDescription(node["Description"].Value);
            }

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            if(ActionType.Equals("Add"))
            {
                if (mQuest != null)
                    QS.AddQuest(mQuest);
                else if (nQuest != null)
                    QS.AddQuest(nQuest);
            }

            if(ActionType.Equals("Off"))
            {
                QS.CompleteQuest(this._id);
            }

            if(ActionType.Equals("On"))
            {
                QS.CompleteQuest(this._id);
            }

            if(ActionType.Equals("Remove"))
            {
                QS.RemoveQuest(this._id);
            }

            End();
        }

        #region static
        public static QuestWork Create(string id, string actionType = "Add", MainQuest quest = null, QuestNode nQuest = null)
        {

            QuestWork temp = new QuestWork();
            temp.ID = "QuestWork";

            temp._id = id;
            temp.ActionType = actionType;
            temp.mQuest = quest;
            temp.nQuest = nQuest;

            return temp;
        }
        #endregion
    }
}
