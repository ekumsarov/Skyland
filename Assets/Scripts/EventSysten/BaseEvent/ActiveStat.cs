using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActiveStat : GameEvent
    {
        string statID;
        bool active;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActiveStat";

            this.statID = "no";
            if (node["ID"] != null)
                this.statID = node["ID"].Value;

            this.active = true;
            if (node["active"] != null)
                this.active = node["active"].AsBool;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (this.statID.Equals("no"))
                return false;

            return true;
        }

        public override void Start()
        {
            SM.ActiveStat(this.statID, this.active);
            End();
        }


        #region static
        public static ActiveStat Create(string ID, bool active = true)
        {
            ActiveStat temp = new ActiveStat();
            temp.ID = "ActiveStat";
            temp.statID = ID;
            temp.active = active;

            return temp;
        }
        #endregion
    }
}
