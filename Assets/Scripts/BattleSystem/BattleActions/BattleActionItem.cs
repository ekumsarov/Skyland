using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActionItem : UIItem
{

    [UnityEngine.SerializeField]
    UIImage ico;

    string ActionType;
    BattleActions.BattleAction _battleAct;
    public BattleActions.BattleAction BattleAction
    {
        get { return this._battleAct; }
    }

    public override void Setting()
    {
        if (ico == null)
            ico = this.gameObject.GetComponentInChildren<UIImage>();

        ico.Image = "info";
        this.ItemTag = "BattleAction";

        this.SelectedAction = true;

        base.Setting();
    }

    public void SetAction(string icon, string action, BattleActions.BattleAction _ac)
    {
        this.ico.Image = icon;
        this.ActionType = action;
        this.Info = MyString.MakeJSON("{'Action':'" + action + "'}");
        this._battleAct = _ac;
        this.TooltipText = LocalizationManager.Get(_ac.ActName);

    }
}
