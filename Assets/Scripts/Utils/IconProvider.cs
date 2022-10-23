
using System;
using System.Collections.Generic;
using UnityEngine;

public class TextIconProvider
{
    static SpriteProvider _provider;

    public static Sprite GetIcon(string name)
    {
        Init();

        return _provider.Get(name);
    }

    public static void Init()
    {
        if (_provider == null)
            _provider = new SpriteProvider("images/text_icons", "money", "text_");
    }
}

public class GuiIconProvider
{
    static SpriteProvider _provider;

    public static Sprite GetIcon(string name)
    {
        Init();

        return _provider.Get(name);
    }

    public static void Init()
    {
        if (_provider == null)
            _provider = new SpriteProvider("images/GUI", "BaseIcon");
    }
}

public class BattleIconProvider
{
    static SpriteProvider _provider;

    public static Sprite GetIcon(string name)
    {
        Init();

        return _provider.Get(name);
    }

    public static void Init()
    {
        if (_provider == null)
            _provider = new SpriteProvider("images/GUI", "info");
    }
}

public class SpriteProvider
{
    string folder;
    string prefix;
    string default_key;

    bool isMultiscale;
    bool isLazy;

    Dictionary<string, Sprite> _map = null;

    //List<ResourcesLinker> linkers;

    public static void InitAll()
    {
        TextIconProvider.Init();
    }

    public SpriteProvider(string f, string d, string p = null, bool multiscale = false, bool lazy = false)
    {
        folder = f;
        prefix = p;
        default_key = d;
        isMultiscale = multiscale;
        isLazy = lazy;
        //linkers = new List<ResourcesLinker>();
        if (!isLazy)
            _map = InitializeMap();
    }

    /*public void AddLinker(string name)
    {
        string filepath = "linkers/" + name;
        if (isMultiscale)
            filepath = filepath + GameScaler.CurrentPostfix;
        UnityEngine.Object o = Resources.Load(filepath);
        linkers.Add(o as ResourcesLinker);
    }*/

    public Sprite Get(string name)
    {
        try
        {
/*            foreach (var linker in linkers)
            {
                Sprite res = linker.FindByName(name);
                if (res != null)
                    return res;
            }*/

            if (isLazy)
            {
                Sprite res = null;
                string path = folder + "/" + name;
 //               if (isMultiscale)
 //                  path += GameScaler.CurrentPostfix;

                res = Resources.Load<Sprite>(path);
                return res;
            }
            else
            {

                if (_map == null)
                    _map = InitializeMap();
                if (_map.ContainsKey(name))
                    return _map[name];
                return _map[default_key];
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("No {0} sprite in dictionary!", name);
            Debug.LogError(e.Message);
            return null;
        }
    }

    private Dictionary<string, Sprite> InitializeMap()
    {
        Dictionary<string, Sprite> res = new Dictionary<string, Sprite>();
        List<Sprite> icons = new List<Sprite>(Resources.LoadAll<Sprite>(folder));
        string postfix = null;

        foreach (var sprite in icons)
        {
            string key = sprite.name;
            if (postfix != null)
            {
                if (!key.EndsWith(postfix))
                {
                    continue;
                }
                key = key.Remove(key.Length - postfix.Length, postfix.Length);
            }

            if (prefix != null && !prefix.Equals(""))
            {
                if (!sprite.name.StartsWith(prefix))
                {
                    Resources.UnloadAsset(sprite);
                    continue;
                }

                key = sprite.name.Remove(0, prefix.Length);
            }
            res.Add(key, sprite);
        }
        icons.Clear();
        return res;
    }
}