using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddBattleAction : GameEvent
    {
        string ActID;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddBattleAction";

            if (node["ID"] != null)
                ActID = node["ActID"].Value;

            To = "";
            if (node["To"] != null)
                To = node["To"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            GM.Player.Group.AddAction(ActID, To);

            End();
        }

        #region static
        public static AddBattleAction Create(string actID, string To)
        {
            AddBattleAction temp = new AddBattleAction();
            temp.ID = "AddBattleAction";

            temp.ActID = actID;
            temp.To = To;

            return temp;
        }
        #endregion
    }
}