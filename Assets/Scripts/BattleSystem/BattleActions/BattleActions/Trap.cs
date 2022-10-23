using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BattleActions
{
    public class Trap : BattleAction
    {
        public override void PrepareBattleAction()
        {
            this.ID = "Trap";

            this.SuccessCheck = GroupSkillCheck.Create();
            this.AvoidCheck = null;

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
                    this.SuccessCheck.AddSkill(SkillCheckObject.Create("dexterity", 12, maxResult:1));
                    this.SuccessCheck.AddSkill(SkillCheckObject.Create("intelligence", 12, maxResult: 1));
                }
            }
            else
            {
                this.SuccessCheck.AddSkill(SkillCheckObject.Create("dexterity", 12, maxResult: 1));
                this.SuccessCheck.AddSkill(SkillCheckObject.Create("intelligence", 12, maxResult: 1));
            }
        }

        public override void Start()
        {
            this.SuccessCheck.CompleteCheck(this.Object.bindUnit);

            UIM.BAS.StartCheck(this.SuccessCheck, this.Object.Side, "SuccessIcon", CompleteAct);
        }

        public override void CompleteAct()
        {
            if(this.SuccessCheck.FinalResult <= 0)
            {
                this.End();
                return;
            }

            this.enemy.AddEffect("WolfTrapEffect");
            this.Object.bindUnit.RemoveAction("Trap");
            this.End();
        }

        public override string GetDescription()
        {
            return LocalizationManager.Get("TrapDes");
        }

        public override string GetActionActiveMessage()
        {
            return LocalizationManager.Get("TrapDesFull");
        }
    }
}