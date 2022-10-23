using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GameEvents
{
    public class AiInit : GameEvent
    {
        string Island;
        int Settlements;
        string cType;
        string dType;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "AiInit";

            //        "{ 'Event':'AiInit', 'Island':'Random', 'Settlements':'2', 'Charachter':'Random', 'StartPower':'Random' }"

            this.Island = "random";
            if (node["Island"] != null)
                this.Island = node["Island"].Value;

            this.Settlements = 0;
            if (node["Settlements"] != null)
                this.Settlements = node["Settlements"].AsInt;

            this.cType = "random";
            if (node["Charachter"] != null)
                this.cType = node["Charachter"].Value;

            this.dType = "Random";
            if (node["Diplomacy"] != null)
                this.dType = node["Diplomacy"].Value;
        }

        public override bool CanActive()
        {
            return true;
        }

        public override void Start()
        {
            AICharacter CharType = AICharacter.Defender;
            if (this.cType == "random")
                CharType = (AICharacter)UnityEngine.Random.Range(0, 2);
            else
                CharType = (AICharacter)Enum.Parse(typeof(AICharacter), this.cType);

            AIDiplomacyType DipType = (AIDiplomacyType)Enum.Parse(typeof(AIDiplomacyType), this.dType);
            

            End();
        }
    }
}