using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class InitCanvas : GameEvent
    {
        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "InitCanvas";
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            UIM.InitSceenDemension();
            End();
        }


        #region static
        public static InitCanvas Create()
        {
            InitCanvas temp = new InitCanvas();
            temp.ID = "InitCanvas";

            return temp;
        }
        #endregion
    }
}