using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class NomadAttackGive : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'StorageAdd', 'AddList':[ { 'id':'Wood', 'count':-10 }, { 'id':'Stone', 'count':-10 }, { 'id':'Food', 'count':-10 }, { 'id':'Skystone', 'count':-5 }]"),
               Make("{ 'Base':'ShowTooltip', 'ID':'NomadShip', 'text':'NomadGiveAnswer' }"),
               Make("{ 'Base':'MoveObject', 'ID':'NomadShip', 'to':'island_1', 'fast':'false', 'MoveTime':'5' }"),
               Make("{ 'Base':'DestroyObject', 'ID':'NomadShip' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'player', 'text':'NomadGiveAnswer2' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'player', 'text':'NomadAttackFightCon2' }"),
               Make("{ 'Base':'FadeScreen', 'Fade':'in' }"),
               Make("{ 'Base':'CallPack', 'ID':'ToraldMeet' }")
            };
        }
    }
}