using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;

namespace BattleActions
{

    public class BattleAction : ObjectID, ICloneable
    {
        #region properties
        string ObjectID;

        HeroInfoItem _object;
        public HeroInfoItem Object
        {
            get { return this._object; }
            set { this._object = value; }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string ID
        {
            get { return ObjectID; }
            set { this.ObjectID = value; }
        }

        public bool initialized = false;
        public string Action;
        public JSONNode param;

        public bool EnemySide = true;
        public HeroInfoItem enemy = null;
        int actCost = 1;
        #endregion

        #region Action Info

        protected string _ico = "info";
        public string Icon
        { get { return this._ico; } }

        protected string _name = "Action";
        public string ActName
        { get { return this._name; } }

        protected string _desctiprion = "info";
        public string Description
        { get { return this._desctiprion; } }

        #endregion

        #region Sub Work

        public void SafeCall(string methodName)
        {
            if (string.IsNullOrEmpty(methodName)) return;
            MethodInfo mi = this.GetType().GetMethod(methodName);
            if (mi != null)
                mi.Invoke(this, null);
            else
                Debug.LogError(ID + ": Can't find method " + methodName);
        }

        public virtual void Init()
        {
            this.initialized = true;
        }

        public virtual void CompleteAct()
        {
            this.End();
        }

        public virtual void PressedItem(HeroInfoItem item)
        {
            if(this.Object.Side == 1)
            {
                this.enemy = item;
                UIM.BAS.SelectOpponent(item);
                return;
            }

            this.enemy = item;
            UIM.BAS.SelectOpponent(item);
            if (this.CanActive())
                this.Start();
        }

        #endregion


        #region
        public GroupSkillCheck SuccessCheck;
        public GroupSkillCheck AvoidCheck;

        #endregion

        #region override functions

        public virtual bool CanActive()
        {
            if (Object == null)
            {
                Debug.LogError(this.ID + " has no Parent");
                return false;
            }

            if ((EnemySide && enemy.Side == Object.Side) || (!EnemySide && enemy.Side != enemy.Side))
                return false;

            if (Object.bindUnit.ActionPoints - actCost < 0)
                return false;

            if (Object.bindUnit.EffectCancelAction(this.ID))
                return false;

            return true;
        }

        public virtual void Start()
        {
            if (!initialized)
                this.Init();

            bool noAction = Action.Equals("");

            if (noAction && param == null)
                Action = "Play";

            Action = "";
        }

        public virtual void PrepareBattleAction()
        {

        }

        public virtual void End()
        {
            //Debug.LogError("Complete: " + ID);
            this.Object.bindUnit.EndTurn = true;
            UIM.BAS.NextTurn();
            ES.NotifySubscribers(TriggerType.ActionEventEnd.ToString(), this.ID);
        }

        public virtual string GetDescription()
        {
            return LocalizationManager.Get("test_string");
        }

        public virtual string GetActionActiveMessage()
        {
            return LocalizationManager.Get("test_string");
        }

        public virtual string GetActionAnswer()
        {
            return LocalizationManager.Get("test_string");
        }

        #endregion

        #region InitAllBattleAction
        private static Dictionary<string, Type> assets;
        public static Dictionary<string, BattleAction> packs;

        public static void initAllPacks()
        {
            assets = new Dictionary<string, Type>();
            packs = new Dictionary<string, BattleAction>();

            getAllAssets();

            BattleAction res = null;

            foreach (var key in assets.Keys)
            {
                try
                {
                    res = Activator.CreateInstance(assets[key]) as BattleAction;
                    res.ID = key;
                    res.Init();
                    packs.Add(key, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + key);
                    Debug.LogError(e.Message);
                }
            }
        }

        public static BattleAction loadBattleAction(string node, HeroInfoItem unit)
        {
            BattleAction res = null;

            if (assets == null)
            {
                getAllAssets();
            }

            if (packs.ContainsKey(node))
            {
                res = packs[node].Clone() as BattleAction;
                res.Object = unit;
                res.PrepareBattleAction();
                return res;

            }

            if (assets.ContainsKey(node))
            {
                try
                {
                    res = Activator.CreateInstance(assets[node]) as BattleAction;
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
            {
                res = packs[node].Clone() as BattleAction;
                res.Object = unit;
                res.PrepareBattleAction();
            }


            if (res == null)
            {
                Debug.LogError("Failed to load event! ID: " + node);
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
                    if (t.IsSubclassOf(typeof(BattleActions.BattleAction)))
                    {
                        string typeName = t.ToString();
                        string shortName;
                        if (typeName.IndexOf("BattleActions.") == 0)
                        {
                            shortName = typeName.Substring("BattleActions.".Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                        else if (typeName.IndexOf("BattleActions" + GM.mission + ".") == 0)
                        {
                            shortName = typeName.Substring(string.Concat("BattleActions.", GM.mission).Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                    }
            }
        }
        #endregion
    }

}