using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lodkod;
using System.IO;
using System.Reflection;
using System.Linq;
using SimpleJSON;

namespace BuildTypes
{
    public class BuildInstance
    {
        protected BuildInfo _info;
        protected BuildCell _parent;
        protected Subscriber _subscriber;
        protected float _buildTime;

        protected virtual void InitializeBuild(BuildCell icon, BuildInfo info)
        {

        }

        public virtual void Improve(float delta)
        {
            this._buildTime -= delta;
            if (this._buildTime <= 0)
                this.CompleteBuild();
        }

        public virtual void CompleteBuild()
        {

        }

        public virtual void Upgrade()
        {

        }

        public virtual void StopProduction()
        {

        }

        public virtual void ActivateProduction()
        {

        }


        #region InitAllBuildTypes
        public static BuildInstance GetBuild(BuildCell parent, BuildInfo info)
        {
            BuildInstance build = BuildInstance.LoadBuildTypes(info.Name);
            if (build == null)
                build = BuildInstance.LoadBuildTypes(info.type.ToString());

            if (build != null)
                build.InitializeBuild(parent, info);

            return build;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        
        private static Dictionary<string, Type> assets;
        public static Dictionary<string, BuildInstance> packs;

        public static void InitAllPacks()
        {
            assets = new Dictionary<string, Type>();
            packs = new Dictionary<string, BuildInstance>();

            GetAllAssets();

            BuildInstance res = null;

            foreach (var key in assets.Keys)
            {
                try
                {
                    res = Activator.CreateInstance(assets[key]) as BuildInstance;
                    packs.Add(key, res);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error reading script " + key);
                    Debug.LogError(e.Message);
                }
            }
        }

        public static BuildInstance LoadBuildTypes(string node)
        {
            BuildInstance res = null;

            if (assets == null)
            {
                GetAllAssets();
            }

            if (packs.ContainsKey(node))
            {
                res = packs[node].Clone() as BuildInstance;
                return res;

            }

            if (assets.ContainsKey(node))
            {
                try
                {
                    res = Activator.CreateInstance(assets[node]) as BuildInstance;
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
                res = packs[node].Clone() as BuildInstance;
            }


            if (res == null)
            {
                Debug.LogError("Failed to load build! ID: " + node);
            }

            return res;
        }

        static void GetAllAssets()
        {
            Assembly[] asset = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();
            foreach (var a in asset)
            {
                foreach (var t in a.GetExportedTypes())
                    if (t.IsSubclassOf(typeof(BuildInstance)))
                    {
                        string typeName = t.ToString();
                        string shortName;
                        if (typeName.IndexOf("BuildTypes.") == 0)
                        {
                            shortName = typeName.Substring("BuildTypes.".Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                        else if (typeName.IndexOf("BuildTypes" + GM.mission + ".") == 0)
                        {
                            shortName = typeName.Substring(string.Concat("BuildTypes.", GM.mission).Length);
                            Debug.Log(shortName);
                            assets[shortName] = t;
                        }
                    }
            }
        }
        #endregion
    }
}