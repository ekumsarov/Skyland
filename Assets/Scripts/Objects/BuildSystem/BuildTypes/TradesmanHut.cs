using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using SimpleJSON;
using System.Linq;
using GameEvents;

namespace BuildTypes
{
    public class TradesmanHut : BuildInstance
    {
        List<ActionButtonInfo> _choices;
        int buyNewStuffLefel = 0;
        bool canBuy = true;

        protected override void InitializeBuild(BuildCell parent, BuildInfo info)
        {
            this._parent = parent;
            this._info = info;

            this._buildTime = info.BuildTime;
            this._choices = new List<ActionButtonInfo>();
            this._choices.Add(ActionButtonInfo.Create("BuyNewStuff").SetCallback(BuyNewStuff)
                .SetAvailableCondition(StatCondition.Make("Skystone", 5, StatCondition.StatConType.More)));
        }

        public override void CompleteBuild()
        {
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, stat.amount);

            for(int i = 0; i < this._choices.Count; i++)
            {
                this._parent.AddActionChoice(this._choices[i]);
            }

            this._parent.State = BuildState.bs_Ready;
            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Instantly, TooltipObject.UI, "NotReady", lSize: 45, time: 2f);
        }

        public override void Upgrade()
        {
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, -stat.amount);
        }

        public override void StopProduction()
        {
            for (int i = 0; i < this._info.Cost.Count; i++)
            {
                if (this._info.Cost[i].type.Equals("Unit") && this._info.Cost[i].amount > 0)
                {
                    SM.Stats["Unit"].Count += this._info.Cost[i].amount;
                    break;
                }

            }
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, -stat.amount);
        }

        public override void ActivateProduction()
        {
            for (int i = 0; i < this._info.Cost.Count; i++)
            {
                if (this._info.Cost[i].type.Equals("Unit") && this._info.Cost[i].amount > 0)
                {
                    if (SM.Stats["Unit"].Count < this._info.Cost[i].amount)
                    {
                        UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Tootip, TooltipFillMode.Instantly, TooltipObject.UI, "NoHaveUnit", lSize: 45);
                        return;
                    }
                    else
                        SM.Stats["Unit"].Count -= this._info.Cost[i].amount;
                    break;
                }

            }
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, stat.amount);
        }

        public void BuyNewStuff()
        {
            if(buyNewStuffLefel == 0)
            {
                SM.Stats["Skystone"].Count -= 5;
            }

            this._parent.RemoveActionChoice("BuyNewStuff");
            ActionButtonInfo item = this._choices.FirstOrDefault(ite => ite.ID.Equals("BuyNewStuff"));
            if (item != null)
                this._choices.Remove(item);

            canBuy = false;
            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "BuildplaceBuyStuff", lSize: 45);
            ExpiredDay.ExpiredAfterTicks(8, act: CompleteBuyNewStuff);
        }

        public void CompleteBuyNewStuff()
        {
            for(int i = 0; i < this._choices.Count; i++)
            {
                this._parent.RemoveActionChoice(this._choices[i].ID);
            }

            if(buyNewStuffLefel == 0)
            {
                this._choices.Add(ActionButtonInfo.Create("BuildplaceBuySimpleSword")
                    .SetAvailableCondition(StatCondition.Make("Skystone", 8, StatCondition.StatConType.More))
                    .SetCallData("BuySimpleSword")
                    .SetType(ActionType.Pack));

                this._parent.Activity.PushPack("BuySimpleSword", new List<GameEvent>()
                {
                    AddStat.Create("Skystone", -8),
                    LootWork.Create("SimpleSword", LS.LootType.Weapon, "Player"),
                    ShowTooltip.Create(UIM.ScreenCenter, null, Text:"BuildplaceCongrat")
                });

                this._choices.Add(ActionButtonInfo.Create("BuildplaceBuySimpleBow")
                    .SetAvailableCondition(StatCondition.Make("Skystone", 10, StatCondition.StatConType.More))
                    .SetCallData("BuySimpleBow")
                    .SetType(ActionType.Pack));

                this._parent.Activity.PushPack("BuySimpleBow", new List<GameEvent>()
                {
                    AddStat.Create("Skystone", -8),
                    LootWork.Create("SimpleBow", LS.LootType.Weapon, "Player"),
                    ShowTooltip.Create(UIM.ScreenCenter, null, Text:"BuildplaceCongrat")
                });
            }

            for (int i = 0; i < this._choices.Count; i++)
            {
                this._parent.AddActionChoice(this._choices[i]);
            }
        }
    }
}
