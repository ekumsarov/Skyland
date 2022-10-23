using Lodkod;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

public class TRM // Info Manager
{
    private static TRM instance = null;

    public static void NewGame(JSONNode slotInfo)
    {
        if (TRM.instance != null)
            TRM.instance = null;

        TRM.instance = new TRM();

        TextAsset pathStringAvailableBuildInfo = Resources.Load("missions/config/BuildInfo") as TextAsset;
        TextAsset pathStringBuildInfo = Resources.Load("missions/config/BuildInfo") as TextAsset;
        TextAsset pathStringBuildInfoAdd = Resources.Load("missions/mission" + GM.mission + "/config/BuildInfo") as TextAsset;
        TextAsset pathStringBattleUnitInfo = Resources.Load("missions/config/WarUnitInfo") as TextAsset;
        TextAsset pathStringBattleFieldInfo = Resources.Load("missions/config/BattleFieldInfo") as TextAsset;
        TextAsset pathStringBattleAction = Resources.Load("missions/config/BattleAction") as TextAsset;
        TextAsset pathStringBattleUnitAdd = Resources.Load("missions/mission" + GM.mission + "/config/BattleUnitAdd") as TextAsset;
        TextAsset pathStringBattleActionAdd = Resources.Load("missions/mission" + GM.mission + "/config/BattleActionAdd") as TextAsset;

        
    }
}