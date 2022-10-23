using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class DamageGroup : GameEvent
    {
        string To;
        string Hero;
        int Amount;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "DamageGroup";

            To = "self";
            if (node["To"] != null)
                To = node["To"].Value;

            Hero = "Group";
            if (node["Hero"] != null)
                Hero = node["Hero"].Value;

            Amount = 0;
            if (node["Amount"] != null)
                Amount = node["Amount"].AsInt;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            SceneObject obj = GetObject(To) as SceneObject;

            obj.Group.Damage(Hero, Amount);

            End();
        }


        #region static
        public static DamageGroup Create(string to, int amount, string hero = "Group")
        {
            DamageGroup temp = new DamageGroup();
            temp.ID = "DamageGroup";

            temp.To = to;
            temp.Hero = hero;
            temp.Amount = amount;

            return temp;
        }
        #endregion
    }
}