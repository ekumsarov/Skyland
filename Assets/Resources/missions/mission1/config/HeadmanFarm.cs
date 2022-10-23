using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;
using SimpleJSON;
using BuildTypes;

namespace BuildTypes1
{
    public class HeadmanFarm : BuildInstance
    {
        private List<iStat> _productionList;

        protected override void InitializeBuild(BuildCell parent, BuildInfo info)
        {
            this._parent = parent;
            this._info = info;

            this._buildTime = info.BuildTime;

            this._productionList = new List<iStat>();
            if (this._info.Special["Productions"] != null)
                this._productionList = iStat.createResList(this._info.Special["Productions"]);
            else if (this._info.Special["Type"] != null)
                this._productionList.Add(iStat.Create(this._info.Special["Type"].Value, this._info.Special["Production"].AsFloat));
        }

        public override void CompleteBuild()
        {
            foreach (var stat in this._productionList)
                if (stat.amount >= 0)
                    SM.AddProduct(stat.type, stat.amount);

            foreach (var stat in this._info.Consumtion)
                if (stat.amount >= 0)
                    SM.AddСonsumption(stat.type, stat.amount);

            this._parent.State = BuildState.bs_Ready;
        }

        public override void Upgrade()
        {
            foreach (var stat in this._productionList)
                if (stat.amount >= 0)
                    SM.AddProduct(stat.type, -stat.amount);

            foreach (var stat in this._info.Consumtion)
                if (stat.amount >= 0)
                    SM.AddСonsumption(stat.type, -stat.amount);
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

            foreach (var stat in this._productionList)
                if (stat.amount >= 0)
                    SM.AddProduct(stat.type, -stat.amount);

            foreach (var stat in this._info.Consumtion)
                if (stat.amount >= 0)
                    SM.AddСonsumption(stat.type, -stat.amount);
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

            foreach (var stat in this._productionList)
                if (stat.amount >= 0)
                    SM.AddProduct(stat.type, stat.amount);

            foreach (var stat in this._info.Consumtion)
                if (stat.amount >= 0)
                    SM.AddСonsumption(stat.type, stat.amount);
        }
    }
}