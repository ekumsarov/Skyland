using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventPackSystem
{
    using SimpleJSON;
    using System.IO;

    public class EventPack {

        public string ID { set; get; }
        public List<JSONNode> events;

        public static EventPack Create(string ID, List<JSONNode> list)
        {
            EventPack temp = new EventPack();
            temp.events = list;
            return temp;
        }

        public virtual void Create()
        {
            events = new List<JSONNode>();
        }

        public JSONNode Make(string parse)
        {
            try
            {
                return JSON.Parse(parse.Replace("'", "\""));
            }
            catch (IOException e)
            {
                Debug.Log(e);
                throw;
            }
            
        }

        public static JSONNode Node(string parse)
        {
            try
            {
                return JSON.Parse(parse.Replace("'", "\""));
            }
            catch (IOException e)
            {
                Debug.Log(e);
                throw;
            }

        }

        public void AddNode(string parse)
        {
            try
            {
                events.Add(JSON.Parse(parse.Replace("'", "\"")));
            }
            catch (IOException e)
            {
                Debug.Log(e);
                throw;
            }
        }
    }

    
}