using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MakePack : GameEvent
    {
        string EventsID;
        SkyObject parent;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MakePack";

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            To = "";
            if (node["To"] != null)
                To = node["To"].Value;

        }

        public override bool CanActive()
        {
            parent = GetObject(To);
            if (parent == null)
            {
                Debug.LogError("Event: " + this.ID + ". Cannot find object ID: " + To);
                return false;
            }

            return true;
        }

        public override void Start()
        {

            parent.Activity.pushPack(GM.Pack(EventsID));
            End();
        }
    }
}