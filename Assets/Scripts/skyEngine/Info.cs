using Lodkod;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class IOM // Info Manager
{
    private static IOM instance = null;
    public Dictionary<string, List<BuildInfo>> buildInfo;
    public Dictionary<BuildType, List<BuildInfo>> avaliableBuild;
    public Dictionary<string, BattleActionInfo> battleActionInfo;
    public Dictionary<string, BattleEffectInfo> battleEffectInfo;
    public Dictionary<string, BattleUnitInfo> battleUnitInfo;
    public Dictionary<string, HeroInfo> heroList;
    public Dictionary<string, HeroInfo> playerPartyList;
    public Dictionary<string, LootItem> lootItemList;

    public static void NewGame(JSONNode slotInfo)
    {
        if (IOM.instance != null)
        {
            IOM.instance.buildInfo.Clear();
            IOM.instance = null;
        }

        IOM.instance = new IOM();
        IOM.instance.buildInfo = new Dictionary<string, List<BuildInfo>>();
        IOM.instance.avaliableBuild = new Dictionary<BuildType, List<BuildInfo>>();
        IOM.instance.battleActionInfo = new Dictionary<string, BattleActionInfo>();
        IOM.instance.battleEffectInfo = new Dictionary<string, BattleEffectInfo>();
        IOM.instance.battleUnitInfo = new Dictionary<string, BattleUnitInfo>();
        IOM.instance.heroList = new Dictionary<string, HeroInfo>();
        IOM.instance.playerPartyList = new Dictionary<string, HeroInfo>();
        IOM.instance.lootItemList = new Dictionary<string, LootItem>();

        TextAsset pathStringAvailableBuildInfo = Resources.Load("missions/config/BuildInfo") as TextAsset;
        TextAsset pathStringBuildInfo = Resources.Load("missions/config/BuildInfo") as TextAsset;
        TextAsset pathStringBuildInfoAdd = Resources.Load("missions/mission" + GM.mission + "/config/BuildInfoAdd") as TextAsset;
        TextAsset pathStringBattleUnitInfo = Resources.Load("missions/config/WarUnitInfo") as TextAsset;
        TextAsset pathStringHeroInfo = Resources.Load("missions/config/HeroesInfo") as TextAsset;
        TextAsset pathStringHeroInfoAdd = Resources.Load("missions/mission" + GM.mission + "/config/HeroesInfoAdd") as TextAsset;
        TextAsset pathStringBattleAction = Resources.Load("missions/config/BattleActionInfo") as TextAsset;
        TextAsset pathStringBattleEffect = Resources.Load("missions/config/BattleEffectInfo") as TextAsset;
        TextAsset pathStringBattleEffectAdd = Resources.Load("missions/mission" + GM.mission + "/config/BattleEffectAdd") as TextAsset;
        TextAsset pathStringBattleUnitAdd = Resources.Load("missions/mission" + GM.mission + "/config/BattleUnitAdd") as TextAsset;
        TextAsset pathStringBattleActionAdd = Resources.Load("missions/mission" + GM.mission + "/config/BattleActionAdd") as TextAsset;
        TextAsset pathStringLootItem = Resources.Load("missions/config/LootItemsInfo") as TextAsset;

        /*
         * Build Info
         */
        if (pathStringBuildInfo != null && !pathStringBuildInfo.text.Equals(""))
        {
            JSONNode BIDoc = JSON.Parse(pathStringBuildInfo.text);

            JSONNode buildArray = BIDoc["Builds"];
            JSONArray buttonArray;

            List<BuildInfo> tList;
            BuildInfo temp;

            foreach (var key in buildArray.Keys)
            {
                buttonArray = buildArray[key].AsArray;
                tList = new List<BuildInfo>();

                IOM.instance.buildInfo.Add(key, tList);

                for (int k = 0; k < buttonArray.Count; k++)
                {
                    temp = BuildInfo.Make(key, k, buttonArray[k]);
                    tList.Add(temp);
                }
            }
        }
        else
            Debug.LogError("Null string by: pathStringBuildInfo");

        /*
         * Build Add Info
         */
        if (pathStringBuildInfoAdd != null && !pathStringBuildInfoAdd.text.Equals(""))
        {
            JSONNode BIDoc = JSON.Parse(pathStringBuildInfoAdd.text);

            JSONNode buildArray = BIDoc["Builds"];
            JSONArray buttonArray;

            List<BuildInfo> tList;
            BuildInfo temp;

            foreach (var key in buildArray.Keys)
            {
                buttonArray = buildArray[key].AsArray;
                tList = new List<BuildInfo>();

                IOM.instance.buildInfo.Add(key, tList);

                for (int k = 0; k < buttonArray.Count; k++)
                {
                    temp = BuildInfo.Make(key, k, buttonArray[k]);
                    tList.Add(temp);
                }
            }
        }
        else
            Debug.LogError("Null string by: pathStringBuildInfoAdd");

        /*
         * Avaliable Build Info
         */
        IOM.instance.avaliableBuild.Add(BuildType.DefenseBuild, new List<BuildInfo>());
        IOM.instance.avaliableBuild.Add(BuildType.MainBuild, new List<BuildInfo>());
        IOM.instance.avaliableBuild.Add(BuildType.MainSpecial, new List<BuildInfo>());
        IOM.instance.avaliableBuild.Add(BuildType.ProductBuild, new List<BuildInfo>());
        IOM.instance.avaliableBuild.Add(BuildType.SpecialBuild, new List<BuildInfo>());

        JSONNode avaliableBuild = slotInfo["AvaliableBuilds"];
        foreach (var key in avaliableBuild.Keys)
        {
            for (int i = 0; i < avaliableBuild[key].AsInt; i++)
            {
                IOM.instance.avaliableBuild[IOM.instance.buildInfo[key][i].type].Add(IOM.instance.buildInfo[key][i]);
            }
        }

        /*
         * Battle Action Info
         */
        if (pathStringBattleAction != null && !pathStringBattleAction.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringBattleAction.text);
            JSONNode battleArray = BADoc["Actions"];
            BattleActionInfo btemp;

            foreach (var key in battleArray.Keys)
            {
                btemp = BattleActionInfo.Make(key, battleArray[key]);
                IOM.instance.battleActionInfo.Add(key, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleAction");

        /*
         * Battle Effect Info
         */
        if (pathStringBattleEffect != null && !pathStringBattleEffect.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringBattleEffect.text);
            JSONNode battleArray = BADoc["Effects"];
            BattleEffectInfo btemp;

            foreach (var key in battleArray.Keys)
            {
                btemp = BattleEffectInfo.Make(key, battleArray[key]);
                IOM.instance.battleEffectInfo.Add(key, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleAction");

        if (pathStringBattleEffectAdd != null && !pathStringBattleEffectAdd.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringBattleEffectAdd.text);
            JSONNode battleArray = BADoc["Effects"];
            BattleEffectInfo btemp;

            foreach (var key in battleArray.Keys)
            {
                btemp = BattleEffectInfo.Make(key, battleArray[key]);
                IOM.instance.battleEffectInfo.Add(key, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleAction");

        /*
         * Battle Action Add Info
         */
        if (pathStringBattleActionAdd != null && !pathStringBattleActionAdd.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringBattleActionAdd.text);
            JSONNode battleArray = BADoc["Actions"];
            BattleActionInfo btemp;

            foreach (var key in battleArray.Keys)
            {
                btemp = BattleActionInfo.Make(key, battleArray[key]);
                IOM.instance.battleActionInfo.Add(key, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleActionAdd");

        /*
         * Battle Unit Info
         */
        if (pathStringBattleUnitInfo != null && !pathStringBattleUnitInfo.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringBattleUnitInfo.text);
            JSONArray battleArray = BADoc["Units"].AsArray;
            BattleUnitInfo btemp;

            for (int i = 0; i < battleArray.Count; i++)
            {
                btemp = BattleUnitInfo.Make(battleArray[i]["BattleUnitType"].Value, battleArray[i]);
                IOM.instance.battleUnitInfo.Add(battleArray[i]["BattleUnitType"].Value, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleActionAdd");


        /*
         * Hero list
         */
        HeroInfo htemp;
        JSONArray arr = slotInfo["HeroList"].AsArray;
        for(int i = 0; i < arr.Count; i++)
        {
            htemp = HeroInfo.Make(arr[i]["BattleUnitType"].Value, arr[i]);
            IOM.instance.playerPartyList.Add(arr[i]["BattleUnitType"].Value, htemp);
        }

        if (pathStringHeroInfo != null && !pathStringHeroInfo.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringHeroInfo.text);
            JSONArray battleArray = BADoc["Units"].AsArray;
            HeroInfo btemp;

            for (int i = 0; i < battleArray.Count; i++)
            {
                btemp = HeroInfo.Make(battleArray[i]["BattleUnitType"].Value, battleArray[i]);
                IOM.instance.heroList.Add(battleArray[i]["BattleUnitType"].Value, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleActionAdd");

        if (pathStringHeroInfoAdd != null && !pathStringHeroInfoAdd.text.Equals(""))
        {
            JSONNode BADoc = JSON.Parse(pathStringHeroInfoAdd.text);
            JSONArray battleArray = BADoc["Units"].AsArray;
            HeroInfo btemp;

            for (int i = 0; i < battleArray.Count; i++)
            {
                btemp = HeroInfo.Make(battleArray[i]["BattleUnitType"].Value, battleArray[i]);
                IOM.instance.heroList.Add(battleArray[i]["BattleUnitType"].Value, btemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleActionAdd");



        /*
         * Loot Item Info
         */
        if (pathStringLootItem != null && !pathStringLootItem.text.Equals(""))
        {
            JSONNode LIDoc = JSON.Parse(pathStringLootItem.text);
            JSONArray lootArray = LIDoc["LootItemInfo"].AsArray;
            LootItem ltemp;

            for (int i = 0; i < lootArray.Count; i++)
            {
                ltemp = LootItem.Make(lootArray[i]["ID"].Value, lootArray[i]);
                IOM.instance.lootItemList.Add(lootArray[i]["ID"].Value, ltemp);
            }
        }
        else
            Debug.LogError("Null string by: pathStringBattleAction");
    }

    /*public static List<BuildInfo> getBuildInfo(int type)
    {
        string Type = ((BuildType)type).ToString();
        List<BuildInfo> temp = new List<BuildInfo>();

        for (int i = 0; i < IOM.instance.buildInfo[Type].Count; i++)
        {
            List<BuildInfo> i_temp = new List<BuildInfo>();
            temp.Add(IOM.instance.buildInfo[Type][i]);
        }

        return temp;
    }*/

    public static BuildInfo getBuildInfo(string type, int level = 0)
    {
        if (!IOM.instance.buildInfo.ContainsKey(type))
        {
            Debug.LogError("No such build ID: " + type);
            return null;
        }
        if (IOM.instance.buildInfo[type].Count <= level)
        {
            Debug.LogError("No level " + level + "in build ID: " + type);
            return null;
        }
        return IOM.instance.buildInfo[type][level];
    }

    public static List<BuildInfo> GetAvaliableBuildForMain()
    {
        List<BuildInfo> temp = new List<BuildInfo>();
        foreach (var inf in IOM.instance.avaliableBuild)
        {
            if (inf.Key != BuildType.MainBuild && inf.Key != BuildType.MainSpecial && inf.Key != BuildType.SpecialBuild)
                temp.AddRange(inf.Value);
        }
        return temp;
    }

    public static string GetBuilfDescription(string ID, int level = 0, bool withcost = false)
    {
        if (!IOM.instance.buildInfo.ContainsKey(ID))
        {
            Debug.LogError("No such build ID: " + ID);
            return "";
        }

        // TODO: need complete patterns
        if (IOM.instance.buildInfo[ID][level].type == BuildType.MainBuild)
            return LocalizationManager.Get("MainBuildPattern", IOM.instance.buildInfo[ID][level].Special["OpenCells"].AsInt);
        else if (IOM.instance.buildInfo[ID][level].type == BuildType.ProductBuild)
        {
            BuildInfo binfo = IOM.instance.buildInfo[ID][level];
            string des = LocalizationManager.Get(IOM.instance.buildInfo[ID][level].Description) + "\n\n" + LocalizationManager.Get("ProductBuildPattern", binfo.Special["Production"].AsInt, SM.Stats[binfo.Special["Type"].Value].Icon);

            if (withcost)
            {
                des += LocalizationManager.Get("CostTemplate") + " ";
                foreach (var res in binfo.Cost)
                {
                    if (res.amount == 0)
                        continue;

                    if (SM.Stats[res.type.ToString()].Count >= res.amount)
                        des += LocalizationManager.Mark(Mark.green, res.amount);
                    else
                        des += LocalizationManager.Mark(Mark.red, res.amount);

                    des += " <icon=" + SM.Stats[res.type.ToString()].Icon + "/>  ";
                }
            }

            des = des.Replace("\r", "");
            return des;
        }
        else if (IOM.instance.buildInfo[ID][level].type == BuildType.DefenseBuild)
        {
            BuildInfo binfo = IOM.instance.buildInfo[ID][level];
            string des = IOM.instance.buildInfo[ID][level].Description + "\n\n" + LocalizationManager.Get("DefenseBuildPattern");

            if (withcost)
            {
                des += LocalizationManager.Get("CostTemplate") + " ";
                foreach (var res in binfo.Cost)
                {
                    if (res.amount == 0)
                        continue;

                    if (SM.Stats[res.type.ToString()].Count >= res.amount)
                        des += LocalizationManager.Mark(Mark.green, res.amount);
                    else
                        des += LocalizationManager.Mark(Mark.red, res.amount);

                    des += " <icon=" + SM.Stats[res.type.ToString()].Icon + "/>  ";
                }
            }
            des = des.Replace("\r", "");
            return des;
        }
        else if (IOM.instance.buildInfo[ID][level].type == BuildType.SpecialBuild)
            return LocalizationManager.Get("SpecialBuildPattern", IOM.instance.buildInfo[ID][level].Description);

        return "";
    }

    public static BuildInfo GetSpecialBuildInfo(string type, int level = 0)
    {
        if (IOM.instance.buildInfo.ContainsKey(type) == false)
        {
            Debug.LogError("No such special build ID: " + type);
            return null;
        }
        return IOM.instance.buildInfo[type][level];
    }

    public static Dictionary<string, List<BuildInfo>> BuildInfoDic
    {
        get { return IOM.instance.buildInfo; }
    }

    public static Dictionary<string, BattleUnitInfo> BattleUnitInfoDic
    {
        get { return IOM.instance.battleUnitInfo; }
    }

    public static Dictionary<string, BattleActionInfo> BattleActionInfoDic
    {
        get { return IOM.instance.battleActionInfo; }
    }

    public static Dictionary<string, BattleEffectInfo> BattleEffectInfoDic
    {
        get { return IOM.instance.battleEffectInfo; }
    }

    public static Dictionary<string, HeroInfo> HeroList
    {
        get { return IOM.instance.heroList; }
    }

    public static Dictionary<string, HeroInfo> PlayerPartyList
    {
        get { return IOM.instance.playerPartyList; }
    }

    public static LootItem GetLootItem(string name)
    {
        if(!IOM.instance.lootItemList.ContainsKey(name))
        {
            Debug.LogError("No such loot item ID: " + name);
            return null;
        }

        return IOM.instance.lootItemList[name];
    }
}
