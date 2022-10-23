using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;
using BattleActions;

namespace BattleUnitAI
{
    public class BattleUnitEnemyAI : ObjectID, ICloneable
    {
        public BattleUnitEnemy parent;
        public int UpToAct;
        public HeroInfoItem _target;
        protected int SpentAction;
        protected List<string> availableActions;

        public virtual void MakeAction()
        {
            if (this.availableActions == null)
                this.availableActions = new List<string>();

            if (this.parent.bindUnit == null)
            {
                Debug.LogError("Something wrong in EnemyAI. No bind unit");
                this.CompleteAction();
            }

            this.availableActions.Clear();
            this.availableActions.AddRange(this.parent.bindUnit.actions.Where(actstr => this.parent.bindUnit.EffectCancelAction(actstr) == false).ToList());

            if (this.availableActions.Count == 0)
            {
                this.CompleteAction();
                return;
            }

            string actName;
            if (this.availableActions.Count > 1)
                actName = this.availableActions[UnityEngine.Random.Range(0, this.availableActions.Count - 1)];
            else
                actName = this.availableActions[0];



            int index = 0;
            float maxHP = 0f;
            int maxDef = 0;
            int maxAttack = 0;

            int type = (int)UnityEngine.Random.Range(0, 2);

            if (type == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if (maxHP < UIM.BAS.PlayerArmy[i].CurrentHP)
                    {
                        index = i;
                        maxHP = UIM.BAS.PlayerArmy[i].CurrentHP;
                    }
                }
            }
            else if (type == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if (maxDef < UIM.BAS.PlayerArmy[i].skills["dexterity"].Max)
                    {
                        index = i;
                        maxDef = UIM.BAS.PlayerArmy[i].skills["dexterity"].Max;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (i >= UIM.BAS.PlayerArmy.Count)
                        break;

                    if (maxAttack < UIM.BAS.PlayerArmy[i].skills["strenght"].Max)
                    {
                        index = i;
                        maxAttack = UIM.BAS.PlayerArmy[i].skills["strenght"].Max;
                    }
                }
            }

            this._target = UIM.BAS.playerHeroItems[index];

            BattleAction act = BattleAction.loadBattleAction(IOM.BattleActionInfoDic[this.parent.bindUnit.actions[0]].Name,
                this.parent);
            act.PressedItem(this._target);
            UIM.BAS.StartEnemyAction(act);
        }

        public virtual void CompleteAction()
        {
            this.parent.bindUnit.EndTurn = true;
            UIM.BAS.NextTurn();
        }

        #region InitAllBattleUnitAI
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        string ObjectID;
        public string ID
        {
            get { return ObjectID; }
            set { this.ObjectID = value; }
        }

        private static Dictionary<string, Type> assets;
        public static Dictionary<string, BattleUnitEnemyAI> packs;

        public static void initAllPacks()
        {
            assets = new Dictionary<string, Type>();
            packs = new Dictionary<string, BattleUnitEnemyAI>();

            getAllAssets();

            BattleUnitEnemyAI res = null;

            foreach (var key in assets.Keys)
            {
                try
                {
                    res = Activator.CreateInstance(assets[key]) as BattleUnitEnemyAI;
                    res.ID = key;
                    packs.Add(key, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + key);
                    Debug.LogError(e.Message);
                }
            }
        }

        public static BattleUnitEnemyAI loadBattleAction(string node)
        {
            BattleUnitEnemyAI res = null;

            if (assets == null)
            {
                getAllAssets();
            }

            if (packs.ContainsKey(node))
            {
                res = packs[node].Clone() as BattleUnitEnemyAI;
                return res;

            }

            if (assets.ContainsKey(node))
            {
                try
                {
                    res = Activator.CreateInstance(assets[node]) as BattleUnitEnemyAI;
                    res.ID = node;
                    packs.Add(node, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + node);
                    Debug.LogError(e.Message);
                }
            }

            if (packs.ContainsKey(node))
                res = packs[node].Clone() as BattleUnitEnemyAI;


            if (res == null)
            {
                Debug.LogError("Failed to load EnemyAI! ID: " + node);
                res = new BattleUnitEnemyAI();
            }

            return res;
        }

        static void getAllAssets()
        {
            Assembly[] asset = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();
            foreach (var a in asset)
            {
                foreach (var t in a.GetExportedTypes())
                    if (t.IsSubclassOf(typeof(BattleUnitAI.BattleUnitEnemyAI)))
                    {
                        string typeName = t.ToString();
                        string shortName;
                        if (typeName.IndexOf("BattleUnitAI.") == 0)
                        {
                            shortName = typeName.Substring("BattleUnitAI.".Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                    }
            }
        }
        #endregion
    }
}