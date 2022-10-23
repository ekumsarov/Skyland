using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class RepairShuttleStart : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
                Make("{ 'Base':'ReactButRebuild', 'To':'MainShip', 'Ico':'buid_icon', 'EventID':'RepairShuttle', 'timer':'0'  }"),
                Make("{ 'Base':'AddPack', 'ID':'RepairShuttleDone', 'To':'MainShip' }"),
                Make(@"{ 'Base':'ActionWork', 'ActionType':'Context', 'To':'MainShip', 'ActionID':'RepairShuttle', 'Text':'RepairShuttle', 'Actions':
                    [
                        { 'type':'Pack', 'CallID':'RepairShuttleComplete', 'Text':'RepairShuttleAction', 'Condition':{ 'type':'Stat', 'stat':'Wood', 'count':'10' } },
                        { 'type':'Close', 'Text':'CloseDialogue'}
                    ]")
            };
        }
    }
}