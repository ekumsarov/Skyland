using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class RepairShuttleComplete : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
                Make("{ 'Base':'ReactButRebuild', 'To':'MainShip', 'Ico':'buid_icon', 'EventID':'RepairShuttleDone', 'timer':'4'  }"),
                Make("{ 'Base':'PlayerTask', 'To':'MainShip', 'Task':'Interact'  }")
            };
        }
    }
}