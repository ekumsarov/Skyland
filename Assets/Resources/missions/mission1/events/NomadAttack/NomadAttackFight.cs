using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class NomadAttackFight : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'ShowTooltip', 'ID':'NomadShip', 'text':'NomadAttackFightAnswer' }"),
               Make("{ 'Base':'FadeScreen', 'Fade':'in' }"),
               Make("{ 'Base':'Wait', 'second':'2' }"),
               Make("{ 'Base':'FadeScreen', 'Fade':'out' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'NomadShip', 'text':'NomadAttackFightAnswer2' }"),
               Make("{ 'Base':'MoveObject', 'ID':'NomadShip', 'to':'island_1', 'fast':'false', 'MoveTime':'5' }"),
               Make("{ 'Base':'DestroyObject', 'ID':'NomadShip' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'player', 'text':'NomadAttackFightCon' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'player', 'text':'NomadAttackFightCon2' }"),
               Make("{ 'Base':'FadeScreen', 'Fade':'in' }"),
               Make("{ 'Base':'CallPack', 'ID':'ToraldMeet' }")
            };
        }
    }
}
