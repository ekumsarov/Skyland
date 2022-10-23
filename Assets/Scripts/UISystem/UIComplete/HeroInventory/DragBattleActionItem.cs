using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBattleActionItem : DragAndDropItem
{
    [UnityEngine.SerializeField]

    string ActionType;
    BattleActions.BattleAction _battleAct;
    public BattleActions.BattleAction BattleAction
    {
        get { return this._battleAct; }
    }

    public override void Setting()
    {
        this.Image = "info";
        this.ID = "BattleAction";

        base.Setting();
    }

    public void SetAction(string icon, string action, BattleActions.BattleAction _ac)
    {
        this.Image = icon;
        this.ActionType = action;
        this._battleAct = _ac;
        this.TooltipText = LocalizationManager.Get(_ac.ActName);

    }
}
