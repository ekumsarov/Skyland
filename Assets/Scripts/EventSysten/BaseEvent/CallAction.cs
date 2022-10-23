using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CallAction : GameEvent
    {
        string ActionID;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CallAction";

            this.To = "self";
            if (node["To"] != null)
                this.To = node["To"].Value;

            if (node["ID"] != null)
                ActionID = node["ID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }


        public override void Start()
        {
            SkyObject temp = GetObject(To);

            if (temp != null)
                temp.CallAction(ActionID);
            else
                Object.CallAction(ActionID);

            End();
        }


        #region static
        public static CallAction Create(string ActionID, string to = "self")
        {
            CallAction temp = new CallAction();
            temp.ID = "CallAction";

            temp.ActionID = ActionID;
            temp.To = to;

            return temp;
        }
        #endregion
    }
}