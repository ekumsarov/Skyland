using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CallEvent : GameEvent
    {
        string EventsID;
        JSONNode Node;
        bool FromNode;
        SkyObject parent;
        string To;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CallEvent";
            

            if (node["ID"] != null)
                EventsID = node["ID"].Value;

            FromNode = false;
            Node = node["ID"];
            if (Node != null && Node["Event"] != null)
                FromNode = true;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;
        }

        public override bool CanActive()
        {
            parent = GetObject(To);
            if(parent == null)
            {
                Debug.LogError("Not found object: " + To);
                return false;
            }

            return base.CanActive();
        }

        public override void Start()
        {

            if(FromNode)
                parent.Activity.CallEvent(Node);
            else
                parent.Activity.CallEvent(EventsID);

            End();
        }

        #region static
        public static CallEvent Create(string EventID = null, string Event = null, string to = "self")
        {
            CallEvent temp = new CallEvent();
            temp.ID = "CallEvent";
            temp.To = to;

            if (EventID == null && Event == null)
            {
                temp.To = "justtonotactive";
                return temp;
            }

            if (EventID != null)
            {
                temp.FromNode = false;
                temp.EventsID = EventID;
            }

            if (Event != null)
            {
                temp.FromNode = true;
                temp.Node = JSON.Parse(Event.Replace("'", "\""));
            }

            return temp;
        }
        #endregion
    }
}