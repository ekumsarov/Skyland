using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class LootItemUI : UIItem
{
    public UIImage lootIcon;
    public SimpleText lootValue;

    public override void Setting()
    {
        base.Setting();

        this.lootIcon.Visible = true;
        this.lootValue.Visible = false;
        this.lootIcon.Image = "EmptyIcon";
    }

    public void Setup(LootItem loot)
    {
        this.TooltipText = LocalizationManager.Get(loot.ID);

        this.lootIcon.Image = loot.Icon;
        if (loot.Quantity == LS.LootQuantity.Simple)
            this.lootValue.Visible = false;
        else
        {
            this.lootValue.Text = loot.Amount.ToString();
            this.lootValue.Visible = true;
        }
    }
}