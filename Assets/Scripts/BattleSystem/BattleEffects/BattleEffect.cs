using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;

namespace BattleEffects
{

    public class BattleEffect : ObjectID, ICloneable
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

        protected bool initialized = false;
        protected BattleEffectInfo data;

        protected bool turnOn = true;
        protected int _turnsCount = -1;
        protected int _turns = -1;
        public int Turns
        {
            get { return this._turnsCount; }
            set
            {
                this._turnsCount = value;
                if (this._turnsCount <= 0 && turnOn)
                    this.DestroyEffect();
            }
        }

        public bool EffectDestroyed = false;
        #endregion

        #region Effect Info

        protected string _ico = "info";
        public string Icon
        { get { return this._ico; } }

        protected string _desctiprion = "info";
        public string Description
        { get { return this._desctiprion; } }

        protected List<string> _actionsToCancel;
        public bool CancelAction(string action)
        {
            if (this._actionsToCancel == null)
                return false;

            return this._actionsToCancel.Any(act => act.Equals(action));
        }

        #endregion

        #region Sub Work

        public virtual void Init()
        {
            this._actionsToCancel = new List<string>();

            if(IOM.BattleEffectInfoDic.ContainsKey(this.ID))
            {
                this.data = IOM.BattleEffectInfoDic[this.ID];

                if (this.data.actToCancel != null && this.data.actToCancel.Count > 0)
                    this._actionsToCancel.AddRange(this.data.actToCancel);

                this.initialized = true;
            }
        }

        #endregion

        #region override functions

        public virtual void Start()
        {
        }

        public virtual bool CanApplyEffect()
        { return true; }

        public virtual void PrepareBattleEffect()
        {
        }

        public virtual void TurnStart()
        { }

        public virtual void TurnEnd()
        {
            if (this.EffectDestroyed)
                return;

            if(this.turnOn)
            {
                this.Turns -= 1;
                if (this.Turns <= 0)
                    this.DestroyEffect();
            }
        }

        public virtual void DestroyEffect()
        {
            ES.NotifySubscribers(TriggerType.EffectDestroyed.ToString(), this.ID);
            this.EffectDestroyed = true;
        }

        public virtual string GetDescription()
        {
            return LocalizationManager.Get("test_string");
        }

        public virtual string GetEffectActiveMessage()
        {
            return LocalizationManager.Get("test_string");
        }

        #endregion

        #region InitAllBattleEffects
        private static Dictionary<string, Type> assets;
        public static Dictionary<string, BattleEffect> packs;

        public static void initAllPacks()
        {
            assets = new Dictionary<string, Type>();
            packs = new Dictionary<string, BattleEffect>();

            getAllAssets();

            BattleEffect res = null;

            foreach (var key in assets.Keys)
            {
                try
                {
                    res = Activator.CreateInstance(assets[key]) as BattleEffect;
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

        public static BattleEffect loadBattleEffect(string node, HeroInfoItem unit)
        {
            BattleEffect res = null;

            if (assets == null)
            {
                getAllAssets();
            }

            if (packs.ContainsKey(node))
            {
                res = packs[node].Clone() as BattleEffect;
                res.Object = unit;
                res.PrepareBattleEffect();
                return res;

            }

            if (assets.ContainsKey(node))
            {
                try
                {
                    res = Activator.CreateInstance(assets[node]) as BattleEffect;
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
                res = packs[node].Clone() as BattleEffect;
                res.Object = unit;
                res.PrepareBattleEffect();
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
                    if (t.IsSubclassOf(typeof(BattleEffects.BattleEffect)))
                    {
                        string typeName = t.ToString();
                        string shortName;
                        if (typeName.IndexOf("BattleEffects.") == 0)
                        {
                            shortName = typeName.Substring("BattleEffects.".Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                        else if (typeName.IndexOf("BattleEffects" + GM.mission + ".") == 0)
                        {
                            shortName = typeName.Substring(string.Concat("BattleEffects.", GM.mission).Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                    }
            }
        }
        #endregion
    }

}