using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddEvent : GameEvent
    {
        string EventsID;
        SkyObject parent;
        string To;
        bool activate;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddEvent";

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            activate = false;
            if (node["Activate"] != null)
                activate = node["Activate"].AsBool;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (To.Equals("Manager"))
                return true;

            parent = GetObject(To);

            if (parent == null)
            {
                Debug.LogError("Event: " + this.ID + ". Cannot find object ID: " + To);
                return false;
            }

            return base.CanActive();
        }

        public override void Start()
        {
            if (To.Equals("Manager"))
            {
                GEM.AddEvent(EventsID);
                if(activate)
                    GEM.Execute(EventsID);
            }
            else
            {
                parent.Activity.AddEvent(EventsID);
                if(activate)
                    parent.Activity.CallEvent(EventsID);
            }
                

            End();
        }

        #region static
        public static AddEvent Create(string EventID, string To)
        {
            AddEvent temp = new AddEvent();
            temp.ID = "AddEvent";

            temp.EventsID = EventID;
            temp.To = To;

            return temp;
        }
        #endregion
    }
}