using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lodkod;

public class PlayerIcon : IconObject
{
    public override void HardSet()
    {
        base.HardSet();

        this.Group.AddHeroes(IOM.PlayerPartyList);
        LS.PartyUpdate();

        this.InteractType = IconInteractType.Player;

        Actions NotReady = Actions.Get("Context");

        NotReady.ID = "NotReady";
        NotReady.SetText("NotReady");
        NotReady.AddChoice(ActionButtonInfo.Create("MoveBack").SetCallData("StandartAction"));
        this.AddAction(NotReady);

        Actions mainAction = Actions.Get("Context");

        mainAction.ID = "StandartAction";
        mainAction.AddChoice(ActionButtonInfo.Create("GroupMenu").SetCallback(OpenGroup));
        mainAction.AddChoice(ActionButtonInfo.Create("OpenDiary").SetCallback(OpenDiary));
        mainAction.AddChoice(ActionButtonInfo.Create("Close").SetType(ActionType.Close));
        this.AddAction(mainAction);

        this.MainEvent = "StandartAction";
    }

    public override void SetObjectParent(SceneObject parent)
    {

        this._parent = parent;
        Transform trans = this._parent.LocationPanel.transform;
        this._button.transform.SetParent(trans);
        this._button.transform.localRotation = new Quaternion(0, 0, 0, 0);
        this._button.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        this._button.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    public void OpenGroup()
    {
        UIM.OpenMenu("HeroesMenu");
    }

    public void OpenDiary()
    {
        UIM.OpenMenu("QuestMenu");
    }

    public void AddFood()
    {
        //QS.AddQuest(QuestNode.Create("TestQuest").SetDescription("TestDescription\n\nWOW!!!\nHow?").SetIcon("BaseIcon"));
        //SM.Stats["Food"].Count += 7;
    }
}
