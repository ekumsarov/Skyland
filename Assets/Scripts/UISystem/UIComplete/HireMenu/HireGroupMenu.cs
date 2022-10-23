using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lodkod;
using System;
using System.Linq;
using GameEvents;
using SimpleJSON;
using BattleActions;

public class HireGroupMenu : MenuEx
{

    #region base parameters

    [SerializeField] UIIconText TroopInfo;
    [SerializeField] List<SkillItem> skills;
    [SerializeField] List<StatItem> cost;
    [SerializeField] List<StatItem> totalCost;
    [SerializeField] List<UnitInfoItem> groups;
    [SerializeField] List<InfoButton> actions;
    [SerializeField] List<InfoButton> troops;
    [SerializeField] SimpleText TroopAmount;

    private int activeHeroIndex = 0;
    private int activeUnitIndex = 0;
    #endregion

    public override void Setting()
    {
        base.Setting();

        this.troops = new List<InfoButton>();
        foreach(var item in this._allItems.Where(troop => troop.Value.ItemTag.Equals("TroopItem")))
        {
            this.troops.Add(item.Value as InfoButton);
        }
    }

    public override void Open()
    {


        this._allItems["HeroGroups"].Visible = true;
        this._allItems["PlayerActions"].Visible = true;
        this._allItems["HeroInventory"].Visible = true;
        this._allItems["ActionChoices"].Visible = false;
        this._allItems["Inventory"].Visible = false;

        base.Open();
    }

    public override void PressedItem(UIItem data)
    {
        
    }
}
