using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using GameEvents;
using System;

public class MissionLoader : MonoBehaviour {

    void Awake()
    {
        DragAndDropItem.PrepareDragItems();

        TextAsset asetText = Resources.Load("missions/Slots/test") as TextAsset;
        string pathString = asetText.text;

        JSONNode doc = JSON.Parse(pathString);
        GM.mission = doc["SelectedMission"];
        GM.PlatformType = (Lodkod.PlatformType)Enum.Parse(typeof(Lodkod.PlatformType), doc["Platform"].Value);
        IOM.NewGame(doc);
        IconObject.InitializePrafabs();
        BuildTypes.BuildInstance.InitAllPacks();

        ES.NewGame();
        TM.NewGame(doc);
        LS.NewGame(doc);
        BS.NewGame();
        GEM.NewGame();
        SaveManager.NewGame();
        UIParameters.NewGame();
        GameEvent.initAllPacks();
        IM.NewGame();
        GM.NewGame();
        SpriteProvider.InitAll();
        
        UIM.NewGame();
        CM.NewGame();
        SM.NewGame();
        //RM.NewGame();
        //BM.NewGame();
        
        BattleUnitAI.BattleUnitEnemyAI.initAllPacks();
        BattleActions.BattleAction.initAllPacks();
        BattleEffects.BattleEffect.initAllPacks();
        QS.NewGame();

        GM.InitGame();
    }
}

/*
 * 
 * GM - Global Manager (work with global functions, has units array, player hero)
 * 
 * GIM - GUI Manager. Need for work with GUI
 * 
 * IOM - Build Info Manager. Init build template. Cost of Builds and other info
 * 
 * AIM - AI Manager. Has list of AI Units.
 * 
 * SM - Storage Manager. Work with storages, for player and AI
 * 
 * TM - Timer Activity Manager. Need to create differents global events with timer
 * 
 * RM - Resource Manager. Have list of all resource object on map
 * 
 * BM - Build Manager. Have list of centers(AI and player). And have list of settlements.
 * 
 * IM - Island Manager. Init all islands and have a list of island to access them
 * 
 * EM - Event Manager. Need to add events in queue.
 * 
 * MM - Map Manager. Init all info and work with mesh map. Need to read to know about all objects on it and can show where to place it
 */
