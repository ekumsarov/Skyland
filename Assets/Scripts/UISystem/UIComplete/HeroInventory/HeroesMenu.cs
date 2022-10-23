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

public class HeroesMenu : MenuEx
{

    #region base parameters

    [SerializeField] List<SkillItem> skills;
    [SerializeField] List<DragAndDropCell> EquipedActions;
    [SerializeField] List<DragAndDropCell> actions;
    [SerializeField] List<UnitInfoItem> groups;
    [SerializeField] List<DragAndDropCell> loots;
    [SerializeField] DragAndDropCell Weapon1;
    [SerializeField] DragAndDropCell Weapon2;
    [SerializeField] DragAndDropCell Amulet;
    [SerializeField] DragAndDropCell Armor;
    [SerializeField] DragAndDropCell Extra;

    private int activeHeroIndex = 0;
    #endregion

    public override void Setting()
    {
        base.Setting();

        actions = new List<DragAndDropCell>();
        foreach(var lootItem in this._allItems.Where(item => item.Value.ItemTag.Equals("InventoryActionItem")))
        {
            if (actions.Count < 40)
            {
                DragAndDropCell cell = lootItem.Value as DragAndDropCell;
                cell.NotifyDelegate = OnItemPlace;
                actions.Add(cell);
            }
                
        }

        foreach (var lootItem in this._allItems.Where(item => item.Value.ItemTag.Equals("HeroLoot")))
        {
            if (loots.Count < 40)
            {
                DragAndDropCell cell = lootItem.Value as DragAndDropCell;
                cell.NotifyDelegate = OnItemPlace;
                loots.Add(cell);
            }

        }

        Weapon1.AddAllowedItems(LS.LootType.Weapon.ToString());
        Weapon1.NotifyDelegate = OnItemPlace;
        Weapon2.AddAllowedItems(LS.LootType.SecondWeapon.ToString());
        Weapon2.NotifyDelegate = OnItemPlace;
        Amulet.AddAllowedItems(LS.LootType.Amulet.ToString());
        Amulet.NotifyDelegate = OnItemPlace;
        Armor.AddAllowedItems(LS.LootType.Armor.ToString());
        Armor.NotifyDelegate = OnItemPlace;
        Extra.AddAllowedItems(LS.LootType.Extra.ToString());
        Extra.NotifyDelegate = OnItemPlace;

        for (int i = 0; i < this.EquipedActions.Count; i++)
        {
            this.EquipedActions[i].NotifyDelegate = OnItemPlace;
            this.EquipedActions[i].AddAllowedItems("ActionItem");
        }
    }

    public override void Open()
    {
        for(int i = 0; i < 3; i++)
        {
            if (i < GM.PlayerIcon.Group.GetUnits().Count)
            {
                this._allImages["HeroIcon" + (i + 1)].Visible = true;
                this._allImages["HeroIcon" + (i + 1)].Image = GM.PlayerIcon.Group.GetUnits()[i].Icon;
            }
            else
                this._allImages["HeroIcon" + (i + 1)].Visible = false;
        }

        activeHeroIndex = 0;

        int index = 0;
        foreach (var skill in GM.PlayerIcon.Group.GetUnits()[0].skills)
        {
            skills[index].Setup(skill.Value);
            index++;
            if (index >= 5)
                break;
        }

        index = 0;
        foreach (var unit in GM.PlayerIcon.Group.GetUnits()[0].units)
        {
            groups[index].unitIcon.Image = unit.Key.Icon;
            groups[index].unitAmount.Text = unit.Value.ToString();

            index++;
            if (index >= 5)
                break;
        }

        for(int i = 0; i < 8; i++)
        {
            this.EquipedActions[i].RemoveItem();
            if (i < GM.PlayerIcon.Group.GetUnits()[0].EquipedActions.Count)
            {
                string act = GM.PlayerIcon.Group.GetUnits()[0].EquipedActions[i];
                this.EquipedActions[i].PlaceItem(IOM.BattleActionInfoDic[act].Icon, act, "ActionItem");
            }

        }

        for(int i = 0; i < 40; i++)
        {
            this.actions[i].RemoveItem();
            if (i < GM.PlayerIcon.Group.GetUnits()[0].actions.Count)
            {
                string act = GM.PlayerIcon.Group.GetUnits()[0].actions[i];
                this.actions[i].PlaceItem(IOM.BattleActionInfoDic[act].Icon, act, "ActionItem");
            }
        }

        LootItem item = LS.GetHeroItem(LS.LootType.Weapon, "Player");
        if(item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.Weapon.ToString());
        }

        item = LS.GetHeroItem(LS.LootType.SecondWeapon, "Player");
        if (item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.SecondWeapon.ToString());
        }

        item = LS.GetHeroItem(LS.LootType.Amulet, "Player");
        if (item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.Amulet.ToString());
        }

        item = LS.GetHeroItem(LS.LootType.Armor, "Player");
        if (item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.Armor.ToString());
        }

        item = LS.GetHeroItem(LS.LootType.Extra, "Player");
        if (item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.Extra.ToString());
        }

        for (int i = 0; i < 40; i++)
        {
            this.loots[i].RemoveItem();
            if (i < LS.Bag.Values.Count)
            {
                this.loots[i].PlaceItem(LS.Bag.ElementAt(i).Value.Icon, LS.Bag.ElementAt(i).Value.ID, LS.Bag.ElementAt(i).Value.type.ToString());
            }
        }

        this._allItems["HeroGroups"].Visible = true;
        this._allItems["PlayerActions"].Visible = true;
        this._allItems["HeroInventory"].Visible = true;
        this._allItems["ActionChoices"].Visible = false;
        this._allItems["Inventory"].Visible = false;

        base.Open();
    }

    public override void SelectedItem(UIItem data, bool enter)
    {

        if (data.ItemTag.Equals("EndRound"))
            return;

        HeroInfoItem unit = data as HeroInfoItem;
    }

    public override void PressedItem(UIItem data)
    {
        if (data.ItemTag.Equals("HeroButton"))
        {
            this._allItems["HeroGroups"].Visible = true;
            this._allItems["PlayerActions"].Visible = true;
            this._allItems["HeroInventory"].Visible = true;
            this._allItems["ActionChoices"].Visible = false;
            this._allItems["Inventory"].Visible = false;
        }

        if (data.ItemTag.Equals("LootButton"))
        {
            this._allItems["HeroGroups"].Visible = false;
            this._allItems["PlayerActions"].Visible = false;
            this._allItems["HeroInventory"].Visible = true;
            this._allItems["ActionChoices"].Visible = false;
            this._allItems["Inventory"].Visible = true;
        }

        if (data.ItemTag.Equals("ActionButton"))
        {
            this._allItems["HeroGroups"].Visible = true;
            this._allItems["PlayerActions"].Visible = true;
            this._allItems["HeroInventory"].Visible = false;
            this._allItems["ActionChoices"].Visible = true;
            this._allItems["Inventory"].Visible = false;
        }

        if (data.ItemTag.Equals("Close"))
        {
            this.Close();
        }
    }

    #region show panel control

    public void OnItemPlace(DragAndDropCell.DropDescriptor descriptor)
    {
        if(descriptor.destinationCell.ItemTag.Equals("InventoryActionItem"))
        {
            GM.PlayerIcon.Group.GetUnits()[this.activeHeroIndex].UnequipAction(descriptor.item.ItemID);
        }

        if(descriptor.destinationCell.ItemTag.Equals("HeroActionItem"))
        {
            GM.PlayerIcon.Group.GetUnits()[this.activeHeroIndex].EquipAction(descriptor.item.ItemID);
        }

        //if (descriptor.destinationCell.ItemTag.Equals("HeroLoot"))()
        //    LS.AddItem(descriptor.item.ItemID, LS.LootType.Weapon, "bag");

        if (descriptor.destinationCell.ItemTag.Equals("HeroLootWeapon1"))
            LS.EquipItem(descriptor.item.ItemID, LS.LootType.Weapon, GM.PlayerIcon.Group.GetUnits()[this.activeHeroIndex].Name);

        if (descriptor.destinationCell.ItemTag.Equals("HeroLootWeapon2"))
            LS.EquipItem(descriptor.item.ItemID, LS.LootType.SecondWeapon, GM.PlayerIcon.Group.GetUnits()[this.activeHeroIndex].Name);

        if (descriptor.destinationCell.ItemTag.Equals("HeroLootAmulet"))
            LS.EquipItem(descriptor.item.ItemID, LS.LootType.Amulet, GM.PlayerIcon.Group.GetUnits()[this.activeHeroIndex].Name);

        StartCoroutine(StartUpdateMenu());
    }

    IEnumerator StartUpdateMenu()
    {
        yield return new WaitForEndOfFrame();

        UpdateMenu();
    }

    private void UpdateMenu()
    {

        int index = 0;
        foreach (var skill in GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].skills)
        {
            skills[index].Setup(skill.Value);
            index++;
            if (index >= 5)
                break;
        }

        index = 0;
        foreach (var unit in GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].units)
        {
            groups[index].unitIcon.Image = unit.Key.Icon;
            groups[index].unitAmount.Text = unit.Value.ToString();

            index++;
            if (index >= 5)
                break;
        }

        for (int i = 0; i < 8; i++)
        {
            this.EquipedActions[i].RemoveItem();
            if (i < GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].EquipedActions.Count)
            {
                string act = GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].EquipedActions[i];
                this.EquipedActions[i].PlaceItem(IOM.BattleActionInfoDic[act].Icon, act, "ActionItem");
            }

        }

        for (int i = 0; i < 40; i++)
        {
            this.actions[i].RemoveItem();
            if (i < GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].actions.Count)
            {
                string act = GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].actions[i];
                this.actions[i].PlaceItem(IOM.BattleActionInfoDic[act].Icon, act, "ActionItem");
            }
        }

        LootItem item = LS.GetHeroItem(LS.LootType.Weapon, GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].Name);
        if (item != null)
        {
            Weapon1.PlaceItem(item.Icon, item.ID, LS.LootType.Weapon.ToString());
        }
        else
            Weapon1.RemoveItem();

        item = LS.GetHeroItem(LS.LootType.SecondWeapon, GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].Name);
        if (item != null)
        {
            Weapon2.PlaceItem(item.Icon, item.ID, LS.LootType.SecondWeapon.ToString());
        }
        else
            Weapon2.RemoveItem();

        item = LS.GetHeroItem(LS.LootType.Amulet, GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].Name);
        if (item != null)
        {
            Amulet.PlaceItem(item.Icon, item.ID, LS.LootType.Amulet.ToString());
        }
        else
            Amulet.RemoveItem();

        item = LS.GetHeroItem(LS.LootType.Armor, GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].Name);
        if (item != null)
        {
            Armor.PlaceItem(item.Icon, item.ID, LS.LootType.Armor.ToString());
        }
        else
            Armor.RemoveItem();

        item = LS.GetHeroItem(LS.LootType.Extra, GM.PlayerIcon.Group.GetUnits()[activeHeroIndex].Name);
        if (item != null) 
        {
            Extra.PlaceItem(item.Icon, item.ID, LS.LootType.Extra.ToString());
        }
        else
            Extra.RemoveItem();

        for (int i = 0; i < 40; i++)
        {
            this.loots[i].RemoveItem();
            if (i < LS.Bag.Values.Count)
            {
                this.loots[i].PlaceItem(LS.Bag.ElementAt(i).Value.Icon, LS.Bag.ElementAt(i).Value.ID, LS.Bag.ElementAt(i).Value.type.ToString());
            }
        }
    }

    #endregion

    
}