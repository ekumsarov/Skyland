using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class LDMineHill : SceneObject {

    public static float yExtra = 0.5f;

    public override void HardSet()
    {
        base.HardSet();

        this.ID = "hill" + this.IslandNumber;
    }

}
