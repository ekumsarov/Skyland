using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using SimpleJSON;

namespace BuildTypes
{
    public class Casarm : BuildInstance
    {
        List<ActionButtonInfo> _choices;
        int guardsLevel = 0;

        protected override void InitializeBuild(BuildCell parent, BuildInfo info)
        {
            this._parent = parent;
            this._info = info;

            this._buildTime = info.BuildTime;
            //this._choices = new List<ActionButtonInfo>();

            //this._choices.Add(ActionButtonInfo.Create("SentryBuyGuards").SetCallback(BuyGuards));
        }

        public override void CompleteBuild()
        {
            //foreach (var stat in this._info.Consumtion)
            //    if (stat.amount >= 0)
            //        SM.AddСonsumption(stat.type, stat.amount);



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

        public void BuyGuards()
        {

        }
    }
}
