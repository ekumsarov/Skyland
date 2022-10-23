using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using BattleActions;

namespace BattleActions1
{
    public class TrollAttack : BattleAction
    {
        public override void PrepareBattleAction()
        {
            this.ID = "TrollAttack";

            this.SuccessCheck = GroupSkillCheck.Create();
            this.AvoidCheck = GroupSkillCheck.Create();


            if (IOM.BattleActionInfoDic.ContainsKey(this.ID))
            {
                BattleActionInfo info = IOM.BattleActionInfoDic[this.ID];

                this._ico = info.Icon;
                this._desctiprion = info.Description;
                this._name = info.Name;


                /////////////////
                JSONArray arr = null;
                if (info.OtherData["SkillCheck"] != null)
                    arr = info.OtherData["SkillCheck"].AsArray;
                if (arr != null)
                {
                    for (int i = 0; i < arr.Count; i++)
                        SuccessCheck.AddSkill(SkillCheckObject.Create(arr[i]));
                }
                else
                {
                    this.SuccessCheck.AddSkill(SkillCheckObject.Create("strenght", 6));
                }

                /////////////////
                arr = null;
                if (info.OtherData["AvoidCheck"] != null)
                    arr = info.OtherData["AvoidCheck"].AsArray;
                if (arr != null)
                {
                    for (int i = 0; i < arr.Count; i++)
                        AvoidCheck.AddSkill(SkillCheckObject.Create(arr[i]));
                }
                else
                {
                    this.AvoidCheck.AddSkill(SkillCheckObject.Create("dexterity", 7));
                }
            }
            else
            {
                this.SuccessCheck.AddSkill(SkillCheckObject.Create("strenght", 6));
                this.AvoidCheck.AddSkill(SkillCheckObject.Create("stamina", 7));
            }
        }

        public override void Start()
        {
            this.SuccessCheck.CompleteCheck(this.Object.bindUnit);
            this.AvoidCheck.CompleteCheck(this.enemy.bindUnit);

            UIM.BAS.StartCheck(this.SuccessCheck, this.Object.Side, "icon_bow", Step2);
        }

        public void Step2()
        {
            UIM.BAS.StartCheck(this.AvoidCheck, this.enemy.Side, "Shield", CompleteAct);
        }

        public override void CompleteAct()
        {
            if (this.SuccessCheck.FinalResult > this.AvoidCheck.FinalResult)
            {
                int dam = this.SuccessCheck.FinalResult - this.AvoidCheck.FinalResult;
                this.enemy.TakeDamage(dam);
                this.End();
            }
            else
                this.End();

        }

        public override string GetDescription()
        {
            return LocalizationManager.Get("AnimalAttackDes", 1, -1, -1);
        }

        public override string GetActionActiveMessage()
        {
            return LocalizationManager.Get("AnimalAttackDes", 1, -1, -1);
        }
    }
}