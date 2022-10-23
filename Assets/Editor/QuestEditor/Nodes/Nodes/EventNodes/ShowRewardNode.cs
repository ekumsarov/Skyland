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
    public class ShowRewardNode : BaseNode
    {
        static int ActionCount = 1;

        List<ShowRewardItemNode> _list;

        public static ShowRewardNode Create(Vector2 position)
        {
            ShowRewardNode temp = new ShowRewardNode();
            temp.GUID = "ShowRewardNode" + ActionCount;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;
            ActionCount += 1;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(position,
                new Vector2(200, 150)));

            var button = new Button(temp.AddItem)
            {
                text = "Add Item"
            };
            temp.titleButtonContainer.Add(button);

            temp._list = new List<ShowRewardItemNode>();

            return temp;
        }

        public static ShowRewardNode LoadNode(JSONNode data)
        {
            ShowRewardNode temp = new ShowRewardNode();

            temp.GUID = data["Node"]["GUID"].Value;
            temp.title = temp.GUID;
            temp.Type = NodeType.Event;

            temp.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            var inputPort = temp.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(float));
            inputPort.portName = "Input";
            temp.inputContainer.Add(inputPort);
            temp.RefreshExpandedState();
            temp.RefreshPorts();
            temp.SetPosition(new Rect(MyString.JSONToVector2(data["Node"]["Position"]),
                new Vector2(200, 150)));

            var button = new Button(temp.AddItem)
            {
                text = "Add Item"
            };
            temp.titleButtonContainer.Add(button);

            temp._list = new List<ShowRewardItemNode>();

            return temp;
        }

        public override JSONNode SerializeNode()
        {
            JSONNode baseNode = this.GetBaseNode("ShowReward");

            baseNode["NodeData"].Add("Base", "FlagWork");

            return baseNode;
        }

        public override JSONNode GetEvent()
        {
            JSONNode baseNode = new JSONClass();

            baseNode.Add("Base", "FlagWork");

            return baseNode;
        }

        public void AddItem()
        {
            _list.Add(ShowRewardItemNode.Create(this));
        }
    }

    public class ShowRewardItemNode
    {
        ShowRewardNode parent;

        EnumField representer;
        TextField iconID;

        FloatField firstVal;
        FloatField secondVal;

        Button _removeButton;

        public static ShowRewardItemNode Create(ShowRewardNode parent)
        {
            ShowRewardItemNode temp = new ShowRewardItemNode();
            temp.parent = parent;
            
            temp.representer = new EnumField("Representer", Represent.Type.Non);
            temp.iconID = new TextField("Icon ID");

            temp._removeButton = new Button(temp.Clean)
            {
                text = "Remmove"
            };

            parent.Add(temp._removeButton);
            parent.Add(temp.representer);
            parent.Add(temp.iconID);

            return temp;
        }

        public void Clean()
        {
            parent.Remove(representer);
            parent.Remove(iconID);

        }
    }
}