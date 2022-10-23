using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GameEvents
{
    public class ClassicBattle : BattleEvent
    {
        #region Start state

        public override void CompleteFadeIn()
        {
            List<string> playStack = new List<string>();
            Dictionary<string, int> enemyStack = new Dictionary<string, int>();

            Island island = null;

            if (isl != null && !isl.Equals(""))
                island = GetObject(isl).GetComponent<Island>();
            else if (Object != GEM.instance)
                island = IM.Islands[GM.Player.IslandNumber];
                
            /*
            if (Player.Equals("Setup") && PlayerStack != null)
            {
                for (int l = 0; l < PlayerStack.Count; l++)
                    playStack.Add(PlayerStack[l]);
            }
            else if (Player.Equals("Player"))
            {
                Dictionary<string, int> temp = GM.Player.GetUnits();

                foreach (var key in temp.Keys)
                    playStack.Add(key);
            }
            else if (Player.Equals("Build"))
            {
                Center cen = BM.GetCenterOnIsland(island.IslandNumber);

                Dictionary<string, int> temp = cen.GetUnits();

                foreach (var key in temp.Keys)
                    playStack.Add(key);
            }
            else if (Player.Equals("Classic"))
            {
                if (GM.Player.IslandNumber == island.IslandNumber)
                {
                    Dictionary<string, int> temp = GM.Player.GetUnits();

                    foreach (var key in temp.Keys)
                        playStack.Add(key);
                }

                if (island.HasBuild)
                {
                    Center cen = BM.GetCenterOnIsland(island.IslandNumber);

                    Dictionary<string, int> temp = cen.GetUnits();

                    foreach (var key in temp.Keys)
                        playStack.Add(key);
                }
            }
            else
            {
                SceneObject def = GetObject(Player) as SceneObject;
                Dictionary<string, int> darr = def.GetUnits();

                foreach (var key in darr.Keys)
                    playStack.Add(key);
            }

            if (Enemy.Equals("Setup") && EnemyStack != null)
            {
                for (int l = 0; l < EnemyStack.Count; l++)
                    enemyStack.Add(EnemyStack.ElementAt(l).Key, EnemyStack.ElementAt(l).Value);
            }
            else
            {
                SceneObject enemy = GM.GetUniqOnIsland(island.IslandNumber);

                Dictionary<string, int> arr = enemy.GetUnits();

                foreach (var key in arr.Keys)
                    enemyStack.Add(key, arr[key]);
            }

            this._ai = BattleAI.Make(AT, this);

            Action dela = End;
            UIM.StartBattle(playStack, enemyStack, dela, this, mapID, player: DT, ai: AT, rounds: Rounds);*/
        }

        #endregion

        #region Main states

        public override void StartRound()
        {
            // Effect check

        }

        public override void EndTurn()
        {
            //this._ai.StartRound(this.GetEnemyActionPoints(), this.CurrentRound);
        }

        public override void EndRound()
        {

        }

        public override void UnitDead()
        {
            base.UnitDead();
        }

        #endregion

        #region AI



        #endregion

        #region static
        public static ClassicBattle Create(string player, string enemy, string isl = null, List<string> playerStack = null, Dictionary<string, int> enemyStack = null, int round = 10)
        {
            ClassicBattle temp = new ClassicBattle();
            temp.ID = "StartBattle";

            temp.Player = player;
            temp.Enemy = enemy;
            temp.isl = isl;
            //temp.PlayerStack = playerStack;
            //temp.EnemyStack = enemyStack;
            temp.Rounds = round;

            return temp;
        }
        #endregion
    }
}