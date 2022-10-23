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
    public class TrollHero : BattleUnitEnemyAI
    {

        public override void MakeAction()
        {
            if (this.availableActions == null)
                this.availableActions = new List<string>();

            if (this.parent.bindUnit == null)
            {
                Debug.LogError("Something wrong in StrangeAnimal1. No bind unit");
                this.CompleteAction();
            }

            this.availableActions.Clear();
            this.availableActions.AddRange(this.parent.bindUnit.actions.Where(actstr => this.parent.bindUnit.EffectCancelAction(actstr) == false).ToList());

            if (this.availableActions.Count == 0)
            {
                this.CompleteAction();
                return;
            }

            List<HeroInfoItem> avaliableEnemies = UIM.BAS.playerHeroItems.Where(uni => uni.Visible && uni.bindUnit.CurrentHP > 0).ToList();
            if(avaliableEnemies.Count == 0)
            {
                CompleteAction();
                return;
            }
            BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["TrollAttack"].Name,
                this.parent);
            this._target = avaliableEnemies[UnityEngine.Random.Range(0, avaliableEnemies.Count)];
            actH.PressedItem(this._target);
            UIM.BAS.StartEnemyAction(actH);
        }
    }
}