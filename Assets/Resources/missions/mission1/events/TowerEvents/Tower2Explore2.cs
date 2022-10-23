using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class Tower2Explore2 : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'ReactButRebuild', 'ID':'MainAction', 'To':'tower14', 'hour':'1', 'EventID':'Tower2Explore2Done' }"),
               Make("{ 'Base':'PushReact', 'To':'tower14' }")
            };
        }
    }
}   