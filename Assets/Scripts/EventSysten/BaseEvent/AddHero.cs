using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AddHero : GameEvent
    {
        string HeroID;
        string To;
        string WorkType;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AddHero";

            this.HeroID = string.Empty;
            if (node["HeroID"] != null)
                HeroID = node["HeroID"].Value;

            To = string.Empty;
            if (node["To"] != null)
                To = node["To"].Value;

            WorkType = "Add";
            if (node["WorkType"] != null)
                WorkType = node["WorkType"].Value;

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);
        }

        public override bool CanActive()
        {
            if (HeroID.IsNullOrEmpty() || To.IsNullOrEmpty())
            {
                Debug.LogError("Event: " + this.ID + ". Cannot find object ID: " + To);
                return false;
            }

            return base.CanActive();
        }

        public override void Start()
        {
            SkyObject temp = GetObject(To);

            if (temp != null)
            {
                if (WorkType.Equals("Remove"))
                    temp.Group.RemoveHero(HeroID);
                else
                    temp.Group.AddNewHero(HeroID);
            }
                
            else
                Debug.LogError("NotFoundParent");

            End();
        }
    }
}