using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Lodkod;

public class RewardMenu : MenuEx
{
    List<RewardItem> _rewards;
    public SimpleText RewardText;

    public override void Setting()
    {
        base.Setting();

        RewardText.Text = LocalizationManager.Get("GetRewardText");

        this._rewards = new List<RewardItem>();
        foreach(var item in this._allItems)
        {
            if (item.Value.ItemTag.Equals("RewardItem"))
                this._rewards.Add(item.Value.GetComponent<RewardItem>());
        }
    }

    public override void Open()
    {
        if(UIParameters.Reward.list.Count == 0)
        {
            this.Close();
            return;
        }

        GM.GameState = GameState.ContextWorking;

        int Count;
        if (this._rewards.Count > UIParameters.Reward.list.Count)
            Count = this._rewards.Count;
        else
            Count = UIParameters.Reward.list.Count;



        for(int i = 0; i < Count; i++)
        {
            if(i >= this._rewards.Count)
            {
                this._rewards.Add(RewardItem.Create("Reward" + i));
                this._rewards[i].SetupItem(UIParameters.Reward.list[i]);
                this._rewards[i].Visible = true;
            }
            else
            {
                if (i >= UIParameters.Reward.list.Count)
                    this._rewards[i].Visible = false;
                else
                {
                    this._rewards[i].SetupItem(UIParameters.Reward.list[i]);
                    this._rewards[i].Visible = true;
                }
            }
        }

        this.Reset();
        base.Open();
    }

    public override void Reset()
    {
        foreach (var item in this._allItems)
            item.Value.Reset();

        foreach (var panel in this._allPanels)
            panel.Value.Reset();
    }

    public override void PressedItem(UIItem data)
    {
        this.Close();
        GM.GameState = GameState.Game;
    }
}
