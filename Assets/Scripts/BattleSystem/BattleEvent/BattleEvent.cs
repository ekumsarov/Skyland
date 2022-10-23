using UnityEngine;
using SimpleJSON;
using Lodkod;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace GameEvents
{
    public class BattleEvent : GameEvent
    {
        #region Start state
        protected string Player;
        protected string Enemy;

        protected string EnemyObject;

        protected List<string> PlayerSetup;
        protected List<string> EnemySetup;

        protected string isl;

        protected int Rounds;

        protected Subscriber sub;

        protected ResultID result;

        Action DrawDelegate = null;

        public override void PrepareEvent(JSONNode node)
        {
            this.ID = "BattleEvent";

            Player = "Classic";
            if (node["Player"] != null)
                Player = node["Player"].Value;

            Enemy = "Classic";
            if (node["Enemy"] != null)
                Enemy = node["Enemy"].Value;

            EnemyObject = "";
            if (node["EnemyObject"] != null)
                EnemyObject = node["EnemyObject"].Value;

            PlayerSetup = null;
            if (node["PlayerStack"] != null)
            {
                PlayerSetup = new List<string>();
                JSONArray ar = node["PlayerSetup"].AsArray;
                for (int i = 0; i < ar.Count; i++)
                    PlayerSetup.Add(ar[i].Value);
            }

            EnemySetup = null;
            if (node["EnemyStack"] != null)
            {
                EnemySetup = new List<string>();
                JSONArray ar = node["EnemySetup"].AsArray;
                for (int i = 0; i < ar.Count; i++)
                    EnemySetup.Add(ar[i].Value);
            }

            result = null;
            if ( node["Result"] != null)
            {
                result = ResultID.Create(node["Result"]);
            }

            isl = "Player";
            if (node["Island"] != null)
                isl = node["Island"].Value;

            Rounds = 50;
            if (node["Rounds"] != null)
                Rounds = node["Rounds"].AsInt;


        }

        public override bool CanActive()
        {
            return true;
        }

        public void Play()
        {
            this.sub = Subscriber.Create(this);
            UIM.Fade(callF: CompleteFadeIn);
        }

        public virtual void CompleteFadeIn()
        {
//            Island island;
//            if (isl == "Player")
//                island = IM.GetIsland(GM.Player);
//            else
//                island = GetObject(isl) as Island;

            List<HeroUnit> playStack = new List<HeroUnit>();
            List<HeroUnit> enemyStack = new List<HeroUnit>();

            if (Player.Equals("Setup") && PlayerSetup != null)
            {
                for (int l = 0; l < PlayerSetup.Count; l++)
                    if (IOM.HeroList.ContainsKey(PlayerSetup[l]))
                        playStack.Add(HeroUnit.Make(IOM.HeroList[PlayerSetup[l]]));
                    else
                        Debug.LogError("No Hero/Enemy info ID: " + PlayerSetup[l]);
            }
            else if (Player.Equals("Player"))
            {
                playStack = (GM.Player.Group.GetUnits());
            }
            /*else if (Player.Equals("Build"))
            {
                Center cen = BM.GetCenterOnIsland(island.IslandNumber);

                playStack = cen.Group.GetUnits();
            }*/
            else if (Player.Equals("Classic"))
            {
                playStack = GM.Player.Group.GetUnits();
                /*
                if (island == null)
                {
                    Debug.LogError("Not found island: " + isl);
                    End();
                    return;
                }

                if (GM.Player.IslandNumber == island.IslandNumber)
                    playStack = GM.Player.Group.GetUnits();
                else if (island.HasBuild)
                {
                    Center cen = BM.GetCenterOnIsland(island.IslandNumber);
                    if(cen != null)
                        playStack = cen.Group.GetUnits();
                }*/
            }
            else
            {
                SceneObject def = GetObject(Player) as SceneObject;
                playStack = def.Group.GetUnits();
            }

            if (Enemy.Equals("Setup") && EnemySetup != null)
            {
                for (int l = 0; l < EnemySetup.Count; l++)
                    if (IOM.HeroList.ContainsKey(EnemySetup[l]))
                        enemyStack.Add(HeroUnit.Make(IOM.HeroList[EnemySetup[l]]));
                    else
                        Debug.LogError("No Hero/Enemy info ID: " + EnemySetup[l]);
            }
            else
            {
                SkyObject enemy = GM.GetObject(Enemy);

                enemyStack = enemy.Group.GetUnits();
            }

            Action dela = End;
            UIM.StartBattle(playStack, enemyStack, dela, this, rounds: Rounds);
        }

        public virtual void PlaceUnits()
        {
            UIM.OpenMenu("BattleMenu");
            UIM.Fade(false, callF: StartRound);
        }

        #endregion

        #region Main states

        protected int CurrentRound = 0;

        public virtual void StartRound()
        {
            // Effect check

//            UIM.BAS.PlayerControl = true;

            UIM.BAS.NextTurn();
        }

        public virtual void EndTurn()
        {
            if((UIM.BAS.enemyHeroItems.All(uni => uni.Visible == false) && UIM.BAS.enemyHeroItems.Any(uni => uni.bindUnit.CurrentHP > 0))
                || (UIM.BAS.playerHeroItems.All(uni => uni.Visible == false) && UIM.BAS.playerHeroItems.Any(uni => uni.bindUnit.CurrentHP > 0)))
            {
                DrawDelegate?.Invoke();
                this.End();
            }

            UIM.BAS.NextTurn();
        }

        public virtual void EndRound()
        {
            UIM.BAS.NewRound();
        }

        public virtual void UnitDead()
        {

        }

        public virtual void BattleEnd(bool success)
        {
            this.result.CallResult(this.Object, success);
            this.End();
        }

        #endregion

        #region static
        public static BattleEvent Create(string player, string enemy, ResultID res, string isl = "Player", List<string> playerStack = null, List<string> enemyStack = null, int round = 10, Action DrawDelegate = null)
        {
            BattleEvent temp = new BattleEvent();
            temp.ID = "StartBattle";

            temp.Player = player;
            temp.Enemy = enemy;
            temp.isl = isl;
            temp.PlayerSetup = playerStack;
            temp.EnemySetup = enemyStack;
            temp.Rounds = round;
            temp.result = res;
            temp.DrawDelegate = DrawDelegate;

            return temp;
        }
        #endregion
    }
}