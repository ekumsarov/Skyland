using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CompleteCutScene : GameEvent
    {
        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CompleteCutScene";

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIM.Fade(false, gEvent:this);
            //GM.Camera.GetComponent<FogOfWar>().enabled = true;
        }

        public static CompleteCutScene Create()
        {
            CompleteCutScene temp = new CompleteCutScene();
            temp.ID = "CompleteCutScene";

            return temp;
        }
    }
}