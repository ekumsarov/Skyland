using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class EnableUIItem : GameEvent
    {
        bool enable;
        string _id;
        string MenuID;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "EnableUIItem";

            this._id = "All";
            if (node["ID"] != null)
                this._id = node["ID"].Value;

            this.enable = true;
            if (node["Enable"] != null)
                this.enable = node["Enable"].AsBool;

            MenuID = "";
            if (node["MenuID"] != null)
                MenuID = node["MenuID"].Value;

        }

        public override void Start()
        {
            if(!this.MenuID.Equals(""))
                UIM.EnableItem(this.enable, this.MenuID, this._id);

            End();
        }
    }
}