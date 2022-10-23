using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystemAi
{
    public class Greeting : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'MoveCamera', 'to':'AIself', 'fast':'false' }"),
               Make("{ 'Base':'CallAction', 'ID':'Greeting' }")
            };
        }
    }
}
