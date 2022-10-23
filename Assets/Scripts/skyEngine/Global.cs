using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using SimpleJSON;
using Lodkod;
using EventPackSystem;

public class GM
{
    // loader helper 
    
    public GameState _gameState;
    //    public CameraControll cameraControll;
    public CameraScript _camera;
    public Ship mainShip;
    public int sides;

    Transform _hideIcons;

    public PlayerIcon _playerIcon;

    Vector2 _mapSize;
    
    List<SceneObject> units;
    Dictionary<string, SceneObject> UniqObjects;
    Dictionary<string, MapLocationObject> MapObjects;
    Dictionary<string, IconObject> IconObjects;


    EventPackManager packManager;

    public static PlatformType PlatformType = PlatformType.PC;

    private static GM instance = null;
    public static string mission;
    public static void NewGame()
    {
        if (GM.instance != null)
            GM.instance = null;

        GM.instance = new GM();
        GM.instance.units = new List<SceneObject>();
        GM.instance.IconObjects = new Dictionary<string, IconObject>();
        GM.instance.MapObjects = new Dictionary<string, MapLocationObject>();

        GM.instance.sides = 0;

        GM.instance.InitContainers();

        GM.instance.InitUniqs();
        //Unit.preSet();
        

        GM.instance._camera = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        GM.instance.packManager = new EventPackManager();
        GM.instance.mainShip = GM.GetObject("MainShip") as Ship;
        //GM.instance.player = GameObject.Find("Player").GetComponent<Player>();
        //GM.instance.player.initUnit();

        GM.instance._hideIcons = GameObject.Find("HideIcons").transform;

        GM.instance._playerIcon = GameObject.Find("PlayerIcon").GetComponent<PlayerIcon>();
        GM.instance.IconObjects.Add("Player", GM.instance._playerIcon);

        /*LDObject[] temps = Resources.LoadAll<LDObject>("Prefabs/units");
        for(int i = 0; i < temps.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(temps[i]);
            path = path.Substring(path.LastIndexOf("/")+1);
            path = path.Remove(path.IndexOf(".prefab"));
            Debug.Log("prefab path:" + path);
        }*/


    }
    public static void InitGame()
    {
        GM.instance.packManager.initAllPacks();

        GEM.Execute(GM.Pack("init"));
        TM.Start();
        //GM.Player.processState(UnitState.s_Activation, 0);
    }
    public static void CallEditorPacks()
    {
        GM.instance.packManager.LoadAllEditorPacks();
    }
    public static List<SceneObject> Units
    {
        get { return GM.instance.units; }
    }
    public static GameState GameState
    {
        get { return GM.instance._gameState; }
        set { GM.instance._gameState = value; ES.NotifySubscribers(TriggerType.GameStateChanged.ToString(), ""); }
    }
    public static CameraScript Camera
    {
        get { return GM.instance._camera; }
    }
    public static EventPack Pack(string value)
    {
        if (GM.instance.packManager.packs.ContainsKey(value))
            return GM.instance.packManager.packs[value];

        return GM.instance.packManager.loadEvent(value);
    }
    public static EventPack DelayedPack(string objID, string value)
    {
        return GM.instance.packManager.DelaydPacks(objID, value);
    }
    public static PlayerIcon Player
    {
        get { return GM.instance._playerIcon; }
    }
    public static PlayerIcon PlayerIcon
    {
        get { return GM.instance._playerIcon; }
    }
    public static Ship MainShip
    {
        get { return GM.instance.mainShip; }
    }
    public static int Sides
    {
        get { return GM.instance.sides; }
        set { GM.instance.sides = value; }
    }
    public static Vector2 MapSize
    {
        get { return GM.instance._mapSize; }
        set { GM.instance._mapSize = value; }
    }
    public static Dictionary<string, SceneObject> Uniqs
    {
        get { return GM.instance.UniqObjects; }
    }
    public static Transform HideIcons
    {
        get { return GM.instance._hideIcons; }
    }

    /*
     * Need to activate or deactivate ReactPanel
     */
    public static void ActiveIsland(int islNum)
    {
        foreach(var obj in GM.Uniqs.Values)
        {
            if (obj.IslandNumber != islNum)
                obj.LockLocation();
            else
                obj.LockLocation(false);
        }
    }

    #region Logic Function
    /*
     * Logic functions
     * 
     * 
     */

    GameObject ContainerUnits;
    GameObject ContainerCenters;
    GameObject ContainerBonfires;
    GameObject ContainerEnviroment;
    GameObject ContainerIslands;
    GameObject ContainerUniqs;

    void InitContainers()
    {
        ContainerUnits = GameObject.Find("units");
        ContainerCenters = GameObject.Find("centers");
        ContainerBonfires = GameObject.Find("bonfires");
        ContainerEnviroment = GameObject.Find("enviroment");
        ContainerIslands = GameObject.Find("islands");
        ContainerUniqs = GameObject.Find("uniqs");
    }

    public static SceneObject instatinatePrefab(string prefab, Containers container)
    {
        SceneObject temp = UnityEngine.Object.Instantiate(Resources.Load<SceneObject>(prefab));
        temp.HardSet();

        if (container == Containers.Units)
            temp.transform.SetParent(GM.instance.ContainerUnits.transform);
        else if (container == Containers.Centers)
            temp.transform.SetParent(GM.instance.ContainerCenters.transform);
        else if (container == Containers.Bonfires)
            temp.transform.SetParent(GM.instance.ContainerBonfires.transform);
        else if (container == Containers.Enviroment)
            temp.transform.SetParent(GM.instance.ContainerEnviroment.transform);
        else if (container == Containers.Islands)
            temp.transform.SetParent(GM.instance.ContainerIslands.transform);
        else if (container == Containers.Uniqs)
            temp.transform.SetParent(GM.instance.ContainerUniqs.transform);

        return temp;
    }

    public static Transform GetParentContainer(Containers container)
    {
        if (container == Containers.Units)
            return GM.instance.ContainerUnits.transform;
        else if (container == Containers.Centers)
            return GM.instance.ContainerCenters.transform;
        else if (container == Containers.Bonfires)
            return GM.instance.ContainerBonfires.transform;
        else if (container == Containers.Enviroment)
            return GM.instance.ContainerEnviroment.transform;
        else if (container == Containers.Islands)
            return GM.instance.ContainerIslands.transform;
        else
            return GM.instance.ContainerUniqs.transform;
    }

    /**
     * Найти свободного юнита
     */
    public static SceneObject getUnit(Unitype type)
    {
        SceneObject temp = null;

        for (int i = 0; i < instance.units.Count; i++)
        {
            if (instance.units[i].gameObject.activeSelf == false && instance.units[i].GetComponent<Unit>().Type == (int)type)
            {
                temp = instance.units[i];
                return temp;
            }
        }


        if (temp == null)
        {
            switch (type)
            {
                case Unitype.Settler:

                    temp = GM.instatinatePrefab("Prefabs/units/settler", Containers.Units);
                    temp.gameObject.SetActive(false);
                    instance.units.Add(temp);

                    break;


                case Unitype.Forester:

                    temp = GM.instatinatePrefab("Prefabs/units/forester", Containers.Units);
                    temp.gameObject.SetActive(false);
                    instance.units.Add(temp);

                    break;

                case Unitype.Miner:

                    temp = GM.instatinatePrefab("Prefabs/units/miner", Containers.Units);
                    temp.gameObject.SetActive(false);
                    instance.units.Add(temp);

                    break;


                case Unitype.Farmer:

                    temp = GM.instatinatePrefab("Prefabs/units/farmer", Containers.Units);
                    temp.gameObject.SetActive(false);
                    instance.units.Add(temp);

                    break;


                default:
                    break;
            }
        }

        return temp;
    }
    #endregion

    #region FindLogic

    public static SkyObject GetObject(string ID)
    {

        if (ID.Equals("player"))
            return GM.instance._playerIcon;
        else if (ID.Equals("Player"))
            return GM.instance._playerIcon;
        else if (GM.instance.IconObjects.ContainsKey(ID))
            return GM.instance.IconObjects[ID];
        else if (GM.instance.UniqObjects.ContainsKey(ID))
            return GM.instance.UniqObjects[ID];
        else if (ID.Length >= 1 && ID.Substring(0, 1) == "i")
            return IM.Islands.FirstOrDefault(isl => isl.ID.Equals(ID));
        else
            return UIM.GetItem(ID);
    }

    public static IconObject GetIcon(string ID)
    {
        if (GM.instance.IconObjects.ContainsKey(ID))
            return GM.instance.IconObjects[ID];

        Debug.LogError("Not found icon object: " + ID + ". Returning Player Icon.");
        return GM.instance._playerIcon;
    }

    public static SceneObject GetUniqOnIsland(int isl)
    {
        return GM.instance.UniqObjects.FirstOrDefault(uni => uni.Value.IslandNumber == isl).Value;
    }

    #endregion

    public static Spikes createSpike(Vector3 point)
    {
        SceneObject temp = GM.instatinatePrefab("Prefabs/enviroment/spikes", Containers.Enviroment);
        temp.HardSet();
        temp.transform.position = point;
       
        RM.Spikes.Add(temp.GetComponent<Spikes>());

        return temp.GetComponent<Spikes>();
    }

    #region Uniqs Initialize

    void InitUniqs()
    {
        GM.instance.UniqObjects = new Dictionary<string, SceneObject>();

        List<SceneObject> icons = new List<SceneObject>(Resources.LoadAll<SceneObject>("missions/CommonObjects/Prefabs"));
        GameObject parent = GameObject.Find("uniqs");
        
        foreach (var sprite in icons)
        {
            string key = sprite.name;
            SceneObject temp = GameObject.Instantiate<SceneObject>(sprite);
            temp.ID = key;
            temp.HardSet();
            temp.Visible = false;
            temp.gameObject.SetActive(false);
            temp.Lock = true;
            temp.transform.SetParent(parent.transform);
            GM.instance.UniqObjects.Add(key, temp);
            if (temp.GetComponent<MapLocationObject>() != null)
                GM.instance.MapObjects.Add(key, temp.GetComponent<MapLocationObject>());
        }
        icons.Clear();

        icons = new List<SceneObject>(Resources.LoadAll<SceneObject>("missions/mission" + GM.mission + "/Prefabs"));
        foreach (var sprite in icons)
        {
            string key = sprite.name;
            SceneObject temp = GameObject.Instantiate<SceneObject>(sprite);
            temp.ID = key;
            temp.HardSet();
            temp.Visible = false;
            temp.gameObject.SetActive(false);
            temp.Lock = true;
            temp.transform.SetParent(parent.transform);
            GM.instance.UniqObjects.Add(key, temp);
            if (temp.GetComponent<MapLocationObject>() != null)
                GM.instance.MapObjects.Add(key, temp.GetComponent<MapLocationObject>());
        }
        icons.Clear();
    }

    public static List<MapLocationObject> GetObjectsOnIsland(int island)
    {
        return GM.instance.MapObjects.Values.Where(obj => obj.IslandNumber == island).ToList();
    }

    public static void AddUniq(SceneObject obj)
    {
        GM.instance.UniqObjects.Add(obj.ID, obj);
    }

    #endregion

    #region Icon Work

    public static void RemoveIcon(string id)
    {
        if(GM.instance.IconObjects.ContainsKey(id))
        {
            GM.instance.IconObjects[id].RemoveIcon();
            GM.instance.IconObjects[id].Visible = false;
        }
        else
            Debug.LogError("Not found icon object: " + id);
    }

    public static void AddIcon(IconObject icon)
    {
        if(!GM.instance.IconObjects.ContainsKey(icon.ID))
            GM.instance.IconObjects.Add(icon.ID, icon);
    }

    #endregion
}
