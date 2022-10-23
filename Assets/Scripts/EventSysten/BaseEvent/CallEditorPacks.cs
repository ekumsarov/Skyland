using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CallEditorPacks : GameEvent
    {
        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "InitCanvas";
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            GM.CallEditorPacks();
            End();
        }


        #region static
        public static CallEditorPacks Create()
        {
            CallEditorPacks temp = new CallEditorPacks();
            temp.ID = "CallEditorPacks";

            return temp;
        }
        #endregion
    }
}