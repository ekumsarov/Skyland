using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GameEvents
{
    public class IslandResource : GameEvent
    {

        int RandomAmount = 0;
        int islandID = 0;
        bool pair = false;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "IslandResource";

            if (node["ID"] == null)
            {
                Debug.LogError("ID IS NULL ON SET OF ISLAND");
                return;
            }

            pair = false;
            if (node["Pair"] != null)
                pair = node["Pair"].AsBool;

            if (node["ID"].Value == "random")
            {
                if (node["RandomAmount"] != null)
                    RandomAmount = node["RandomAmount"].AsInt;
                else
                    RandomAmount = 4;

                islandID = -1;

                return;
            }

            islandID = node["ID"].AsInt;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (islandID == -1)
            {
                List<Island> tempList = IM.Islands.Where(isl => !isl.HasResource).ToList();

                if (tempList.Count / 2 < RandomAmount)
                    RandomAmount = tempList.Count;

                for (int i = 0; i < RandomAmount; i++)
                {
                    tempList = IM.Islands.Where(isl => !isl.HasResource).ToList();
                    int ranNum = UnityEngine.Random.Range(0, tempList.Count - 1);
//                    IM.MakeResourceOnIsland(tempList[ranNum], pair);
                    tempList.RemoveAt(ranNum);
                }

                End();
                return;
            }

            if (islandID >= IM.Islands.Count)
            {
                Debug.LogError("Incorrect island Number");
                return;
            }

//            IM.MakeResourceOnIsland(IM.Islands[islandID], pair);

            End();
        }
    }
}