using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class StatMaxAmount : GameEvent
    {
        string statID;
        int val;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "StatMaxAmount";

            this.statID = "no";
            if (node["ID"] != null)
                this.statID = node["ID"].Value;

            this.val = 0;
            if (node["value"] != null)
                this.val = node["value"].AsInt;
        }

        public override bool CanActive()
        {
            if (this.statID.Equals("no"))
                return false;

            return true;
        }

        public override void Start()
        {
            SM.AddMax(this.val, this.statID);
            End();
        }


        #region static
        public static StatMaxAmount Create(string ID, int val = 0)
        {
            StatMaxAmount temp = new StatMaxAmount();
            temp.ID = "StatMaxAmount";
            temp.statID = ID;
            temp.val = val;

            return temp;
        }
        #endregion
    }
}