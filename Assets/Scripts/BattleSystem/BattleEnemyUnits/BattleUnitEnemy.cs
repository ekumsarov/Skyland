using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using BattleUnitAI;

public class BattleUnitEnemy : HeroInfoItem
{
    public override void SetupItem(HeroUnit uni)
    {
        base.SetupItem(uni);
        this._side = 1;
        this.UnitDecision = BattleUnitEnemyAI.loadBattleAction(uni.Name);
        this.UnitDecision.parent = this;
    }

    #region Unit enemy logic
    public BattleUnitEnemyAI UnitDecision;

    public void StartTurn()
    {
        this.bindUnit.EffectTurnStart();
        this.MakeAction();
    }

    public void MakeAction()
    {
        UnitDecision.MakeAction();
    }
    

    #endregion
}