using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffectItem : UIItem
{

    [UnityEngine.SerializeField]
    UIImage ico;

    BattleEffects.BattleEffect _battleEffect;

    public override void Setting()
    {
        if (ico == null)
            ico = this.gameObject.GetComponentInChildren<UIImage>();

        ico.Image = "info";
        this.ItemTag = "BattleEffect";

        this.SelectedAction = true;

        base.Setting();
    }

    public void SetEffect(string icon, string action, BattleEffects.BattleEffect _ac)
    {
        this.ico.Image = _ac.Icon;
        this._battleEffect = _ac;
        this.SetTextDelegate(_ac.GetEffectActiveMessage);

    }
}