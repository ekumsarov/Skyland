using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class LootWork : GameEvent
    {
        string WorkType;

        string To;

        string LootID;
        LS.LootType type;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "LootWork";

            WorkType = "Add";
            if (node["WorkType"] != null)
                WorkType = node["WorkType"].Value;

            LootID = null;
            if (node["LootID"] != null)
                LootID = node["LootID"].Value;

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            type = LS.LootType.Weapon;
            if (node["type"] != null)
                type = (LS.LootType)Enum.Parse(typeof(LS.LootType), node["type"].Value);

            if (node["Conditions"] != null)
                MakeConditions(node["Conditions"].AsArray);

        }

        public override void Start()
        {
            LS.AddItem(LootID, type, To);

            End();
        }

        #region static
        public static LootWork Create(string LootID, LS.LootType type, string To)
        {
            LootWork temp = new LootWork();
            temp.ID = "LootWork";

            temp.To = To;
            temp.LootID = LootID;
            temp.type = type;

            return temp;
        }
        #endregion
    }
}