using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class PlayUIAnimation : GameEvent
    {
        UIeX target;
        string animationID;
        string To;

        JSONNode animationType;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "PlayUIAnimation";

            animationID = null;
            if (node["animationID"] != null)
                animationID = node["animationID"].Value;

            To = null;
            if (node["To"] != null)
                To = node["To"].Value;

            animationType = null;
            if (node["animation"] != null)
                animationType = node["animation"];
        }

        public override bool CanActive()
        {
            if (animationID == null || To == null)
                return false;

            return true;
        }

        public override void Start()
        {
            target = GetObject(To) as UIeX;

            if(target == null)
            { Debug.LogError("NO OBJECT TO PLAY ANIMATION ID " + To); End(); return; }

            if(animationType != null)
            {
                UIAnimation anim = GetAnimation(animationType);

                target.SetAnimation(anim, this.animationID);
            }

            target.PlayAnimation(this.animationID);

            End();
        }

        UIAnimation GetAnimation(JSONNode nod)
        {
            if (nod["type"] == null)
                return null;

            if(nod["type"].Value.Equals("Scale"))
            {
                UIAnimation temp = null;

                float nt = 0.5f;

                string fl = nt.ToString();

                float duration = nod["time"].AsFloat;
                Vector3 vector = GetVector3FromNode(nod["offset"]);

                if (nod["offset"] != null)
                    temp = UIM.ScaleOffset(target, vector, duration);

                if (nod["loop"] != null)
                {
                    int count = -1;
                    if (nod["loop"]["count"] != null)
                        count = nod["loop"]["count"].AsInt;

                    bool pingpong = false;
                    if (nod["loop"]["pingpong"] != null)
                        pingpong = nod["loop"]["pingpong"].AsBool;

                    temp.SetLoops(count, pingpong);
                }

                return temp;
            }

            return null;
        }
    }
}