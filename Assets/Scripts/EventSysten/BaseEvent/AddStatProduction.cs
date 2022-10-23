using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddStatProduction : GameEvent
    {
        string statID;
        string sourceID;
        float val;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddStatProduction";

            this.statID = "no";
            if (node["ID"] != null)
                this.statID = node["ID"].Value;

            this.val = 0;
            if (node["value"] != null)
                this.val = node["value"].AsFloat;

            this.sourceID = "baseProduct";
            if (node["Source"] != null)
                this.sourceID = node["Source"].Value;

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
            SM.AddProduct(this.statID, this.val, this.sourceID);
            End();
        }


        #region static
        public static AddStatProduction Create(string ID, int val = 0)
        {
            AddStatProduction temp = new AddStatProduction();
            temp.ID = "AddStatProduction";
            temp.statID = ID;
            temp.val = val;

            return temp;
        }
        #endregion
    }
}