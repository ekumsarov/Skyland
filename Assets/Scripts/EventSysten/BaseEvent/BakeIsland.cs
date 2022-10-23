using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class BakeIsland : GameEvent
    {
        string EventsID;
        SkyObject parent;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "BakeIsland";

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            To = "";
            if (node["To"] != null)
                To = node["To"].Value;

        }

        public override bool CanActive()
        {

            return true;
        }

        public override void Start()
        {

            //IM.BakeIslands();
            End();
        }
    }
}