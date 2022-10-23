using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class SetupIconMainEvent : IconGameEvent
    {
        string IconObjectID;
        string MainEvent;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SetupIconMainEvent";

            IconObjectID = string.Empty;
            if (node["ID"] != null)
                IconObjectID = node["ID"].Value;

            MainEvent = string.Empty;
            if (node["MainEvent"] != null)
                MainEvent = node["MainEvent"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            bool active = base.CanActive();

            if (IconObjectID.IsNullOrEmpty())
                active = false;

            return active;
        }

        public override void Start()
        {
            if (IconObjectID.Equals("self"))
                IconObject.MainEvent = MainEvent;
            else
                GM.GetIcon(IconObjectID).MainEvent = MainEvent;

            End();
        }

        #region static
        public static SetupIconMainEvent Create(string iconID, string mainEvent)
        {
            SetupIconMainEvent temp = new SetupIconMainEvent();
            temp.ID = "CreateIcon";

            temp.IconObjectID = iconID;
            temp.MainEvent = mainEvent;

            return temp;
        }
        #endregion
    }
}