using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class SetupStat : GameEvent
    {
        string statID;
        int val;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SetupStat";

            this.statID = "no";
            if (node["ID"] != null)
                this.statID = node["ID"].Value;

            this.val = 0;
            if (node["value"] != null)
                this.val = node["value"].AsInt;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (this.statID.Equals("no"))
                return false;

            return base.CanActive();
        }

        public override void Start()
        {
            SM.SetupStat(this.val, this.statID);
            End();
        }


        #region static
        public static SetupStat Create(string ID, int val = 0)
        {
            SetupStat temp = new SetupStat();
            temp.ID = "SetupStat";
            temp.statID = ID;
            temp.val = val;

            return temp;
        }
        #endregion
    }
}