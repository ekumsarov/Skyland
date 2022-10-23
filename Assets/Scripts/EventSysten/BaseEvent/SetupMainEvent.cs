using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class SetupMainEvent : IconGameEvent
    {
        string ObjectID;
        string MainEvent;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "SetupMainEvent";

            ObjectID = string.Empty;
            if (node["ID"] != null)
                ObjectID = node["ID"].Value;

            MainEvent = string.Empty;
            if (node["MainEvent"] != null)
                MainEvent = node["MainEvent"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            bool active = base.CanActive();

            if (ObjectID.IsNullOrEmpty())
                active = false;

            return active;
        }

        public override void Start()
        {
            if (ObjectID.Equals("self"))
                IconObject.MainEvent = MainEvent;
            else
                GetObject(ObjectID).MainEvent = MainEvent;

            End();
        }

        #region static
        public static SetupMainEvent Create(string objectID, string mainEvent)
        {
            SetupMainEvent temp = new SetupMainEvent();
            temp.ID = "SetupMainEvent";

            temp.ObjectID = objectID;
            temp.MainEvent = mainEvent;

            return temp;
        }
        #endregion
    }
}