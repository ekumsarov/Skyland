using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class VillageTraderEvent : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "VillageTraderEvent";

            Simple = false;

            subscriber = Subscriber.Create(this);

            initialized = false;
        }

        public void Play()
        {
            if (!initialized)
            {
                initialized = true;
                End();
                return;
            }

            End();
        }

        public void BuyNet()
        {
            Object.RepmoveActionChoice("TradeAction", "HunterNetBuy");
            SM.Stats["Skystone"].Count -= 4;
            LS.AddItem("WolfNet", LS.LootType.Extra, "Player");
            UIM.ShowTooltip(GM.Player, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.Game, "WolfNetRecieve", gEvent: this);
        }

        public void AddTownPassForSale()
        {
            Object.RepmoveActionChoice("VillageTraderGreetings2", "AskForTownPass");
            Object.AddActionChoice("TradeAction",
                ActionButtonInfo.Create("TownPassOnSale")
                .SetAvailableCondition(StatCondition.Make("Skystone", 80, StatCondition.StatConType.More))
                .SetCallback(BuyTownPass));

            Object.Actioned("TradeAction");
            End();
        }

        public void BuyTownPass()
        {
            SM.Stats["Skystone"].Count -= 80;
            LS.AddItem("TownPass", LS.LootType.Extra);
            QS.CompleteQuest("GetTownPass");
            Object.RepmoveActionChoice("TradeAction", "TownPassOnSale");
            UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Type, TooltipObject.Game, "TraderSoldTownPass");

            End();
        }

        public void SoldAnimalSkins()
        {
            UIM.ShowTooltip(Object, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.Game, "VillageTraderThxAnimal", time: 1.4f, lSize:45);
            SM.AddStat(1 * SM.Stats["AnimalSkin"].Count, "Skystone");
            SM.AddStat(2 * SM.Stats["AnimalThooth"].Count, "Skystone");
            SM.Stats["AnimalSkin"].Count = 0;
            SM.Stats["AnimalThooth"].Count = 0;

            End();
        }
    }
}