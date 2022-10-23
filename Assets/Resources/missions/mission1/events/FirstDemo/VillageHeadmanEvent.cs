using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using GameEvents;


namespace GameEvents1
{
    public class VillageHeadmanEvent : GameEvent
    {

        Subscriber subscriber;

        public override void Init()
        {
            this.ID = "VillageHeadmanEvent";

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

        public void NeedAJob()
        {
            //SceneObject temp = GM.GetObject("Cave") as SceneObject;
            //if(temp != null)
            //{
            //    temp.Lock = false;
            //}

            BuildPlace center = GM.GetObject("Sanctuary") as BuildPlace;
            if(center != null)
            {
                Stat.Create("Bakery", icon: "BreadIcon", mainStat: true, changeOnEmpty: true);
                center.AddAvaliable("HeadmanFarm");
            }

            End();
        }
    }
}