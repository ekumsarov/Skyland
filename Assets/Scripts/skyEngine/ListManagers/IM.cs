using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using Lodkod;

public class IM {

    List<Island> _islands; // стек островов
    private static IM instance = new IM();
    private bool CompleteWeb;
    private int _actIsland;
    private Vector2 _mapsize;

    public static void NewGame()
    {
        if (IM.instance != null)
            IM.instance = null;

        IM.instance = new IM();
        IM.instance._islands = new List<Island>();

        IM.instance.CompleteWeb = false;

        Island[] temps = GameObject.Find("islands").GetComponentsInChildren<Island>();
        for(int i = 0; i < temps.Length; i++)
        {
            temps[i].HardSet();
            temps[i].IslandNumber = i;
            temps[i].initIsland();
            IM.instance._islands.Add(temps[i]);
        }
    }

    public static List<Island> Islands
    {
        get { return IM.instance._islands; }
    }

    public static void IslandActions(int islNum)
    {
        if (!IM.instance.CompleteWeb)
            return;

        IM.instance._islands[islNum].State = Island.iState.Active;

        List<int> inConnection = IM.instance._islands[islNum].IslandConnection;

        foreach (int temp in inConnection)
        {
            if (IM.instance._islands[temp].State == Island.iState.Invisible && !SM.CheckFlag("DisableAfterIslandOpen"))
                IM.instance._islands[temp].State = Island.iState.Unexplored;
            else if (IM.instance._islands[temp].State == Island.iState.Active)
                IM.instance._islands[temp].State = Island.iState.Explored;
        }

        
    }

    public static Island GerRandomIsland(bool hasResource = false, int size = 0, bool hasObject = false, bool hasBuild = false)
    {
        if (hasBuild)
            hasObject = true;

        List<Island> temp = IM.Islands.Where(isl => isl.HasResource == hasResource && isl.HasObject == hasObject && isl.HasBuild == hasBuild).ToList();
        if (temp.Count == 0)
        {
            Debug.LogError("Can't find random Island");
            return null;
        }

        return temp[UnityEngine.Random.Range(0, temp.Count-1)];
    }


    public static Island NearstIsland(Island island, bool hasResource = false, bool hasObject = false, bool hasBuild = false)
    {
        float distance = 1000000;
        Island need = null;

        if (hasBuild)
            hasObject = true;

        List<Island> list = IM.Islands.Where(isl => isl.HasResource == hasResource && isl.HasObject == hasObject && isl.HasBuild == hasBuild).ToList();
        foreach (Island temp in list)
        {
            if (temp == island)
                continue;


            float dis = Vector2.Distance(island.position, temp.position);
            if (dis < distance)
            {
                distance = dis;
                need = temp;
            }
        }

        if (need == null)
        {
            Debug.LogError("Can't find nearst island");
            return null;
        }

        return need;
    }

    public static Island NearstIsland(int islandNum, bool hasResource = false, bool hasObject = false, bool hasBuild = false)
    {
        float distance = 1000000;
        Island need = null;
        Island island = IM.Islands[islandNum];

        if (hasBuild)
            hasObject = true;

        List<Island> list = IM.Islands.Where(isl => isl.HasResource == hasResource && isl.HasObject == hasObject && isl.HasBuild == hasBuild).ToList();
        foreach (Island temp in list)
        {
            if (temp == island)
                continue;


            float dis = Vector2.Distance(island.position, temp.position);
            if (dis < distance)
            {
                distance = dis;
                need = temp;
            }
        }

        if (need == null)
        {
            Debug.LogError("Can't find nearst island");
            return null;
        }

        return need;
    }

    public static Island FurtherIsland(Island island, bool hasResource = false, bool hasObject = false, bool hasBuild = false)
    {
        float distance = 0;
        Island need = null;

        if (hasBuild)
            hasObject = true;

        List<Island> list = IM.Islands.Where(isl => isl.HasResource == hasResource && isl.HasObject == hasObject && isl.HasBuild == hasBuild).ToList();
        foreach (Island temp in list)
        {
            if (temp == island)
                continue;


            float dis = Vector2.Distance(island.position, temp.position);
            if (dis > distance)
            {
                distance = dis;
                need = temp;
            }
        }

        if (need == null)
        {
            Debug.LogError("Can't find Further island");
            return null;
        }

        return need;
    }

    /*public static void MakeResourceOnIsland(Island island, bool pair = false)
    {
        if (pair)
            IM.MakeResourceOnIsland(IM.NearstIsland(island.IslandNumber, false, hasBuild: false), false);

        SceneObject forestTemp = GM.instatinatePrefab("Prefabs/enviroment/forest", Containers.Enviroment);
        forestTemp.transform.position = island.ForestPlace.transform.position;
        forestTemp.GetComponent<LDForest>().IslandNumber = island.IslandNumber;
        forestTemp.GetComponent<LDForest>().LockLocation();
        forestTemp.GetComponent<LDForest>().HardSet();
        RM.Forests.Add(forestTemp.GetComponent<LDForest>());
        forestTemp.Visible = island.Visible;
        forestTemp.gameObject.SetActive(true);

        SceneObject mineTemp = GM.instatinatePrefab("Prefabs/enviroment/mine_hill", Containers.Enviroment);
        mineTemp.transform.position = island.MountainPlace.transform.position;
        mineTemp.GetComponent<LDMineHill>().IslandNumber = island.IslandNumber;
        mineTemp.GetComponent<LDMineHill>().LockLocation();
        mineTemp.GetComponent<LDMineHill>().HardSet();
        mineTemp.Visible = island.Visible;
        RM.Hills.Add(mineTemp.GetComponent<LDMineHill>());
        mineTemp.gameObject.SetActive(true);


        island.HasResource = true;


    }*/

    public static void InitWeb(int islNum = -1)
    {
        if (IM.instance.CompleteWeb)
            return;

        Island startIsland;

        if (islNum == -1)
            startIsland = IM.Islands[0];
        else
            startIsland = IM.Islands[islNum];

        if(startIsland == null)
        {
            Debug.LogError("Not found island");
            return;
        }

        startIsland.InWeb = true;

        IM.MakeIslandWeb(startIsland);

        float widthMin = 0f;
        float widthMax = 0;
        float minHeight = 0f;
        float maxHeight = 0;

        foreach(var temp in IM.Islands)
        {
            if (temp.position.x < widthMin)
                widthMin = temp.position.x;
            if (temp.position.x > widthMax)
                widthMax = temp.position.x;
            if (temp.position.y < minHeight)
                minHeight = temp.position.y;
            if (temp.position.y > maxHeight)
                maxHeight = temp.position.y;
        }

        IM.instance._mapsize = new Vector2((widthMax - widthMin) + 6f, (maxHeight - minHeight) + 6f);
        

        if (IM.Islands.Where(isl => isl.InWeb == false).ToList().Count > 0)
            Debug.LogError("Big Fail with Connection");

        IM.instance.CompleteWeb = true;
        IM.IslandActions(startIsland.IslandNumber);
    }

    public static void MakeIslandWeb(Island island)
    {
        foreach (Island temp in IM.Islands)
        {
            if (temp == island)
                continue;

            if (Vector2.Distance(island.position, temp.position) <= 60)
            {
                island.IslandConnection.Add(temp.IslandNumber);
                temp.InWeb = true;
            }
        }

        if (island.IslandConnection.Count == 0)
        {
            float distanceOne = 100000f;
            int k = -1;
            int iNumber = -1;
            foreach (Island temp in IM.Islands)
            {
                k += 1;
                if (temp == island)
                    continue;

                float inDis = Vector2.Distance(island.position, temp.position);

                if (inDis < distanceOne)
                {
                    distanceOne = inDis;
                    iNumber = k;
                }
            }

            island.IslandConnection.Add(IM.Islands[iNumber].IslandNumber);
            IM.Islands[iNumber].InWeb = true;

            float distanceTwo = 100000f;
            k = -1;
            iNumber = -1;
            foreach (Island temp in IM.Islands)
            {
                k += 1;
                if (temp == island)
                    continue;

                float inDis = Vector2.Distance(island.transform.position, temp.position);

                if (inDis < distanceTwo && inDis > distanceOne)
                {
                    distanceTwo = inDis;
                    iNumber = k;
                }
            }

            island.IslandConnection.Add(IM.Islands[iNumber].IslandNumber);
            IM.Islands[iNumber].InWeb = true;

            float distanceThree = 100000f;
            k = -1;
            iNumber = -1;
            foreach (Island temp in IM.Islands)
            {
                k += 1;
                if (temp == island)
                    continue;

                float inDis = Vector2.Distance(island.transform.position, temp.position);

                if (inDis < distanceThree && inDis > distanceTwo)
                {
                    distanceThree = inDis;
                    iNumber = k;
                }
            }

            island.IslandConnection.Add(IM.Islands[iNumber].IslandNumber);
            IM.Islands[iNumber].InWeb = true;
        }

        foreach (int temp in island.IslandConnection)
        {
            if (IM.Islands[temp].IslandConnection.Count == 0)
                IM.MakeIslandWeb(IM.Islands[temp]);
        }
    }

    public static int GetIslandNumber(Vector3 point)
    {
        foreach(var isl in IM.instance._islands)
        {
            if (isl.Collider.bounds.Contains(point))
                return isl.IslandNumber;
        }

        return -1;
    }

    public static Island GetIsland(int islNum)
    {
        if(IM.instance._islands.Count >= islNum || islNum < 0)
        {
            Debug.LogError("No island number: " + islNum);
            return null;
        }

        return IM.instance._islands[islNum];
    }

    public static Island GetIsland(SceneObject obj)
    {
        if (IM.instance._islands.Count >= obj.IslandNumber || obj.IslandNumber < 0)
        {
            Debug.LogError("No island number: " + obj.IslandNumber);
            return null;
        }

        return IM.instance._islands[obj.IslandNumber];
    }

    public static Vector2 MapSize
    {
        get { return IM.instance._mapsize; }
    }

}
