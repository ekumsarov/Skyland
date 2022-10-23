using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;
using BattleEffects;

namespace BattleEffects1
{
    public class AnimalPoisonEffect : BattleEffect
    {

        public override void PrepareBattleEffect()
        {
            this.ID = "AnimalPoisonEffect";

            if (!this.initialized)
                this.Init();

            if (data != null)
            {
                if (data.Icon != null)
                    this._ico = data.Icon;
                else
                    this._ico = "BaseIcon";

                ///////////////////////
                if (data.Turns != -100)
                    this._turns = data.Turns;
                else
                    this._turns = 2;

                ///////////////////////
                this._desctiprion = data.Description;
            }
            else
            {
                this._turns = 2;
            }

            this._turnsCount = this._turns;
            this.turnOn = true;
        }

        public override void TurnStart()
        {
            this.Object.TakeDamage(1);
        }
    }
}