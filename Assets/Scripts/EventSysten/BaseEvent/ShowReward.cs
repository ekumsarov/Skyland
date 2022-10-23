using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class ShowReward : GameEvent
    {
        List<RewardItemInfo> list;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "ShowReward";

            list = new List<RewardItemInfo>();
            if (node["list"] != null)
            {
                JSONArray arr = node["list"].AsArray;
                for(int i = 0; i > arr.Count; i++)
                {
                    Represent.Type presenter = Represent.Type.Simple;
                    if (arr[i]["represent"] != null)
                        presenter = (Represent.Type)Enum.Parse(typeof(Represent.Type), arr[i]["represent"].Value);

                    string icon = "info";
                    if (arr[i]["icon"] != null)
                        icon = arr[i]["info"].Value;

                    float fVal = 0;
                    if(arr[i]["FirstValue"] != null)
                        fVal = arr[i]["FirstValue"].AsFloat;

                    float sVal = 0;
                    if (arr[i]["SecondValue"] != null)
                        sVal = arr[i]["SecondValue"].AsFloat;

                    list.Add(RewardItemInfo.Create(icon, fVal, sVal, presenter));
                }
            }

        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIParameters.Reward.list = this.list;
            UIM.OpenMenu("RewardMenu");

            End();
        }

        #region static
        public static ShowReward Create(List<RewardItemInfo> list)
        {
            ShowReward temp = new ShowReward();
            temp.ID = "ShowReward";

            temp.list = list;

            return temp;
        }
        #endregion
    }
}