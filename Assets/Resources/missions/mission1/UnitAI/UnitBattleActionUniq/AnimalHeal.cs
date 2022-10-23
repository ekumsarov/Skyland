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
    public class AnimalHeal : BattleAction
    {
        public override void PrepareBattleAction()
        {
            this.ID = "AnimalHeal";

            this.SuccessCheck = GroupSkillCheck.Create();


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
                    this.SuccessCheck.AddSkill(SkillCheckObject.Create("intelligence", 4));
                }
            }
            else
            {
                this.SuccessCheck.AddSkill(SkillCheckObject.Create("intelligence", 4));
            }
        }

        public override void Start()
        {
            this.SuccessCheck.CompleteCheck(this.Object.bindUnit);
            UIM.BAS.StartCheck(this.SuccessCheck, this.Object.Side, "BloodIcon", CompleteAct);
        }

        public override void CompleteAct()
        {
            if (this.SuccessCheck.FinalResult > 0)
            {
                int dam = this.SuccessCheck.FinalResult;
                this.Object.HealUnit(dam);
                this.End();
            }
            else
                this.End();

        }

        public override string GetDescription()
        {
            return LocalizationManager.Get("NotReady");
        }

        public override string GetActionActiveMessage()
        {
            return LocalizationManager.Get("NotReady");
        }
    }
}