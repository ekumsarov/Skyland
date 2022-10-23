using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class CreateMainBuild : GameEvent
    {
        string BuildName;
        int OnIsland;
        bool placed;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "CreateMainBuild";

            this.BuildName = "Random";
            if (node["ID"] != null)
                BuildName = node["ID"].Value;

            placed = true;
            if (node["placed"] != null)
                placed = node["placed"].AsBool;

            OnIsland = -1;
            if (node["OnIsland"] != null)
                OnIsland = node["OnIsland"].AsInt;
        }

        public override bool CanActive()
        {
            if(OnIsland == -1)
            {
                Debug.LogError("Set island number in CreateMainBuild");
                return false;
            }

            return true;
        }

        public override void Start()
        {
            BM.InstallMainBuild(this.OnIsland, "Center", complete: placed);
            BM.Mains[this.OnIsland].SetName(this.BuildName);
            
            End();
        }


        #region static
        public static CreateMainBuild Create(int isl, string _bName = "Random", bool _placed = true)
        {
            CreateMainBuild temp = new CreateMainBuild();
            temp.ID = "CreateMainBuild";

            temp.BuildName = _bName;
            temp.placed = _placed;
            temp.OnIsland = isl;

            return temp;
        }
        #endregion
    }
}