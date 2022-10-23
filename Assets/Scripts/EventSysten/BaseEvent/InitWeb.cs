using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class InitWeb : GameEvent
    {

        int islNum = -1;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "InitWeb";

            this.islNum = -1;
            if (node["ID"] != null)
                this.islNum = node["ID"].AsInt;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {

            IM.InitWeb(this.islNum);
            End();
        }
    }
}
