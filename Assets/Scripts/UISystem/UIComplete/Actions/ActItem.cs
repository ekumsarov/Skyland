using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActItem : UIItem
{
    public Image icon;

    public string showValue;
    public bool MainStat;

    public static StatItem Create(string ID, string iconID = "options_icon", string show = "999/999", bool main = false)
    {
        if (StatItem.copyItem == null)
            StatItem.copyItem = GameObject.Instantiate(Resources.Load<StatItem>("Prefabs/UIeX/Complete/StatItem"));

        StatItem temp = GameObject.Instantiate(StatItem.copyItem);

        temp.ID = ID;
        temp.name = ID;
        temp.showValue = show;
        temp.icon.sprite = GuiIconProvider.GetIcon(iconID);
        temp.text.text = temp.showValue;
        temp.MainStat = main;

        UIM.AddMenuItem("StatMenu", temp);

        return temp;
    }
}
