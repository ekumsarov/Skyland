using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;

namespace GameEvents
{
    public class RemoveQuestBut : GameEvent
    {
        string _id;
        string To;
        string Ico;
        string TooltipStr;

        string ActType;
        string Text;
        List<ActionButtonInfo> actions;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "QuestButton";

            this._id = "";
            if (node["ID"] != null)
                this._id = node["ID"].Value;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            //GIM.RemoveQuestButton(this._id);
            UIM.RemoveMenuItem("QuestMenu", this._id);

            End();
        }

        #region static
        public static RemoveQuestBut Create(string _id)
        {
            RemoveQuestBut temp = new RemoveQuestBut();
            temp.ID = "RemoveQuestBut";

            temp._id = _id;

            return temp;
        }
        #endregion
    }
}