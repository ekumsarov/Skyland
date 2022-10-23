using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using SimpleJSON;
using System.Linq;
using GameEvents;

namespace BuildTypes
{
    public class ScoutHut : BuildInstance
    {
        ActionButtonInfo _scoutAction;

        protected override void InitializeBuild(BuildCell parent, BuildInfo info)
        {
            this._parent = parent;
            this._info = info;

            this._buildTime = info.BuildTime;
            this._scoutAction = ActionButtonInfo.Create("ScoutForUnit").SetCallback(ScoutUnit)
                .SetAvailableCondition(StatCondition.Make("Skystone", 5, StatCondition.StatConType.More));
        }

        public override void CompleteBuild()
        {
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, stat.amount);

            this._parent.AddActionChoice(this._scoutAction);

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

        public void ScoutUnit()
        {
            SM.Stats["Skystone"].Count -= 5;

            this._parent.RemoveActionChoice(this._scoutAction.ID);

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "StartScoutUnit", lSize: 45);
            ExpiredDay.ExpiredAfterTicks(UnityEngine.Random.Range(6,12), act: CompleteScoutUnit);
        }

        public void CompleteScoutUnit()
        {
            int unitsfound = 0;
            if(SM.Stats.ContainsKey("Food"))
            {
                float ratio = SM.Stats["Food"].RatioProductionConsumption;
                if(ratio >= 1)
                {
                    UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "CenterScountNotFoundUnits", lSize: 45);
                    return;
                }
                else if(ratio >= 0.7)
                {
                    unitsfound = UnityEngine.Random.Range(1, 3);
                }
                else if(ratio < 0.7)
                {
                    unitsfound = UnityEngine.Random.Range(2, 9);
                }

            }

            UIM.ShowTooltip(UIM.ScreenCenter, TooltipFit.Auto, TooltipTimeMode.Click, TooltipFillMode.Instantly, TooltipObject.UI, "CenterScoutFoundUnits", lSize:45);
            SM.Stats["Units"].Count += unitsfound;
            this._parent.AddActionChoice(this._scoutAction);
        }
    }
}