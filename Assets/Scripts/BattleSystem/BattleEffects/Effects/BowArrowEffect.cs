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
    public class BowArrowEffect : BattleEffect
    {
        string toRemoveSkill;
        int toRemoveAmount;

        public override void PrepareBattleEffect()
        {
            this.ID = "BowArrowEffect";

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
                    this._turns = 2;

                ///////////////////////
                this._desctiprion = data.Description;

                ///////////////////////
                if (data.OtherData != null)
                {
                    if (data.OtherData["SkillCheck"] != null)
                    {
                        if (data.OtherData["SkillCheck"][0]["id"] != null)
                            this.toRemoveSkill = data.OtherData["SkillCheck"][0]["id"].Value;
                        else
                            this.toRemoveSkill = "dexterity";

                        if (data.OtherData["SkillCheck"][0]["complex"] != null)
                            this.toRemoveAmount = data.OtherData["SkillCheck"][0]["complex"].AsInt;
                        else
                            this.toRemoveAmount = 4;
                    }
                }
            }
            else
            {
                this._turns = 2;
                this.toRemoveSkill = "dexterity";
                this.toRemoveAmount = 4;
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
            base.DestroyEffect();
        }
    }
}