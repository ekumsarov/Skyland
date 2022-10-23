using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ZoomIsland : GameEvent
    {

        string island;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ZoomIsland";

            island = "self";
            if (node["ID"] != null)
                island = node["ID"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override void Start()
        {
            Island isl = GetObject(island) as Island;

            if(isl == null)
            {
                Debug.LogError("Not found island ID: " + island);
                End();
                return;
            }

            GM.Camera.MoveToPointAsynk(isl.ZoomPoint, End, time: 1.2f);
        }

        #region static
        public static ZoomIsland Create(string isl)
        {
            ZoomIsland temp = new ZoomIsland();
            temp.ID = "ZoomIsland";

            temp.island = isl;

            return temp;
        }
        #endregion
    }
}