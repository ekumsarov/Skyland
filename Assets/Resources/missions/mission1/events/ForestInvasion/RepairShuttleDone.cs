using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class RepairShuttleDone : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
                Make("{ 'Base':'StorageAdd', 'AddList':[ { 'id':'Wood', 'count':'-10' } ] }"),
                Make("{ 'Base':'ReactButRebuild', 'To':'MainShip', 'Ico':'oprions_icon', 'EventID':'MainAction', 'timer':'0'  }")
            };
        }
    }
}