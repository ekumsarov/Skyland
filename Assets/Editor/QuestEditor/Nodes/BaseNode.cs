using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using SimpleJSON;

namespace QuestEditor
{
    public class BaseNode : Node
    {
        public string GUID;
        public bool EntyPoint = false;
        public Vector2 Position;
        public NodeType Type = NodeType.Action;   

        public virtual JSONNode SerializeNode()
        {
            return null;
        }

        protected JSONNode GetBaseNode(string type)
        {
            JSONNode node = new JSONClass();
            JSONNode nodeInfo = new JSONClass();

            nodeInfo.Add("NodeType", Type.ToString());
            nodeInfo.Add("Type", type);
            nodeInfo.Add("GUID", GUID);
            nodeInfo.Add("EntryPoint", EntyPoint.ToString());
            nodeInfo.Add("Position", MyString.RectToJSON(this.GetPosition()));

            node.Add("Node", nodeInfo);
            node.Add("NodeData", new JSONClass());

            return node;
        }

        public virtual JSONNode GetEvent()
        {
            return null;
        }

        public virtual Port GetOuputPort(string portID)
        {
            return null;
        }

        public virtual void SetupConnectedNode(BaseNode node, Port updatedPort)
        {

        }

        public virtual void RemoveCondition(string ID)
        {

        }
    }
}