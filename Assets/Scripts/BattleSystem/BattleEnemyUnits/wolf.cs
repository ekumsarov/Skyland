using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;
using BattleActions;

namespace BattleUnitAI
{
    public class wolf : BattleUnitEnemyAI
    {

        public override void MakeAction()
        {
            if (this.availableActions == null)
                this.availableActions = new List<string>();

            if(this.parent.bindUnit == null)
            {
                Debug.LogError("Something wrong in wolf. No bind unit");
                this.CompleteAction();
            }

            this.availableActions.Clear();
            this.availableActions.AddRange(this.parent.bindUnit.actions.Where(actstr => this.parent.bindUnit.EffectCancelAction(actstr) == false).ToList());

            if(this.availableActions.Count == 0)
            {
                this.CompleteAction();
                return;
            }

            string actName;
            if (this.availableActions.Count > 1)
                actName = this.availableActions[UnityEngine.Random.Range(0, this.availableActions.Count - 1)];
            else
                actName = this.availableActions[0];



            int index = 0;
            float maxHP = 0f;
            int maxDef = 0;
            int maxAttack = 0;

            int type = (int)UnityEngine.Random.Range(0, 2);

            if(type == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if(maxHP < UIM.BAS.PlayerArmy[i].CurrentHP)
                    {
                        index = i;
                        maxHP = UIM.BAS.PlayerArmy[i].CurrentHP;
                    }
                }
            }
            else if(type == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if (maxDef < UIM.BAS.PlayerArmy[i].skills["dexterity"].Max)
                    {
                        index = i;
                        maxDef = UIM.BAS.PlayerArmy[i].skills["dexterity"].Max;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if (maxAttack < UIM.BAS.PlayerArmy[i].skills["strenght"].Max)
                    {
                        index = i;
                        maxAttack = UIM.BAS.PlayerArmy[i].skills["strenght"].Max;
                    }
                }
            }

            this._target = UIM.BAS.playerHeroItems[index];

            BattleAction act = BattleAction.loadBattleAction(IOM.BattleActionInfoDic[this.parent.bindUnit.actions[0]].Name,
                this.parent);
            act.PressedItem(this._target);
            UIM.BAS.StartEnemyAction(act);
        }
    }
}