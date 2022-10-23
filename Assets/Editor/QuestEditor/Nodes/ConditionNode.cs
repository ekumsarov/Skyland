using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SimpleJSON;
using Lodkod;

namespace QuestEditor
{
    public class ConditionNode
    {
        public string ID;

        private BaseNode _parent;

        private EnumField _conType;
        private EnumField _qualityType;
        private EnumField _daypartType;
        private Label _lineLableTop;
        private Label _lineLabelBottom;

        private TextField _conditionID;
        private IntegerField _intField;
        private Toggle _boolValue;

        private Button _deleteButton;

        public static ConditionNode Create(string ID, BaseNode parent)
        {
            ConditionNode temp = new ConditionNode();

            temp.ID = ID;
            temp._parent = parent;

            temp._lineLableTop = new Label("_____");
            temp._lineLabelBottom = new Label("_____");

            

            temp._deleteButton = new Button(() => temp.RemoveCondition())
            {
                text = "Remove Condition"
            };
            

            temp._conType = new EnumField(ConditionType.NoType);
            temp._conType.RegisterValueChangedCallback(evt => temp.SetupCodition(evt.newValue.ToString()));

            temp._qualityType = new EnumField(StatCondition.StatConType.Equal);
            temp._daypartType = new EnumField(DayPart.Afternoon);


            temp._conditionID = new TextField("");
            temp._boolValue = new Toggle();
            temp._intField = new IntegerField("Value");

            temp._parent.contentContainer.Add(temp._lineLableTop);
            temp._parent.contentContainer.Add(temp._deleteButton);
            temp._parent.contentContainer.Add(temp._conType);
            temp._parent.contentContainer.Add(temp._qualityType);
            temp._parent.contentContainer.Add(temp._daypartType);
            temp._parent.contentContainer.Add(temp._conditionID);
            temp._parent.contentContainer.Add(temp._intField);
            temp._parent.contentContainer.Add(temp._boolValue);
            temp._parent.contentContainer.Add(temp._lineLabelBottom);


            temp._conditionID.visible = false;
            temp._boolValue.visible = false;
            temp._intField.visible = false;
            temp._qualityType.visible = false;


            return temp;
        }

        public static ConditionNode LoadCondition(JSONNode data, BaseNode parent)
        {
            ConditionNode temp = new ConditionNode();

            temp.ID = data["ID"];
            temp._parent = parent;

            temp._lineLableTop = new Label("_____");
            temp._lineLabelBottom = new Label("_____");

            temp._deleteButton = new Button(() => temp.RemoveCondition())
            {
                text = "Remove Condition"
            };

            temp._conType = new EnumField((ConditionType)Enum.Parse(typeof(ConditionType), data["Type"].Value));
            temp._conType.RegisterValueChangedCallback(evt => temp.SetupCodition(evt.newValue.ToString()));


            temp._conditionID = new TextField("");
            if(!data["ConditionID"].Value.IsNullOrEmpty())
                temp._conditionID.SetValueWithoutNotify(data["ConditionID"].Value);

            temp._boolValue = new Toggle();
            temp._boolValue.SetValueWithoutNotify(data["isOn"].AsBool);

            if (!data["QualityType"].Value.IsNullOrEmpty())
                temp._qualityType = new EnumField((StatCondition.StatConType)Enum.Parse(typeof(StatCondition.StatConType), data["QualityType"].Value));
            else
                temp._qualityType = new EnumField(StatCondition.StatConType.Equal);

            if (!data["DayPart"].Value.IsNullOrEmpty())
                temp._daypartType = new EnumField((DayPart)Enum.Parse(typeof(DayPart), data["DayPart"].Value));
            else
                temp._daypartType = new EnumField(DayPart.Afternoon);

            temp._intField = new IntegerField("Value");
            if (!data["Value"].Value.IsNullOrEmpty())
                temp._intField.SetValueWithoutNotify(data["Value"].AsInt);

            temp._parent.contentContainer.Add(temp._lineLableTop);
            temp._parent.contentContainer.Add(temp._deleteButton);
            temp._parent.contentContainer.Add(temp._conType);
            temp._parent.contentContainer.Add(temp._qualityType);
            temp._parent.contentContainer.Add(temp._daypartType);
            temp._parent.contentContainer.Add(temp._conditionID);
            temp._parent.contentContainer.Add(temp._intField);
            temp._parent.contentContainer.Add(temp._boolValue);
            temp._parent.contentContainer.Add(temp._lineLabelBottom);

            temp.SetupCodition(data["Type"].Value);

            return temp;
        }

        public void SetupCodition(string type)
        {
            ConditionType chType = (ConditionType)Enum.Parse(typeof(ConditionType), type);

            if (chType == ConditionType.NoType)
            {
                this._conditionID.visible = false;
                this._boolValue.visible = false;
                this._intField.visible = false;
                this._qualityType.visible = false;
                this._daypartType.visible = false;
            }
            else if (chType == ConditionType.Flag)
            {
                this._conditionID.label = "Flag ID: ";

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
                this._daypartType.visible = false;
            }
            else if (chType == ConditionType.Stat)
            {
                this._conditionID.label = "Stat ID: ";

                this._conditionID.visible = true;
                this._boolValue.visible = false;
                this._intField.visible = true;
                this._qualityType.visible = true;
                this._daypartType.visible = false;
            }
            else if(chType == ConditionType.Loot)
            {
                this._conditionID.label = "Loot ID: ";

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
                this._daypartType.visible = false;
            }
            else if (chType == ConditionType.Quest)
            {
                this._conditionID.label = "Quest ID: ";

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
                this._daypartType.visible = false;
            }
            else if (chType == ConditionType.Daypart)
            {
                this._conditionID.visible = false;
                this._boolValue.visible = false;
                this._intField.visible = false;
                this._qualityType.visible = true;
                this._daypartType.visible = true;
            }
        }

        public void RemoveCondition()
        {
            _parent.Remove(this._lineLableTop);
            _parent.Remove(this._lineLabelBottom);
            _parent.Remove(this._conditionID);
            _parent.Remove(this._boolValue);
            _parent.Remove(this._deleteButton);
            _parent.Remove(this._conType);
            _parent.Remove(this._intField);
            _parent.Remove(this._qualityType);
            _parent.Remove(this._daypartType);
            _parent.RemoveCondition(this.ID);
        }

        public JSONNode GetNode()
        {
            JSONNode node = new JSONClass();

            node.Add("ID", this.ID);
            node.Add("Type", this._conType.value.ToString());
            node.Add("ConditionID", this._conditionID.value);
            node.Add("isOn", this._boolValue.value.ToString());
            node.Add("Value", this._intField.value.ToString());
            node.Add("QualityType", this._qualityType.value.ToString());
            node.Add("DayPart", this._daypartType.value.ToString());

            return node;
        }
    }
}