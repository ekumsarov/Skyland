using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lodkod;

public class BM
{
    List<Bonfire> bonfires;
    Dictionary<int, MainBuild> mains;

    private static BM instance = null;
    public static void NewGame()
    {
        if (BM.instance != null)
            BM.instance = null;

        BM.instance = new BM();

        BM.instance.mains = new Dictionary<int, MainBuild>();
        BM.instance.bonfires = new List<Bonfire>();
    }

    public static void ActiveIsland(int islNum)
    {
    }

    #region Builds

    public static Dictionary<int, MainBuild> Mains
    {
        get { return BM.instance.mains; }
    }

    public static void InstallBuild(int islNumber, string type, bool complete = true, int lvl = 1, int CellNum = -1)
    {
        if(!IM.Islands[islNumber].HasBuild)
        {
            Debug.LogError("Where is no Center on island: " + islNumber);
            return;
        }

        if(!BM.Mains.ContainsKey(islNumber))
        {
            Debug.LogError("Cannot findoCenter on island: " + islNumber);
            return;
        }

        if (complete)
            BM.Mains[islNumber].InstallBuild(type, lvl);
        else
        {
            if(CellNum == -1)
            {
                Debug.LogError("Cell number wrong set!");
                return;
            }

            BM.Mains[islNumber].CreateBuild(type, CellNum);
        }
            
        
    }

    public static void InstallMainBuild(int islNumber, string type, bool complete = true, int lvl = 0, bool islOpened = true)
    {
        Island temp = IM.Islands[islNumber];
        temp.HasBuild = true;

        if (islOpened)
            temp.State = Island.iState.Explored;

        BuildInfo info = IOM.getBuildInfo(type, lvl);

        SceneObject temp_c = GM.instatinatePrefab("Prefabs/units/Builds/" + info.Prefab, Containers.Centers);
        MainBuild cent = temp_c.GetComponent<MainBuild>();
        //cent.transform.position = temp.ObjectPlace.position;
        BM.Mains.Add(islNumber, cent);

        cent.Side = 0;
        cent.IslandNumber = islNumber;

        if (complete)
            cent.InstallBuild(info);
        else
            cent.PlaceBuild(info);
    }
    

    public static void InstallBuild(int islNumber, List<string> types)
    {
        if(!IM.Islands[islNumber].HasBuild)
        {
            Debug.LogError("Forget set Center");
            return;
        }

        string type = "";
        int lvl = -1;
        
        foreach(var key in types)
        {
            foreach(var ch in key)
            {
                if(ch == '1' || ch == '2' || ch == '3')
                {
                    type = key.Substring(key.IndexOf(ch));
                    lvl = ch;
                    lvl -= 1;
                    break;
                }
            }

            if (type == "")
            {
                lvl = 0;
                type = key;
            }


            BM.InstallBuild(islNumber, type, complete: true, lvl: lvl);
        }

    }

    public static Builds MakeBuild(string type, int lvl)
    {
        BuildInfo binfo = IOM.BuildInfoDic[type][0];

        GameObject obj = new GameObject();

        if (binfo.type == BuildType.ProductBuild)
            obj.AddComponent<ProductBuild>();
        else if (binfo.type == BuildType.DefenseBuild)
            obj.AddComponent<DefenseBuild>();
        else
            obj.AddComponent<SpecialBuild>();

        Builds temp = obj.GetComponent<Builds>();
        temp.InitBuild(binfo);

        return temp;
    }
    #endregion
    #region Bonfire
    public static List<Bonfire> Bonfires
    {
        get { return BM.instance.bonfires; }
    }

    public static bool HasBonfire(int islandNumber)
    {
        return BM.Bonfires.Any(bon => bon.IslandNumber == islandNumber);
    }

    public static void RemoveBonfire(int islandNumber)
    {
        Bonfire center = BM.Bonfires.FirstOrDefault(build => build.IslandNumber == islandNumber);
        if (center != null)
        {
            BM.Bonfires.Remove(center);
            GameObject.Destroy(center.gameObject);
        }
    }

    public static void DeactiveBonfire()
    {
        foreach(var bon in BM.instance.bonfires)
        {
            bon.Active = false;
        }
    }

    public static int ActiveBonfire
    {
        get
        {
            foreach (var bon in BM.instance.bonfires)
            {
                if (bon.Active)
                    return bon.IslandNumber;
            }
            return -1;
        }
        
    }

    public static void DeactiveBonfires()
    {
        foreach(var bon in BM.instance.bonfires)
        {
            bon.Active = false;
        }
    }
    #endregion
}
