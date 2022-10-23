using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BattleActions
{
    public class BaseAttack : BattleAction
    {
        GroupSkillCheck CounterCheck;

        public override void PrepareBattleAction()
        {
            this.ID = "BaseAttack";

            this.SuccessCheck = GroupSkillCheck.Create();
            this.AvoidCheck = GroupSkillCheck.Create();
            this.CounterCheck = GroupSkillCheck.Create();


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
                    this.SuccessCheck.AddSkill(SkillCheckObject.Create("strenght", 10, maxResult:3));
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
                    this.AvoidCheck.AddSkill(SkillCheckObject.Create("stamina", 12));
                }

                /////////////////
                arr = null;
                if (info.OtherData["CounterCheck"] != null)
                    arr = info.OtherData["CounterCheck"].AsArray;
                if (arr != null)
                {
                    for (int i = 0; i < arr.Count; i++)
                        this.CounterCheck.AddSkill(SkillCheckObject.Create(arr[i]));
                }
                else
                {
                    this.CounterCheck.AddSkill(SkillCheckObject.Create("dexterity", 20, maxResult:1));
                }
            }
            else
            {
                this.SuccessCheck.AddSkill(SkillCheckObject.Create("strenght", 10, maxResult: 3));
                this.AvoidCheck.AddSkill(SkillCheckObject.Create("stamina", 12));
                this.CounterCheck.AddSkill(SkillCheckObject.Create("dexterity", 20, maxResult: 1));
            }
        }

        public override void Start()
        {
            this.SuccessCheck.CompleteCheck(this.Object.bindUnit);
            this.AvoidCheck.CompleteCheck(this.enemy.bindUnit);

            UIM.BAS.StartCheck(this.SuccessCheck, this.Object.Side, "Sword", Step2);
        }

        public void Step2()
        {
            UIM.BAS.StartCheck(this.AvoidCheck, this.enemy.Side, "Shield", Step3);
        }

        public void Step3()
        {
            if (this.SuccessCheck.FinalResult <= this.AvoidCheck.FinalResult)
            {
                this.CounterCheck.CompleteCheck(this.enemy.bindUnit);
                UIM.BAS.StartCheck(this.CounterCheck, this.enemy.Side, "Sword", CompleteAct);
                return;
            }
            else
                CompleteAct();
        }

        public override void CompleteAct()
        {
            if (this.SuccessCheck.FinalResult > this.AvoidCheck.FinalResult)
            {
                int dam = this.SuccessCheck.FinalResult - this.AvoidCheck.FinalResult;
                this.enemy.TakeDamage(dam);
                this.End();
            }
            else if (this.SuccessCheck.FinalResult <= this.AvoidCheck.FinalResult && this.CounterCheck.FinalResult > 0)
            {
                this.Object.TakeDamage(this.CounterCheck.FinalResult);
                this.End();
            }
            else
                this.End();
                
        }

        public override string GetDescription()
        {
            return LocalizationManager.Get("BaseAttackDes", 1);
        }

        public override string GetActionActiveMessage()
        {
            return LocalizationManager.Get("BaseAttackDesFull", -1, -1, -1, -1);
        }
    }
}