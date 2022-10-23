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
    public class WolfTrapEffect : BattleEffect
    {
        string avoidSkill;
        int avoidAmount;

        public override void PrepareBattleEffect()
        {
            this.ID = "WolfTrapEffect";

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
                    this._turns = 4;

                ///////////////////////
                this._desctiprion = data.Description;

                ///////////////////////
                if (data.OtherData != null)
                {
                    if (data.OtherData["SkillCheck"] != null)
                    {
                        this.avoidSkill = data.OtherData["SkillCheck"][0]["id"].Value;
                        this.avoidAmount = data.OtherData["SkillCheck"][0]["complex"].AsInt;
                    }
                }
            }
            else
            {
                this._turns = 4;
                this.avoidSkill = "dexterity";
                this.avoidAmount = 28;
            }

            this._turnsCount = this._turns;
            this.turnOn = true;
        }

        public override void TurnStart()
        {
            if (this.Object.bindUnit.skills.ContainsKey(this.avoidSkill))
            {
                int complex = this.Object.bindUnit.skills[this.avoidSkill].GetValue;
                if (complex < this.avoidAmount)
                {
                    this.Object.AddEffect("WolfTrapCaughtEffect");
                    this.Object.UpdateItem();
                    this.DestroyEffect();
                    return;
                }
            }

        }
    }
}