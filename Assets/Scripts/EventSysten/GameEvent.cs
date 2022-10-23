using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using Lodkod;
using System.Reflection;

namespace GameEvents
{
        
    public class GameEvent : ObjectID, ICloneable
    {

        string ObjectID;

        protected SkyObject _object;
        public virtual SkyObject Object
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
        
        public bool afterRemove = true;
        public bool FromChat = false;
        public bool Simple = true;
        public bool initialized = false;
        public string Action = "";
        public JSONNode param;
        public List<Condition> Conditions;

        public virtual bool CanActive()
        {
            if (Object == null)
            {
                Debug.LogError(this.ID + " has no Parent");
                return false;
            }

            if (Conditions == null)
                return true;

            bool success = true;
            for(int i = 0; i < this.Conditions.Count; i++)
            {
                success = this.Conditions[i].Available;
                if (success == false)
                    break;
            }
            return success;
        }

        public virtual void Start()
        {
            if (!initialized)
                this.Init();

            bool noAction = Action.Equals("");

            if (noAction && param == null)
                Action = "Play";
                
            MethodInfo mi = this.GetType().GetMethod(Action);
            if (mi != null)
                mi.Invoke(this, null);
            else if (param != null)
                SetParam(param);
            else
                Debug.LogError(ID + ": Not Implemented function " + Action);

            Action = "";
        }

        public virtual void SetParam(JSONNode param)
        {
            foreach(var fd in param.Keys)
            {
                FieldInfo mi = this.GetType().GetField(fd);
                if (mi != null)
                    mi.SetValue(this, ld.GetJSONValue(mi, param[fd]));
                
            }

            if (param["CallID"] != null)
            {
                Object.CallAction(param["CallID"].Value);
            }
            else if(param["Action"] != null)
            {
                MethodInfo mi = this.GetType().GetMethod(param["Action"]);
                if (mi != null)
                    mi.Invoke(this, null);

                return;
            }
            else
            {
                MethodInfo mi = this.GetType().GetMethod(Action);
                if (mi != null)
                {
                    mi.Invoke(this, null);
                    return;
                }
                else
                    Debug.LogError("Not found CallID in param set");

                
            }

            param = null;
            End();
        }

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
            this.Conditions = new List<Condition>();
        }

        public virtual void End()
        {
            //Debug.LogError("Complete: " + ID);
            ES.NotifySubscribers(TriggerType.ActionEventEnd.ToString(), this.ID);

            GEM.ContinueEvent(this);
        }
        
        public virtual void InitEvent()
        {
            afterRemove = true;
            FromChat = false;
            Simple = true;
        }

        public virtual void PrepareEvent(JSONNode node)
        {
            Action = "";
            if (node["Action"] != null)
                Action = node["Action"].Value;

            param = null;
            if (node["param"] != null)
                param = node["param"];
        }

        public virtual void PrepareEvent()
        {
            Action = "";
            param = null;
        }

        public SkyObject GetObject(string id)
        {
            if (id.Equals("self"))
                return Object;
            else if (id.Equals("Manager"))
                return GEM.instance;
            else
                return GM.GetObject(id);
        }

        #region helping function

        protected Vector3 GetVector3FromNode(JSONNode node)
        {
            if(node["y"] == null)
                return new Vector3(node["x"].AsFloat, node["x"].AsFloat, node["x"].AsFloat);
            else if(node["z"] == null)
                return new Vector3(node["x"].AsFloat, node["y"].AsFloat, 0f);
            else
                return new Vector3(node["x"].AsFloat, node["y"].AsFloat, node["z"].AsFloat);
        }

        protected void MakeConditions(JSONArray node)
        {
            if (this.Conditions == null)
                this.Conditions = new List<Condition>();

            for(int i = 0; i < node.Count; i++)
            {
                if (node[i]["Type"].Value.Equals("Stat"))
                    this.Conditions.Add(StatCondition.Make(node[i]));
                else if (node[i]["Type"].Value.Equals("Flag"))
                    this.Conditions.Add(FlagCondition.Make(node[i]));
                else if (node[i]["Type"].Value.Equals("Loot"))
                    this.Conditions.Add(LootCondition.Make(node[i]));
                else if (node[i]["Type"].Value.Equals("Quest"))
                    this.Conditions.Add(QuestCondition.Make(node[i]));
                else if (node[i]["Type"].Value.Equals("Daypart"))
                    this.Conditions.Add(DaypartCondition.Make(node[i]));
            }
        }

        protected Condition MakeCondition(JSONNode node)
        {
            if (node["Type"].Value.Equals("Stat"))
                return StatCondition.Make(node);
            else if (node["Type"].Value.Equals("Flag"))
                return FlagCondition.Make(node);
            else if (node["Type"].Value.Equals("Loot"))
                return LootCondition.Make(node);
            else if (node["Type"].Value.Equals("Quest"))
                return QuestCondition.Make(node);
            else if (node["Type"].Value.Equals("Daypart"))
                return DaypartCondition.Make(node);
            else
                return new Condition();

        }

        public GameEvent AddCondition(Condition condition)
        {
            if (this.Conditions == null)
                this.Conditions = new List<Condition>();

            this.Conditions.Add(condition);

            return this;
        }

        #endregion

        #region InitAllGameEvents
        private static Dictionary<string, Type> assets;
        public static Dictionary<string, GameEvent> packs;

        public static void initAllPacks()
        {
            assets = new Dictionary<string, Type>();
            packs = new Dictionary<string, GameEvent>();

            getAllAssets();

            GameEvent res = null;

            foreach (var key in assets.Keys)
            {
                try
                {
                    res = Activator.CreateInstance(assets[key]) as GameEvent;
                    res.ID = key;
                    res.InitEvent();
                    packs.Add(key, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + key);
                    Debug.LogError(e.Message);
                }
            }
        }
        

        public static GameEvent loadEvent(JSONNode node)
        {
            GameEvent res = null;
            string id = "";

            if (node["Event"] != null)
                id = node["Event"].Value;
            else
                id = node["Base"].Value;

            if (assets == null)
            {
                getAllAssets();
            }

            if (packs.ContainsKey(id))
            {
                res = packs[id].Clone() as GameEvent;
                res.PrepareEvent(node);
                return res;

            }

            if (assets.ContainsKey(id))
            {
                try
                {
                    res = Activator.CreateInstance(assets[id]) as GameEvent;
                    res.ID = id;
                    packs.Add(id, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + id);
                    Debug.LogError(e.Message);
                }
            }

            if (packs.ContainsKey(id))
            {
                res = packs[id].Clone() as GameEvent;
                res.PrepareEvent(node);
            }
                

            if (res == null)
            {
                Debug.LogError("Failed to load event! ID: " + id);
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
                    if (t.IsSubclassOf(typeof(GameEvents.GameEvent)))
                    {
                        string typeName = t.ToString();
                        string shortName;
                        if (typeName.IndexOf("GameEvents.") == 0)
                        {
                            shortName = typeName.Substring("GameEvents.".Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                        else if (typeName.IndexOf("GameEvents" + GM.mission + ".") == 0)
                        {
                            shortName = typeName.Substring(string.Concat("GameEvents.", GM.mission).Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                    }
            }
                
            
        }
        #endregion
    }

}