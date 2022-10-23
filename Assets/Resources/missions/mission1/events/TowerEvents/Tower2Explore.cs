using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class Tower2Explore : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'StorageAdd', 'ID':'Unit', 'Amount':'-2' }"),
               Make("{ 'Base':'ReactButRebuild', 'ID':'MainAction', 'To':'tower14', 'hour':'3', 'EventID':'Tower2ExploreDone' }"),
               Make("{ 'Base':'PushReact', 'To':'tower14' }")
            };
        }
    }
}