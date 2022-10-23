using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddPack : GameEvent
    {
        string EventsID;
        SkyObject parent;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddPack";

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            To = "";
            if (node["To"] != null)
                To = node["To"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);

        }

        public override bool CanActive()
        {
            parent = GetObject(To);
            if (parent == null)
            {
                Debug.LogError("Event: " + this.ID + ". Cannot find object ID: " + To);
                return false;
            }

            return base.CanActive();
        }

        public override void Start()
        {

            parent.Activity.pushPack(GM.Pack(EventsID));
            End();
        }
    }
}
