using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MapIconAdditionalEvent : IconGameEvent
    {
        string IconObjectID;
        string EventID;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MapIconAdditionalEvent";

            IconObjectID = string.Empty;
            if (node["ID"] != null)
                IconObjectID = node["ID"].Value;

            EventID = string.Empty;
            if (node["EventID"] != null)
                EventID = node["EventID"].Value;

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
            MapLocationIcon icon = null;
            if (IconObjectID.Equals("self"))
                icon = IconObject as MapLocationIcon;
            else
                icon = GM.GetIcon(IconObjectID) as MapLocationIcon;

            if (icon != null)
            {
                icon.AfterZoomPack = EventID;
            }
            else
                Debug.LogError("No MapLocationIcon: " + IconObjectID);

            End();
        }

        #region static
        public static MapIconAdditionalEvent Create(string iconID, string EventID)
        {
            MapIconAdditionalEvent temp = new MapIconAdditionalEvent();
            temp.ID = "MapIconAdditionalEvent";

            temp.IconObjectID = iconID;
            temp.EventID = EventID;

            return temp;
        }
        #endregion
    }
}