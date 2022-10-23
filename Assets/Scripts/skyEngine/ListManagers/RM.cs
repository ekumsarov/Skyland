using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Lodkod;

public class RM {

    List<LDForest> forests;
    List<LDMineHill> hiils;
    List<Spikes> spikes;

    private static RM instance = null;
    public static void NewGame()
    {
        if (RM.instance != null)
            RM.instance = null;

        RM.instance = new RM();

        RM.instance.forests = new List<LDForest>();
        RM.instance.hiils = new List<LDMineHill>();
        RM.instance.spikes = new List<Spikes>();
    }

    public static List<LDForest> Forests
    {
        get { return RM.instance.forests; }
    }
    public static List<LDMineHill> Hills
    {
        get { return RM.instance.hiils; }
    }
    public static List<Spikes> Spikes
    {
        get { return RM.instance.spikes; }
    }

    public static SceneObject GetObject(string ID)
    {
        if (RM.instance.forests.Any(obj => obj.ID == ID))
            return RM.instance.forests.First(obj => obj.ID == ID);
        else if (RM.instance.hiils.Any(obj => obj.ID == ID))
            return RM.instance.hiils.First(obj => obj.ID == ID);
        else if (RM.instance.spikes.Any(obj => obj.ID == ID))
            return RM.instance.spikes.First(obj => obj.ID == ID);

        return null;
    }
}
