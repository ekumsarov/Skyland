using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;
using GameEvents;

public class BuildMenu : MenuEx
{
    public override void Setting()
    {
        base.Setting();
        
        this._allIconTexts["CostInfo"].IconText.Text(LocalizationManager.Get("CostLabel"));
        this._allIconTexts["CostInfo"].IconText.ShowComplete();

        this._allIconTexts["ConsumtionInfo"].IconText.Text(LocalizationManager.Get("ConsumptionInDayLabel"));
        this._allIconTexts["ConsumtionInfo"].IconText.ShowComplete();

        this._buildItems = new List<BuildItem>();
        List<UIItem> items = this._allItems.Values.Where(item => item.ItemTag.Equals("BuildItem")).ToList();
        foreach (var ite in items)
        {
            this._buildItems.Add(ite as BuildItem);
            ite._parentMenu = this;
        }

        this._costStats = new List<StatItem>();
        List<UIItem> costitems = this._allItems.Values.Where(item => item.ItemTag.Equals("CostStats")).ToList();
        foreach (var ite in costitems)
        {
            this._costStats.Add(ite as StatItem);
            ite._parentMenu = this;
        }

        this._consStats = new List<StatItem>();
        List<UIItem> consitems = this._allItems.Values.Where(item => item.ItemTag.Equals("ConsStats")).ToList();
        foreach (var ite in consitems)
        {
            this._consStats.Add(ite as StatItem);
            ite._parentMenu = this;
        }

        for (int i = 0; i < this._buildItems.Count; i++)
            this._buildItems[i].ItemNum = i;
    }

    #region Base parameters

    public BuildPlace _build;
    List<BuildItem> _buildItems;
    BuildInfo infoBuild;
    int selectedInfoBuild = 0;

    List<StatItem> _costStats;
    List<StatItem> _consStats;

    #endregion

    public override void Open()
    {
        if(this._build == null)
        {
            Debug.LogError("No setup main build in menu");
            return;
        }

        if(this._build.OpenBuilds.Count == 0)
        {
            Debug.LogError("Build don't have infos");
            return;
        }

        this.selectedInfoBuild = 0;

        for (int i = 0; i < this._buildItems.Count; i++)
        {
            if (i < this._build.OpenBuilds.Count)
            {
                this._buildItems[i].BindBuildInfo(this._build.OpenBuilds[i]);
                this._buildItems[i].Visible = true;
            } 
            else
                this._buildItems[i].Visible = false;

        }

        this.selectedInfoBuild = 0;
        this.SetupInfo(this._buildItems[0].BindedInfo);

        base.Open();
    }

    public override void Close()
    {
        this._build = null;
        this.selectedInfoBuild = 0;

        base.Close();
    }

    public override void PressedItem(UIItem data)
    {
        if(data.ItemTag.Equals("Close"))
        {
            this.Close();
        }
        else if(data.ItemTag.Equals("Build"))
        {
            BuildInfo selectedBuild = this._buildItems[this.selectedInfoBuild].BindedInfo;

            bool canCreate = iStat.CheckList(selectedBuild.Cost);

            if(!canCreate)
                return;

            this._build.StartBuild(selectedBuild);
            this.Close();
        }
        else if(data.ItemTag.Equals("BuildItem"))
        {
            this.selectedInfoBuild = data.ItemNum;

            this.SetupInfo(this._buildItems[this.selectedInfoBuild].BindedInfo);
        }
    }

    void SetupInfo(BuildInfo setupableInfo)
    {
        this._allImages["BuildInfoICon"].Image = setupableInfo.Icon;
        this._allIconTexts["DescriptionInfo"].IconText.Text(LocalizationManager.Get(setupableInfo.Description));
        this._allIconTexts["DescriptionInfo"].IconText.ShowComplete();

        int statItemIndex = 0;
        for (int i = 0; i < setupableInfo.Cost.Count; i++)
        {
            if (setupableInfo.Cost[i].amount > 0 && statItemIndex < this._costStats.Count)
            {
                this._costStats[statItemIndex].SetupStatItem(setupableInfo.Cost[i].type, setupableInfo.Cost[i].amount.ToString());
                this._costStats[statItemIndex].Visible = true;
                statItemIndex++;
            }
               
        }
        if(statItemIndex < this._costStats.Count)
        {
            for (int i = statItemIndex; i < this._costStats.Count; i++)
                this._costStats[i].Visible = false;
        }

        statItemIndex = 0;
        for (int i = 0; i < setupableInfo.Consumtion.Count; i++)
        {
            if (setupableInfo.Consumtion[i].amount > 0 && statItemIndex < this._consStats.Count)
            {
                this._consStats[statItemIndex].SetupStatItem(setupableInfo.Consumtion[i].type, setupableInfo.Consumtion[i].amount.ToString());
                this._consStats[statItemIndex].Visible = true;
                statItemIndex++;
            }

        }
        if (statItemIndex < this._consStats.Count)
        {
            for (int i = statItemIndex; i < this._consStats.Count; i++)
                this._consStats[i].Visible = false;
        }
    }
}
