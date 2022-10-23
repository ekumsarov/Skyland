using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : UIItem
{
    public SimpleText text;
    public UIImage icon;

    public void SetupItem(RewardItemInfo info)
    {
        if (info.HasValue)
        {
            this.text.Text = info.Value;
            this.text.Visible = true;
        }
        else
            this.text.Visible = false;

        this.icon.Image = info.Icon;
    }

    public static RewardItem copyItem;

    public static RewardItem Create(string ID)
    {
        if (RewardItem.copyItem == null)
            RewardItem.copyItem = GameObject.Instantiate(Resources.Load<RewardItem>("Prefabs/UIeX/Complete/RewardItem"));

        RewardItem temp = GameObject.Instantiate(RewardItem.copyItem);

        temp.ID = ID;
        temp.name = ID;
        temp.ItemTag = "RewardItem";

        return temp;
    }

    public static void CreateRewardItem(string ID)
    {
        if (RewardItem.copyItem == null)
            RewardItem.copyItem = GameObject.Instantiate(Resources.Load<RewardItem>("Prefabs/UIeX/Complete/RewardItem"));

        RewardItem temp = GameObject.Instantiate(RewardItem.copyItem);

        temp.ID = ID;
        temp.name = ID;
        temp.ItemTag = "RewardItem";

        UIM.AddMenuItem("RewardMenu", temp);
    }
}
