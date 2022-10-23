using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class PlayerInit : GameEvent
    {
        bool City;
        string CityIsland;
        string HeroIsland;
        int Settlements;

        Dictionary<string, int> builds;

        Vector3 target;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "PlayerInit";

            this.City = true;
            if (node["City"] != null)
                this.City = node["City"].AsBool;

            this.CityIsland = "random";
            if (node["CityIsland"] != null)
                this.CityIsland = node["CityIsland"].Value;

            this.HeroIsland = "random";
            if (node["HeroIsland"] != null)
                this.HeroIsland = node["HeroIsland"].Value;

            this.Settlements = 0;
            if (node["Settlements"] != null)
                this.Settlements = node["Settlements"].AsInt;

            this.builds = new Dictionary<string, int>();
            if (node["Builds"] != null)
            {
                JSONNode nod = node["Builds"];
                foreach(var key in nod.Keys)
                {
                    this.builds.Add(key, nod[key].AsInt);
                }
            }

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            //GM.MainShip.HardSet();
            //GM.MainShip.Process(UnitState.s_Activation);
            GM.Player.HardSet();
            
            End();
        }
    }
}