using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class MapIslandIcon : GameEvent
    {
        string MapIsland;
        string Icon;
        string IconID;
        string ActType;


        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "MapIslandIcon";

            MapIsland = "";
            if (node["ID"] != null)
                MapIsland = node["ID"].Value;

            IconID = "";
            if (node["IconID"] != null)
                IconID = node["IconID"].Value;

            Icon = "";
            if (node["Icon"] != null)
                IconID = node["Icon"].Value;

            ActType = "Add";
            if (node["Action"] != null)
                ActType = node["Action"].Value;

        }

        public override bool CanActive()
        {
            if (IconID.Equals("") || MapIsland.Equals(""))
                return false;

            return true;
        }

        public override void Start()
        {
            Island isl = GetObject(MapIsland) as Island;


            if (ActType.Equals("Add"))
                isl.AddQuestIcon(IconID, Icon);
            else
                isl.RemoveQuestIcon(IconID);

            End();
        }

        #region static
        public static MapIslandIcon Create(string MapIsland, string IconID, string Icon = null, string ActType = "Add")
        {
            MapIslandIcon temp = new MapIslandIcon();
            temp.ID = "MapIslandIcon";

            temp.MapIsland = MapIsland;
            temp.IconID = IconID;
            temp.Icon = Icon;
            temp.ActType = ActType;

            return temp;
        }
        #endregion
    }
}