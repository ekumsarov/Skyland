using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class InitMap : GameEvent
    {

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "InitMap";
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIM.CallFunc("MapMenu", "InitMap");
            //GIM.MapMenu.InitMap();
            End();
        }
    }
}
