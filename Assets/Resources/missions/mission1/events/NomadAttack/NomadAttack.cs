using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventPackSystem;

namespace EventPackSystem1
{
    public class NomadAttack : EventPack
    {
        public override void Create()
        {
            this.events = new List<SimpleJSON.JSONNode>()
            {
               Make("{ 'Base':'MoveCamera', 'to':'player', 'fast':'true' }"),
               Make("{ 'Base':'CreateObject', 'ID':'NomadShip', 'OnIsland':'island_1' }"),
               Make("{ 'Base':'CompleteCutScene' }"),
               Make("{ 'Base':'ShowMenu' }"),
               Make("{ 'Base':'MoveObject', 'ID':'NomadShip', 'to':'island_0', 'fast':'false', 'MoveTime':'5' }"),
               Make("{ 'Base':'ShowTooltip', 'ID':'player', 'text':'NomadAttack1' }"),
               Make(@"{ 'Base':'Dialogue', 'To':'NomadShip', 'ActionID':'NomadAttack', 'Text':'NomadAttack2', 'Actions':
                    [
                        { 'type':'Pack', 'CallID':'NomadAttackGive', 'Text':'NomadAttackGive'},   
                        { 'type':'Pack', 'CallID':'NomadAttackFight', 'Text':'NomadAttackFight'}
                    ]")
            };
        }
    }
}