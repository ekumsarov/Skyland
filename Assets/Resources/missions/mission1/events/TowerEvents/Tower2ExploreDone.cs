using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class Tower2ExploreDone : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'StorageAdd', 'ID':'Unit', 'Amount':'2' }"),
               Make("{ 'Base':'StorageAdd', 'ID':'Wood', 'Amount':'15' }"),
               Make("{ 'Base':'StorageAdd', 'ID':'Stone', 'Amount':'10' }")
            };
        }
    }
}