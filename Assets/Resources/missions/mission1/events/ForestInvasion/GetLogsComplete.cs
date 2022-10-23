using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class GetLogsComplete : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'StorageAdd', 'AddList':[ { 'id':'Wood', 'count':'10' } ] }"),
               Make("{ 'Base':'ReactButSetup', 'To':'forest14', 'setup':'Remove', 'ID':'GetLogsComplete'}"),
               Make("{ 'Base':'RemoveQuestBut', 'ID':'GetLogs' }")
            };
        }
    }
}