using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatItem : UIItem
{
    public static StatItem copyItem;

    public Text text;
    public Image icon;

    public string showValue;
    public bool MainStat;

    public static StatItem Create(string ID, string iconID = "options_icon", string show = "999/999", bool main = false)
    {
        if(StatItem.copyItem == null)
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


    public void SetupStatItem(string statID, string values)
    {
        this.icon.sprite = GuiIconProvider.GetIcon(SM.GetStatIcon(statID));
        this.text.text = values;
    }

    public string Text
    {
        set
        {
            this.text.text = value;
        }
    }

    // How to activate item
    protected override void Show()
    {
        if (_fitscreen)
            Fit();

        if (Resize != UIResize.Fixed)
            Reset();

        this.gameObject.SetActive(true);

        this._parentMenu.Reset();
    }
}