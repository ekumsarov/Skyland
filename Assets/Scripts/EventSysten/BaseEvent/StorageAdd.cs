using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class StorageAdd : GameEvent
    {
        JSONArray resArray;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "StorageAdd";

            this.resArray = null;
            if (node["AddList"] != null)
                this.resArray = node["AddList"].AsArray;
        }

        public override bool CanActive()
        {
            if (this.resArray == null)
            {
                Debug.LogError("Cannot start: " + this.ID);
                return false;
            }

            return true;
        }

        public override void Start()
        {
            for (int i = 0; i < this.resArray.Count; i++)
                SM.AddStat(this.resArray[i]["count"].AsInt, this.resArray[i]["id"].Value);

            End();
        }
    }
}