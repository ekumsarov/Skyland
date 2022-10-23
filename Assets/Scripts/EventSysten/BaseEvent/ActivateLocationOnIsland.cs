using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ActivateLocationOnIsland : GameEvent
    {
        bool activated;
        int islandNumber;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ActivateLocationOnIsland";

            activated = true;
            if (node["Activate"] != null)
                activated = node["Activate"].AsBool;

            islandNumber = 0;
            if (node["Island"] != null)
                islandNumber = node["Island"].AsInt;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            List<MapLocationObject> list = GM.GetObjectsOnIsland(islandNumber);
            for(int i = 0; i < list.Count; i++)
            {
                list[i].Visible = activated;
            }

            End();
        }

        public static ActivateLocationOnIsland Create(int ID, bool active = true)
        {
            ActivateLocationOnIsland temp = new ActivateLocationOnIsland();

            temp.ID = "ActivateObject";
            temp.islandNumber = ID;
            temp.activated = active;

            return temp;
        }
    }
}