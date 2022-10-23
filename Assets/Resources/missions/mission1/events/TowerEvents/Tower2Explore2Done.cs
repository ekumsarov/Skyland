using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class Tower2Explore2Done : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'StorageAdd', 'ID':'Wood', 'Amount':'10' }"),
               Make("{ 'Base':'StorageAdd', 'ID':'Stone', 'Amount':'5' }")
            };
        }
    }
}