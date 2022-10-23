using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameEvents
{
    public class RemoveBonfire : GameEvent
    {

        int islandID = 0;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "RemoveBonfire";

            islandID = node["ID"].AsInt;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            BM.RemoveBonfire(islandID);

            End();
        }
    }
}
