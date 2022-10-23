using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lodkod;
using System;
using System.Linq;
using GameEvents;
using SimpleJSON;
using BattleActions;

public class BattleSystem : MenuEx
{

    #region base parameters

    [UnityEngine.SerializeField]
    bool Active = false;
    bool PlayerTurn = false;

    [SerializeField] UIItem _frameSelector;

    Action Event;
    public BattleEvent BattleEvent;

    public List<HeroUnit> PlayerArmy;
    public List<HeroUnit> EnemyArmy;

    HeroInfoItem activeBattleUnit = null;
    HeroInfoItem selectedOpponent = null;
    BattleAction activeBattleAction = null;

//    public ActionSkillPanel actPanel;

    [SerializeField] HeroPanel _playerPanel;
    [SerializeField] HeroPanel _enemyPanel;

    public List<HeroInfoItem> playerHeroItems;
    public List<BattleUnitEnemy> enemyHeroItems;

    List<BattleActionItem> actionItems;

    #endregion

    public override void Setting()
    {
        base.Setting();

        this.PlayerTurn = false;
        
        this.playerHeroItems = new List<HeroInfoItem>();
        this.enemyHeroItems = new List<BattleUnitEnemy>();
        this.actionItems = new List<BattleActionItem>();

        

        for (int i = 1; i <= this.actionItems.Count; i++)
        {
            BattleActionItem item = this._allItems["BattleAction" + i] as BattleActionItem;
            this.actionItems.Add(item);
        }

        for(int i = 1; i < 4; i++)
        {
            if (this._allItems.ContainsKey("Hero" + i))
                this.playerHeroItems.Add(this._allItems["Hero" + i] as HeroInfoItem);

            if (this._allItems.ContainsKey("Enemy" + i))
                this.enemyHeroItems.Add(this._allItems["Enemy" + i] as BattleUnitEnemy);
        }
    }

    public void Prepare(List<HeroUnit> LArmy, List<HeroUnit> RArmy, Action ev, BattleEvent BattleEv, int rounds = 10)
    {
        this.PlayerTurn = false;
        this.Event = ev;
        this.BattleEvent = BattleEv;
        this.PlayerArmy = LArmy;
        this.EnemyArmy = RArmy;
        this._frameSelector.Visible = false;

        this.selectedOpponent = null;
        this.activeBattleUnit = null;
        this.activeBattleAction = null;

        _playerPanel.ClearCheck();
        _enemyPanel.ClearCheck();


        for (int i = 0; i < this.playerHeroItems.Count; i++)
        {
            if(i < this.PlayerArmy.Count)
            {
                this.playerHeroItems[i].SetupItem(this.PlayerArmy[i]);
                this.playerHeroItems[i].Visible = true;
            }
            else
                this.playerHeroItems[i].Visible = false;
        }

        for (int i = 0; i < this.enemyHeroItems.Count; i++)
        {
            if (i < this.EnemyArmy.Count)
            {
                this.enemyHeroItems[i].SetupItem(this.EnemyArmy[i]);
                this.enemyHeroItems[i].Visible = true;
            }
            else
                this.enemyHeroItems[i].Visible = false;
        }

        for (int i = 0; i < 4; i++)
        {
            if (i < this.PlayerArmy.Count)
            {
                this.PlayerArmy[i].MakeDexResult();
                this.PlayerArmy[i].EndTurn = false;
            }

            if (i < this.EnemyArmy.Count)
            {
                this.EnemyArmy[i].MakeDexResult();
                this.EnemyArmy[i].EndTurn = false;
            }
        }

        this.BattleEvent.PlaceUnits();
    }

    public void NextTurn()
    {
        this.PlayerTurn = false;

        _frameSelector.Visible = false;

        if(this.selectedOpponent != null)
            this.selectedOpponent.Frame = false;

        if(this.activeBattleUnit != null)
            this.activeBattleUnit.Frame = false;

        _playerPanel.RegisterHero(null);
        _enemyPanel.RegisterHero(null);
        

        StartCheckAnimation = false;

        activeBattleAction = null;
        selectedOpponent = null;
        activeBattleUnit = null;

        if (activeBattleUnit != null)
            activeBattleUnit.bindUnit.EffectTurnEnd();

        if(this.activeBattleUnit != null)
            this.activeBattleUnit.Frame = false;

        if (this.PlayerArmy.All(unit => unit.CurrentHP <= 0))
        {
            this.BattleEvent.BattleEnd(false);
            this.Close();
            return;
        }
        else if(this.EnemyArmy.All(unit => unit.CurrentHP <= 0))
        {
            this.BattleEvent.BattleEnd(true);
            this.Close();
            return;
        }
            
            
        int max = 0;
        int index = 0;
        bool player = true;
        for (int i = 0; i < 4; i++)
        {
            if (i < this.PlayerArmy.Count && this.PlayerArmy[i].CurrentHP > 0 && !this.PlayerArmy[i].EndTurn && this.PlayerArmy[i].DexResult > max)
            {
                max = this.PlayerArmy[i].DexResult;
                index = i;
                player = true;
            }

            if (i < this.EnemyArmy.Count && this.EnemyArmy[i].CurrentHP > 0 && !this.EnemyArmy[i].EndTurn && this.EnemyArmy[i].DexResult > max)
            {
                max = this.EnemyArmy[i].DexResult;
                index = i;
                player = false;
            }
        }
        
        if(max == 0)
        {
            this.BattleEvent.EndRound();
            return;
        }

        if(player)
        {
            
            this.activeBattleUnit = this.playerHeroItems[index];
            this.activeBattleUnit.Frame = true;
            this.ActivatePlayerUnit(this.activeBattleUnit);
        }
        else
        {
            this.PlayerTurn = false;
            this.activeBattleUnit = this.enemyHeroItems[index];
            this._enemyPanel.RegisterHero(this.activeBattleUnit);
            this.enemyHeroItems[index].Frame = true;
            this.enemyHeroItems[index].StartTurn();
        }
    }

    public void NewRound()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < this.PlayerArmy.Count && this.PlayerArmy[i].CurrentHP > 0)
            {
                this.PlayerArmy[i].EndTurn = false;
                this.PlayerArmy[i].MakeDexResult();
            }
                

            if (i < this.EnemyArmy.Count && this.EnemyArmy[i].CurrentHP > 0)
            {
                this.EnemyArmy[i].EndTurn = false;
                this.EnemyArmy[i].MakeDexResult();
            }
        }

        this.NextTurn();
    }

    public override void SelectedItem(UIItem data, bool enter)
    {
        if (!this.PlayerTurn)
            return;

        if (this.StartCheckAnimation)
            return;

        if (data.ItemTag.Equals("EndRound"))
            return;

        HeroInfoItem unit = data as HeroInfoItem;

        if(enter && unit != null)
        {
            if (unit.Side == activeBattleUnit.Side)
                return;

            _enemyPanel.RegisterHero(unit);

            if (activeBattleAction != null)
                _enemyPanel.RegisterActionAnswer(activeBattleAction);

            _frameSelector.position = new Vector3(unit.position.x, _frameSelector.position.y, _frameSelector.position.z);

            if (_frameSelector.Visible == false)
                _frameSelector.Visible = true;
        }
        else if (!enter)
        {
            if (unit == null || unit == activeBattleUnit || unit == selectedOpponent)
                return;

            if(activeBattleUnit != null)
            {
                if (activeBattleUnit.Side == 0)
                    _playerPanel.RegisterHero(activeBattleUnit);
                else
                    _enemyPanel.RegisterHero(activeBattleUnit);

                if(activeBattleAction != null)
                {
                    if (activeBattleUnit.Side == 0)
                        _playerPanel.RegisterAction(activeBattleAction);
                    else
                        _enemyPanel.RegisterActionAnswer(activeBattleAction);
                }
            }
        }
    }

    public override void PressedItem(UIItem data)
    {
        if (!this.PlayerTurn)
            return;


        if (data.ItemTag.Equals("EndRound"))
            return;

        if(activeBattleAction == null)
        {
            if (data.ItemTag.Equals("BattleAction"))
            {
                BattleActionItem item = data as BattleActionItem;
                this.activeBattleAction = item.BattleAction;

                if(activeBattleUnit != null)
                {
                    if (activeBattleUnit.Side == 0)
                        _playerPanel.RegisterAction(activeBattleAction);
                    else
                        _enemyPanel.RegisterAction(activeBattleAction);
                }

                if(selectedOpponent != null)
                {
                    if (selectedOpponent.Side == 0)
                        _playerPanel.RegisterActionAnswer(activeBattleAction);
                    else
                        _enemyPanel.RegisterActionAnswer(activeBattleAction);
                }
            }
            else
                return;
            /*else if(data.ItemTag.Equals("EnemyUnit"))
            {
                HeroInfoItem unit = data as HeroInfoItem;

                if(selectedOpponent != null)
                {
                    selectedOpponent.Frame = false;
                    selectedOpponent = null;

                    if (selectedOpponent.Side == 1)
                        _enemyPanel.RegisterHero(null);
                    else
                        _playerPanel.RegisterHero(null);
                }

                selectedOpponent = unit;
                selectedOpponent.Frame = true;
                if (selectedOpponent.Side == 1)
                    _enemyPanel.RegisterHero(selectedOpponent);
                else
                    _playerPanel.RegisterHero(selectedOpponent);
            }*/
        }
        else if(activeBattleAction != null)
        {
            if (data.ItemTag.Equals("EnemyUnit") || data.ItemTag.Equals("HeroUnit"))
            {
                HeroInfoItem unit = data as HeroInfoItem;

                if (activeBattleAction != null)
                    activeBattleAction.PressedItem(unit);
            }
            else if (data.ItemTag.Equals("BattleAction"))
            {
                BattleActionItem item = data as BattleActionItem;
                this.activeBattleAction = item.BattleAction;

                if (activeBattleUnit != null)
                {
                    if (activeBattleUnit.Side == 0)
                        _playerPanel.RegisterAction(activeBattleAction);
                    else
                        _enemyPanel.RegisterAction(activeBattleAction);
                }

                if (selectedOpponent != null)
                {
                    if (selectedOpponent.Side == 0)
                        _playerPanel.RegisterActionAnswer(activeBattleAction);
                    else
                        _enemyPanel.RegisterActionAnswer(activeBattleAction);
                }
            }
            else
                Debug.LogError("No found tag of item in BattleSystem. Tag of item: " + data.ItemTag);
        }
    }
    
    void BattleState(HeroInfoItem unit)
    {
        if(unit != null && unit.Side == 0)
        {
            int index = 0;
            foreach(var skill in unit.bindUnit.skills)
            {
                if (index >= _playerPanel._skills.Count)
                    break;

                _playerPanel._skills[index].Setup(skill.Value);
                index++;
            }

            index = 0;
            foreach(var action in unit.Actions)
            {
                if (index >= 8)
                    break;

                _playerPanel._actions[index].SetAction(action.Icon, action.ActName, action);
                _playerPanel._actions[index].Visible = true;
            }

            for(int i = index; i < _playerPanel._actions.Count; i++)
            {
                _playerPanel._actions[i].Visible = false;
            }
        }
        else if(unit == null && this.activeBattleUnit != null && this.activeBattleUnit.Side == 0)
        {
            int index = 0;
            foreach (var skill in activeBattleUnit.bindUnit.skills)
            {
                if (index >= _playerPanel._skills.Count)
                    break;

                _playerPanel._skills[index].Setup(skill.Value);
                index++;
            }

            index = 0;
            foreach (var action in activeBattleUnit.Actions)
            {
                if (index >= 8)
                    break;

                _playerPanel._actions[index].SetAction(action.Icon, action.ActName, action);
                _playerPanel._actions[index].Visible = true;
            }

            for (int i = index; i < _playerPanel._actions.Count; i++)
            {
                _playerPanel._actions[i].Visible = false;
            }
        }
    }

    void ActivatePlayerUnit(HeroInfoItem unit)
    {
        /* for (int i = 1; i <= 4; i++)
        {
            if (i <= unit.bindUnit.actions.Count)
            {
                BattleActionItem act = this._allItems["BattleAction" + i] as BattleActionItem;
                act.Visible = true;
                act.SetAction(IOM.BattleActionInfoDic[unit.bindUnit.actions[i - 1]].Icon, IOM.BattleActionInfoDic[unit.bindUnit.actions[i - 1]].Name, BattleAction.loadBattleAction(IOM.BattleActionInfoDic[unit.bindUnit.actions[i - 1]].Name, unit));
            }
            else 
                this._allItems["BattleAction" + i].Visible = false;
        }*/
        _playerPanel.RegisterHero(unit);
        unit.bindUnit.EffectTurnStart();
        //this._allItems["ActionPanel"].Visible = true;
        this.PlayerTurn = true;
    }

    #region show panel control

    public void SelectOpponent(HeroInfoItem unit)
    {
        this.selectedOpponent = unit;

        if (this.activeBattleUnit != null && this.activeBattleUnit.Side == this.selectedOpponent.Side)
            return;

        if (this.selectedOpponent.Side == 0)
            this._playerPanel.RegisterHero(this.selectedOpponent);
        else
            this._enemyPanel.RegisterHero(this.selectedOpponent);
    }

    public void StartEnemyAction(BattleAction action)
    {
        activeBattleAction = action;
        if (activeBattleUnit != null)
        {
            if (activeBattleUnit.Side == 0)
                _playerPanel.RegisterAction(activeBattleAction);
            else
                _enemyPanel.RegisterAction(activeBattleAction);
        }

        if (selectedOpponent != null)
        {
            if (selectedOpponent.Side == 0)
                _playerPanel.RegisterActionAnswer(activeBattleAction);
            else
                _enemyPanel.RegisterActionAnswer(activeBattleAction);
        }

        action.Start();
    }

    public void ShowAction(BattleAction action, int side)
    {
        if (side == 0)
            _playerPanel.RegisterAction(action);
        else
            _enemyPanel.RegisterAction(action);
    }

    #endregion

    #region both check panels control

    private GroupSkillCheck _sCheck;
    int _side = 0;
    string _sIcon;
    Action _resultCallback;
    private bool StartCheckAnimation = false;

    public void StartCheck(GroupSkillCheck succesCheck, int side, string sIcon, Action callback)
    {
        _sCheck = succesCheck;
        _side = side;
        _sIcon = sIcon;
        _resultCallback = callback;
        StartCheckAnimation = true;

        if (side == 0)
            _playerPanel.ShowCheckSkills(succesCheck);
        else
            _enemyPanel.ShowCheckSkills(succesCheck);

        StartCoroutine(ShowResultWait());
    }

    IEnumerator ShowResultWait()
    {
        yield return new WaitForSeconds(3f);
        ShowResult();
    }

    void ShowResult()
    {
        if (_side == 0)
            _playerPanel.ShowResult(_sCheck, _sIcon, _resultCallback);
        else
            _enemyPanel.ShowResult(_sCheck, _sIcon, _resultCallback);
    }

    #endregion

    /*#region single panel control

    public void StartCheck(GroupSkillCheck skillCheck, int side, string icon, Action callback)
    {
        if (side == 0)
            _playerPanel.ShowCheckSkills(skillCheck);
        else
            _enemyPanel.ShowCheckSkills(skillCheck);

        StartCoroutine(SingleShowResultWait());
    }

    IEnumerator SingleShowResultWait()
    {
        yield return new WaitForSeconds(1.5f);
        SingleShowResult();
    }

    void SingleShowResult()
    {

    }

    #endregion*/
}