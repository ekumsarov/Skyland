using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using SimpleJSON;

public class SkillCheckObject : ObjectID
{
    #region ID
    string ObjectID = "nil";

    public string ID
    {
        get { return ObjectID; }
        set { this.ObjectID = value; }
    }
    #endregion

    // SkillID - the name of skill
    string _skillID = "nill";
    public string Skill
    {
        get { return this._skillID; }
        set { this._skillID = value; }
    }

    // Complex - complex of skill check
    private int _complex = 0;
    public int Complex
    {
        get { return this._complex; }
    }

    // Amount - need to complete
    private int _amount = 0;
    public int Amount
    {
        get { return this._amount; }
    }

    // Amount - need to complete
    private bool _hasMax = false;
    private int _maxResult = 0;
    public int MaxResult
    {
        set
        {
            if (value <= 0)
                return;

            _hasMax = true;
            _maxResult = value;
        }
        get
        {
            return this._maxResult;
        }
    }

    // Result - amount of result
    private int _complexResult = 0;
    public int ComplexResult
    {
        get { return this._complexResult; }
    }

    // Result - amount of result
    private int _amountResult = 0;
    public int ResultaAmount
    {
        get { return this._amountResult; }
    }

    private bool _completeUnfullDice = false;
    public bool CompleteDice
    {
        get { return this._completeUnfullDice; }
    }

    private int _remainder = 0;
    public int Remainder
    {
        get { return this._remainder; }
    }


    public static SkillCheckObject Create(string id, int complex, int amount = 0, int maxResult = 0)
    {
        return new SkillCheckObject()
        {
            _skillID = id,
            _complex = complex,
            _amount = amount,
            MaxResult = maxResult
        };
    }

    public static SkillCheckObject Create(JSONNode node)
    {
        SkillCheckObject temp = new SkillCheckObject();

        if (node["id"] != null)
            temp._skillID = node["id"].Value;

        if (node["complex"] != null)
            temp._complex = node["complex"].AsInt;

        if (node["amount"] != null)
            temp._amount = node["amount"].AsInt;

        if (node["max"] != null)
            temp._maxResult = node["max"].AsInt;

        return temp;
    }

    public int GetMaxResult(HeroUnit unit)
    {
        if (!unit.skills.ContainsKey(this.Skill))
            return 0;

        int remaind = 0;
        int maxResult = Math.DivRem(unit.skills[this.Skill].Max, this._complex, out remaind);

        if (remaind > 0)
            maxResult++;

        if (_maxResult > 0)
            return _maxResult > maxResult ? maxResult : _maxResult;
        
        return maxResult;
    }

    public int GetMaxResult(List<HeroUnit> unit)
    {
        if (unit.Any(uni => !uni.skills.ContainsKey(this.Skill)))
        {
            Debug.LogError("Unit has no skill: " + this.Skill);
            return 0;
        }

        int remaind = 0;
        int complexresult = 0;
        foreach (var uni in unit)
            complexresult = uni.skills[this.Skill].Max;
        int maxResult = Math.DivRem(complexresult, this._complex, out remaind);

        if (remaind > 0)
            maxResult++;

        if (_maxResult > 0)
            return _maxResult > maxResult ? maxResult : _maxResult;

        return maxResult;
    }

    public void CompleteCheck(HeroUnit unit, bool fullCheck = true)
    {
        if(!unit.skills.ContainsKey(this.Skill))
        {
            Debug.LogError("Unit: " + unit.Name + " has no skill: " + this.Skill);
            return;
        }

        this._complexResult = unit.skills[this.Skill].GetValue;
        this._amountResult = Math.DivRem(this._complexResult, this._complex, out this._remainder);

        if(fullCheck)
        {
            if (this._remainder > 0)
            {
                double chance = this._remainder / this._complex;
                int chanceOne = (int)(Math.Round(chance, 2) * 100);

                if (UnityEngine.Random.Range(0, 100) <= chanceOne)
                {
                    this._amountResult += 1;
                    this._completeUnfullDice = true;
                }
            }
        }
        if(_hasMax && this._amountResult > this._maxResult)
        {
            this._amountResult = this._maxResult;
            this._complexResult = this._amountResult * this._complex;
            this._completeUnfullDice = false;
        }
    }

    public void CompleteCheck(List<HeroUnit> unit, bool fullCheck = true)
    {
        if (unit.Any(uni => !uni.skills.ContainsKey(this.Skill)))
        {
            Debug.LogError("Unit has no skill: " + this.Skill);
            return;
        }

        foreach(var uni in unit)
            this._complexResult = uni.skills[this.Skill].GetValue;

        this._amountResult = Math.DivRem(this._complexResult, this._complex, out this._remainder);

        if(fullCheck)
        {
            if (this._remainder > 0)
            {
                double chance = this._remainder / this._complex;
                int chanceOne = (int)(Math.Round(chance, 2) * 100);

                if (UnityEngine.Random.Range(0, 100) <= chanceOne)
                {
                    this._amountResult += 1;
                    this._completeUnfullDice = true;
                }
            }
        }

        if (_hasMax && this._amountResult > this._maxResult)
        {
            this._amountResult = this._maxResult;
            this._complexResult = this._amountResult * this._complex;
            this._completeUnfullDice = false;
        }
    }

    public bool CheckResult
    {
        get
        {
            if(this._amount == 0)
            {
                Debug.LogError("Amount in check is 0");
                return true;
            }

            return this._amountResult >= this._amount;
        }
    }
}

public class GroupSkillCheck
{
    private List<SkillCheckObject> _skills;
    public List<SkillCheckObject> Skills
    { get { return this._skills; } }

    private int _finalResult;
    public int FinalResult
    {
        get { return this._finalResult; }
    }

    private int _combinedComplex;
    public int CombinedComplex
    {
        get { return this._combinedComplex; }
    }

    private int _complexResult;
    public int ComplexResult
    {
        get { return this._complexResult; }
    }

    private bool _completeUnfullDice = false;
    public bool CompleteDice
    {
        get { return this._completeUnfullDice; }
    }

    // Amount - need to complete
    private int _amount = -1;
    public int Amount
    {
        get { return this._amount; }
    }

    public int MaxResult(HeroInfoItem unit)
    {
        int result = 1000;
        int check = 1000;
        for (int i = 0; i < this._skills.Count; i++)
        {

            check = this._skills[i].GetMaxResult(unit.bindUnit);
            if (check < result)
                result = check;
        }

        return result;
    }

    public int MaxResult(List<HeroUnit> unit)
    {
        int result = 1000;
        int check = 1000;
        for (int i = 0; i < this._skills.Count; i++)
        {

            check = this._skills[i].GetMaxResult(unit);
            if (check < result)
                result = check;
        }

        return result;
    }

    public bool CheckResult
    {
        get
        {
            if (this._amount == 0)
            {
                Debug.LogError("Amount in check is 0");
                return true;
            }

            return this._amount >= this._finalResult;
        }
    }

    public static GroupSkillCheck Create()
    {
        return new GroupSkillCheck();
    }

    public static GroupSkillCheck Create(List<SkillCheckObject> list)
    {
        GroupSkillCheck temp = new GroupSkillCheck();
        temp._skills = new List<SkillCheckObject>();
        temp._skills.AddRange(list);

        if(temp._amount == -1)
        {
            for(int i = 0; i < temp._skills.Count; i++)
            {
                if (temp._skills[i].Amount > temp._amount)
                    temp._amount = temp._skills[i].Amount;
            }
        }

        return temp;
    }

    public GroupSkillCheck AddSkillObject(SkillCheckObject obj)
    {
        if (this._skills == null)
            this._skills = new List<SkillCheckObject>();

        this._skills.Add(obj);
        return this;
    }

    public void AddSkill(SkillCheckObject obj)
    {
        if (this._skills == null)
            this._skills = new List<SkillCheckObject>();

        this._skills.Add(obj);
    }

    public GroupSkillCheck SetAmount(int amount)
    {
        this._amount = amount;
        return this;
    }

    public void CompleteCheck(HeroUnit unit)
    {
        foreach(var skill in this._skills)
        {
            if (!unit.skills.ContainsKey(skill.Skill))
            {
                Debug.LogError("Unit: " + unit.Name + " has no skill: " + skill.Skill);
                return;
            }
        }

        this._finalResult = 0;
        int allRemainders = 0;

        int biggest = 0;
        for (int i = 0; i < this._skills.Count; i++)
        {
            this._skills[i].CompleteCheck(unit, false);
            this._combinedComplex += this._skills[i].Complex;
            this._complexResult += this._skills[i].ComplexResult;
            allRemainders += this._skills[i].Remainder;
            if (this._skills[i].ResultaAmount > biggest)
                biggest = this._skills[i].ResultaAmount;
        }

        bool check = true;
        for (int i = 0; i < biggest; i++)
        {

            for (int j = 0; j < this._skills.Count; j++)
            {
                if (this._skills[j].ResultaAmount < i)
                {
                    check = false;
                    break;
                }
            }

            if (check)
                this._finalResult += 1;
            else
                break;
        }

        if (allRemainders > 0)
        {
            double chance = allRemainders / this._combinedComplex;
            int chanceOne = (int)(Math.Round(chance, 2) * 100);

            if (UnityEngine.Random.Range(0, 100) <= chanceOne)
            {
                this._finalResult += 1;
                this._completeUnfullDice = true;
            }
        }
    }

    public void CompleteCheck(List<HeroUnit> units)
    {
        foreach (var skill in this._skills)
        {
            foreach(var unit in units)
            if (!unit.skills.ContainsKey(skill.Skill))
            {
                Debug.LogError("Unit: " + unit.Name + " has no skill: " + skill.Skill);
                return;
            }
        }

        this._finalResult = 0;
        int allRemainders = 0;

        int biggest = 0;
        for (int i = 0; i < this._skills.Count; i++)
        {
            this._skills[i].CompleteCheck(units, false);
            this._combinedComplex += this._skills[i].Complex;
            this._complexResult += this._skills[i].ComplexResult;
            allRemainders += this._skills[i].Remainder;
            if (this._skills[i].ResultaAmount > biggest)
                biggest = this._skills[i].ResultaAmount;
        }

        bool check = true;
        for (int i = 0; i < biggest; i++)
        {

            for (int j = 0; j < this._skills.Count; j++)
            {
                if (this._skills[j].ResultaAmount < i)
                {
                    check = false;
                    break;
                }
            }

            if (check)
                this._finalResult += 1;
            else
                break;
        }

        if (allRemainders > 0)
        {
            double chance = allRemainders / this._combinedComplex;
            int chanceOne = (int)(Math.Round(chance, 2) * 100);

            if (UnityEngine.Random.Range(0, 100) <= chanceOne)
            {
                this._finalResult += 1;
                this._completeUnfullDice = true;
            }
        }
    }
}