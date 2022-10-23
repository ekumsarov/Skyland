using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SimpleJSON;

namespace QuestEditor
{
    public class ActionChoicePort
    {

        public string CallData;
        public NodeType Type = NodeType.Action;
        public string GUID;

        private TextField _textIDView;
        private TextField _callDataView;
        private EnumField _typeView;

        private ActionNode _parent;
        public Port _port;

        private Dictionary<string, ConditionTypes> _conditions;

        public static ActionChoicePort Create(ActionNode parent, string choiceID)
        {
            ActionChoicePort temp = new ActionChoicePort();
            temp._parent = parent;
            temp.GUID = choiceID;

            temp._port = parent.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            temp._port.portName = choiceID;

            temp._typeView = new EnumField("Type: ", temp.Type);
            temp._typeView.RegisterValueChangedCallback(evt => { temp.Type = (NodeType)Enum.Parse(typeof(NodeType), evt.newValue.ToString()); });
            temp._port.contentContainer.Add(temp._typeView);

            temp._callDataView = new TextField("CallData: ");
            temp._callDataView.RegisterValueChangedCallback(evt =>
            {
                temp.CallData = evt.newValue;
            });
            temp._callDataView.SetValueWithoutNotify(string.Empty);
            temp._port.contentContainer.Add(temp._callDataView);

            var textField = new TextField("ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp._port.portName = evt.newValue;
                temp.GUID = evt.newValue;
            });
            textField.SetValueWithoutNotify(choiceID);
            temp._port.contentContainer.Add(textField);

            var deleteButton = new Button(() => temp.CleanPort())
            {
                text = "Remove"
            };
            temp._port.contentContainer.Add(deleteButton);

            var aCondition = new Button(() => temp.AddCondition())
            {
                text = "Add Condition"
            };
            temp._port.contentContainer.Add(aCondition);


            temp._port.style.flexDirection = FlexDirection.ColumnReverse;
            temp._port.style.height = new StyleLength(StyleKeyword.Auto);
            temp._port.style.flexWrap = Wrap.Wrap;
            var portLabel = temp._port.contentContainer.Q<Label>("type");
            temp._port.contentContainer.Remove(portLabel);

            temp._port.contentContainer.Insert(0, new Label("   "));

            parent.outputContainer.Add(temp._port);

            return temp;
        }

        public static ActionChoicePort LoadPort(ActionNode parent, JSONNode data)
        {
            ActionChoicePort temp = new ActionChoicePort();

            temp._parent = parent;
            temp.GUID = data["ID"].Value;
            temp.Type = (NodeType)Enum.Parse(typeof(NodeType), data["Type"].Value);

            temp._port = parent.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(float));
            temp._port.portName = temp.GUID;

            temp._typeView = new EnumField("Type: ", temp.Type);
            temp._typeView.RegisterValueChangedCallback(evt => { temp.Type = (NodeType)Enum.Parse(typeof(NodeType), evt.newValue.ToString()); });
            temp._port.contentContainer.Add(temp._typeView);

            temp._callDataView = new TextField("CallData: ");
            temp._callDataView.RegisterValueChangedCallback(evt =>
            {
                temp.CallData = evt.newValue;
            });
            temp._callDataView.SetValueWithoutNotify(data["CallData"].Value);
            temp._port.contentContainer.Add(temp._callDataView);

            var textField = new TextField("ID: ");
            textField.RegisterValueChangedCallback(evt =>
            {
                temp._port.portName = evt.newValue;
                temp.GUID = evt.newValue;
            });
            textField.SetValueWithoutNotify(temp.GUID);
            temp._port.contentContainer.Add(textField);

            var deleteButton = new Button(() => temp.CleanPort())
            {
                text = "Remove"
            };
            temp._port.contentContainer.Add(deleteButton);

            var aCondition = new Button(() => temp.AddCondition())
            {
                text = "Add Condition"
            };
            temp._port.contentContainer.Add(aCondition);



            temp._port.style.flexDirection = FlexDirection.ColumnReverse;
            temp._port.style.height = new StyleLength(StyleKeyword.Auto);
            temp._port.style.flexWrap = Wrap.Wrap;
            var portLabel = temp._port.contentContainer.Q<Label>("type");
            temp._port.contentContainer.Remove(portLabel);

            temp._port.contentContainer.Insert(0, new Label("   "));

            if(data["Conditions"]!=null)
            {
                temp._conditions = new Dictionary<string, ConditionTypes>();

                JSONArray arrayConditions = data["Conditions"].AsArray;
                for(int i = 0; i < arrayConditions.Count; i++)
                {
                    temp._conditions.Add(arrayConditions[i]["ID"], ConditionTypes.LoadCondition(arrayConditions[i], temp, temp._port));
                }

            }

            parent.outputContainer.Add(temp._port);

            return temp;
        }

        public void AddCondition()
        {
            if (this._conditions == null)
                this._conditions = new Dictionary<string, ConditionTypes>();

            string conditionID = $"Condition{this._conditions.Count}";
            this._conditions.Add(conditionID, ConditionTypes.Create(conditionID, this, _port));
        }

        public void RemoveCondition(string con)
        {
            this._conditions.Remove(con);
        }

        public void SetCallData(string data)
        {
            this.CallData = data;
            this._callDataView.SetValueWithoutNotify(data);
        }

        public void SetChoiceType(NodeType type)
        {
            this.Type = type;
            this._typeView.SetValueWithoutNotify(type);
        }

        public void CleanPort()
        {
            QEV.Editor.RemovePort(_parent, _port);
            _parent.RemovePort(this);
        }

        public bool EqualsPort(Port port)
        {
            return port == _port;
        }

        public JSONNode GetNode()
        {
            JSONNode node = new JSONClass();

            node.Add("ID", this.GUID);

            if(this.Type == NodeType.Context || this.Type == NodeType.Action)
                node.Add("Type", "Action");
            else 
                node.Add("Type", this.Type.ToString());

            if (!this.CallData.IsNullOrEmpty())
             node.Add("CallData", this.CallData);

            JSONArray array = new JSONArray();
            if (this._conditions != null)
            {
                foreach (var condition in this._conditions)
                {
                    array.Add(condition.Value.GetNode());
                }
            }

            if (array.Count > 0)
                node.Add("Conditions", array);

            return node;
        }
    }


    public class ConditionTypes
    {
        private string ID;

        private ActionChoicePort _parent;
        private Port _port;

        private EnumField _qualityType;
        private EnumField _conType;
        private Label _lineLableTop;
        private Label _lineLabelBottom;

        private TextField _conditionID;
        private IntegerField _intField;
        private Toggle _boolValue;
        private Toggle _isVisibleField;

        private Button _deleteButton;

        public static ConditionTypes Create(string ID, ActionChoicePort parent, Port _port)
        {
            ConditionTypes temp = new ConditionTypes();

            temp.ID = ID;
            temp._parent = parent;
            temp._port = _port;

            temp._lineLableTop = new Label("_____");
            temp._lineLabelBottom = new Label("_____");

            _port.Insert(2, temp._lineLableTop);

            temp._deleteButton = new Button(() => temp.RemoveCondition())
            {
                text = "Remove Condition"
            };
            _port.contentContainer.Insert(2, temp._deleteButton);

            temp._conType = new EnumField(ConditionType.NoType);
            temp._conType.RegisterValueChangedCallback(evt => temp.SetupCodition(evt.newValue.ToString()));

            temp._qualityType = new EnumField(StatCondition.StatConType.Equal);
            temp._intField = new IntegerField("Value");


            temp._conditionID = new TextField("");
            temp._boolValue = new Toggle();
            temp._isVisibleField = new Toggle("IsVisible");
            temp._isVisibleField.SetValueWithoutNotify(true);

            _port.Insert(2, temp._isVisibleField);
            _port.Insert(2, temp._conditionID);
            _port.Insert(2, temp._boolValue);
            _port.Insert(2, temp._conditionID);
            _port.Insert(2, temp._intField);
            _port.Insert(2, temp._qualityType);
            _port.Insert(2, temp._conType);

            temp._conditionID.visible = false;
            temp._boolValue.visible = false;
            temp._intField.visible = false;
            temp._qualityType.visible = false;

            _port.Insert(2, temp._lineLabelBottom);

            return temp;
        }

        public static ConditionTypes LoadCondition(JSONNode data, ActionChoicePort parent, Port port)
        {
            ConditionTypes temp = new ConditionTypes();

            temp.ID = data["ID"];
            temp._parent = parent;
            temp._port = port;

            temp._lineLableTop = new Label("_____");
            temp._lineLabelBottom = new Label("_____");

            port.Insert(2, temp._lineLableTop);

            temp._deleteButton = new Button(() => temp.RemoveCondition())
            {
                text = "Remove Condition"
            };
            port.contentContainer.Insert(2, temp._deleteButton);

            temp._conType = new EnumField((ConditionType)Enum.Parse(typeof(ConditionType), data["Type"].Value));
            temp._conType.RegisterValueChangedCallback(evt => temp.SetupCodition(evt.newValue.ToString()));


            temp._conditionID = new TextField("");
            if (!data["ConditionID"].Value.IsNullOrEmpty())
                temp._conditionID.SetValueWithoutNotify(data["ConditionID"].Value);

            temp._boolValue = new Toggle();
            temp._boolValue.SetValueWithoutNotify(data["isOn"].AsBool);

            if (!data["QualityType"].Value.IsNullOrEmpty())
                temp._qualityType = new EnumField((StatCondition.StatConType)Enum.Parse(typeof(StatCondition.StatConType), data["QualityType"].Value));
            else
                temp._qualityType = new EnumField(StatCondition.StatConType.Equal);

            temp._intField = new IntegerField("Value");
            if (!data["Value"].Value.IsNullOrEmpty())
                temp._intField.SetValueWithoutNotify(data["Value"].AsInt);

            temp._isVisibleField = new Toggle("IsVisible");
            temp._isVisibleField.SetValueWithoutNotify(data["isVisibleCondition"].AsBool);

            port.Insert(2, temp._isVisibleField);
            port.Insert(2, temp._conditionID);
            port.Insert(2, temp._intField);
            port.Insert(2, temp._qualityType);
            port.Insert(2, temp._boolValue);
            port.Insert(2, temp._conType);

            if(temp._conType.value.ToString().Equals("NoType"))
            {
                temp._conditionID.visible = false;
                temp._boolValue.visible = false;
            }

            port.Insert(2, temp._lineLabelBottom);

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
            }
            else if (chType == ConditionType.Flag)
            {
                this._conditionID.label = "Flag ID: ";

                this._boolValue.value = true;

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
            }
            else if (chType == ConditionType.Stat)
            {
                this._conditionID.label = "Stat ID: ";

                this._conditionID.visible = true;
                this._boolValue.visible = false;
                this._intField.visible = true;
                this._qualityType.visible = true;
            }
            else if (chType == ConditionType.Loot)
            {
                this._conditionID.label = "Loot ID: ";

                this._boolValue.value = true;

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
            }
            else if (chType == ConditionType.Quest)
            {
                this._conditionID.label = "Quest ID: ";

                this._boolValue.value = true;

                this._conditionID.visible = true;
                this._boolValue.visible = true;
                this._intField.visible = false;
                this._qualityType.visible = false;
            }
        }

        public void RemoveCondition()
        {
            _port.Remove(this._isVisibleField);
            _port.Remove(this._lineLableTop);
            _port.Remove(this._lineLabelBottom);
            _port.Remove(this._conditionID);
            _port.Remove(this._boolValue);
            _port.Remove(this._deleteButton);
            _port.Remove(this._conType);
            _port.Remove(this._intField);
            _port.Remove(this._qualityType);
            _parent.RemoveCondition(this.ID);
        }

        public JSONNode GetNode()
        {
            JSONNode node = new JSONClass();

            node.Add("ID", this.ID);
            node.Add("isVisibleCondition", this._isVisibleField.value.ToString());
            node.Add("Type", this._conType.value.ToString());
            node.Add("ConditionID", this._conditionID.value);
            node.Add("isOn", this._boolValue.value.ToString());
            node.Add("Value", this._intField.value.ToString());
            node.Add("QualityType", this._qualityType.value.ToString());

            return node;
        }
    }
}