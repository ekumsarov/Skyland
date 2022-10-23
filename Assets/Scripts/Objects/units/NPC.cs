using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class NPC : Unit
{
    // Use this for initialization
    public override void initUnit()
    {
        this.Type = (int)Unitype.NPC;  // тип юнита(объекта) в данной игре

    }


    protected override void s_Activation()
    {
        this.InWork = false;

        this.Velocity = new Vector2(1.0f, 1.0f);

        this.Side = 2;
        this.IslandNumber = IM.GetIslandNumber(this.position);

        this.InWork = false;

        gameObject.SetActive(true);

        this.Finish();
    }



    /*
     * Other Functions
     * 
     */


    protected override void adminTasks()
    {
        this.InProcess = true;
    }

}