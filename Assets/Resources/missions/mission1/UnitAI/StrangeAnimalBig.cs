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
    public class StrangeAnimalBig : BattleUnitEnemyAI
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

            List<HeroInfoItem> avaliableEnemies = UIM.BAS.playerHeroItems.Where(uni => uni.Visible).ToList();

            if (this.parent.bindUnit.CurrentHP <= 3 && this.parent.bindUnit.CurrentHP > 1 && UnityEngine.Random.Range(0, 101) > 60 && this.availableActions.Any(actt => actt.Equals("AnimalHeal")))
            {
                BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["AnimalHeal"].Name,
                this.parent);
                this._target = this.parent;
                actH.PressedItem(this._target);
                UIM.BAS.StartEnemyAction(actH);
                return;
            }
            else if (this.parent.bindUnit.CurrentHP <= 1 && UnityEngine.Random.Range(0, 101) > 60 && this.availableActions.Any(actt => actt.Equals("AnimalRetreat")))
            {
                BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["AnimalRetreat"].Name,
                this.parent);
                this._target = this.parent;
                actH.PressedItem(this._target);
                UIM.BAS.StartEnemyAction(actH);
                return;
            }
            else
            {
                bool hasEffect = avaliableEnemies.Any(uni => uni.bindUnit.HasEffect("AnimalPoisonEffect") == false);
                if (this.availableActions.Any(actc => actc.Equals("AnimalPoison")) && hasEffect && UnityEngine.Random.Range(0, 101) > 75)
                {
                    BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["AnimalPoison"].Name,
                        this.parent);
                    this._target = avaliableEnemies.First(uni => uni.bindUnit.HasEffect("AnimalPoisonEffect") == false);
                    actH.PressedItem(this._target);
                    UIM.BAS.StartEnemyAction(actH);
                    return;
                }
                else
                {
                    if (UnityEngine.Random.Range(0, 101) > 60 && this.availableActions.Any(actt => actt.Equals("AnimalBow")))
                    {
                        BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["AnimalBow"].Name,
                        this.parent);
                        this._target = avaliableEnemies[UnityEngine.Random.Range(0, avaliableEnemies.Count)];
                        actH.PressedItem(this._target);
                        UIM.BAS.StartEnemyAction(actH);
                        return;
                    }
                    else
                    {
                        if (this.availableActions.Any(actt => actt.Equals("AnimalAttack")))
                        {
                            BattleAction actH = BattleAction.loadBattleAction(IOM.BattleActionInfoDic["AnimalAttack"].Name,
                                this.parent);
                            this._target = avaliableEnemies[UnityEngine.Random.Range(0, avaliableEnemies.Count)];
                            actH.PressedItem(this._target);
                            UIM.BAS.StartEnemyAction(actH);
                            return;
                        }
                        else
                        {
                            this.CompleteAction();
                        }
                    }
                }
            }
        }
    }
}