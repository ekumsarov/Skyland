using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class FollowCamera : GameEvent
    {
        string FollowTo;
        bool follow;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "FollowCamera";

            FollowTo = "MainShip";
            if (node["FollowTo"] != null)
                FollowTo = node["FollowTo"].Value;

            follow = true;
            if (node["follow"] != null)
                follow = node["follow"].AsBool;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            if (follow)
                GM.Camera.StartFollow(GetObject(FollowTo) as SceneObject);
            else
                GM.Camera.StopFollow();

            End();
        }


        #region static
        public static FollowCamera Create(string FollowTo, bool follow)
        {
            FollowCamera temp = new FollowCamera();
            temp.ID = "FollowCamera";

            temp.FollowTo = FollowTo;
            temp.follow = follow;

            return temp;
        }
        #endregion
    }
}