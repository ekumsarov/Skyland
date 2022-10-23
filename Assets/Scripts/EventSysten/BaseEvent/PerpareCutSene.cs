using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class PerpareCutSene : GameEvent
    {
        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "PerpareCutSene";

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIM.FastFade();
            End();
        }

        public static PerpareCutSene Create()
        {
            PerpareCutSene temp = new PerpareCutSene();
            temp.ID = "PerpareCutSene";

            return temp;
        }
    }
}