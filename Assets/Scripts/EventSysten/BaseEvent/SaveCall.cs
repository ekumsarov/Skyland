using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class SaveCall : GameEvent
    {

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SaveCall";
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {

            SaveManager.SaveFile();
            End();
        }
    }
}
