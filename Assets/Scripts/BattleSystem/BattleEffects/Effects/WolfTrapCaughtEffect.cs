using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;

namespace BattleEffects
{
    public class WolfTrapCaughtEffect : BattleEffect
    {
        string toRemoveSkill;
        int toRemoveAmount;

        public override void PrepareBattleEffect()
        {
            this.ID = "WolfTrapCaughtEffect";

            if (!this.initialized)
                this.Init();

            if (data != null)
            {
                if (data.Icon != null)
                    this._ico = data.Icon;
                else
                    this._ico = "info";

                ///////////////////////
                if (data.Turns != -100)
                    this._turns = data.Turns;
                else
                    this._turns = 3;

                ///////////////////////
                this._desctiprion = data.Description;

                ///////////////////////
                if (data.OtherData != null)
                {
                    if (data.OtherData["Influence"] != null)
                    {
                        if (data.OtherData["Influence"]["Skill"] != null)
                            this.toRemoveSkill = data.OtherData["Influence"]["Skill"].Value;
                        else
                            this.toRemoveSkill = "dexterity";

                        if (data.OtherData["Influence"]["Amount"] != null)
                            this.toRemoveAmount = data.OtherData["Influence"]["Amount"].AsInt;
                        else
                            this.toRemoveAmount = 10;
                    }
                }
            }
            else
            {
                this._turns = 3;
                this.toRemoveSkill = "dexterity";
                this.toRemoveAmount = 10;
            }

            this._turnsCount = this._turns;
            this.turnOn = true;
        }

        public override void Start()
        {
            this.Object.bindUnit.AddSkillEffect(this.ID, this.toRemoveSkill, this.toRemoveAmount, this.toRemoveAmount);
            this.Object.UpdateItem();
        }

        public override void DestroyEffect()
        {
            this.Object.bindUnit.RemoveSkillEffect(this.ID);
            this.Object.UpdateItem();
            base.DestroyEffect();
        }
    }
}