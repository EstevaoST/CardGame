using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Globals
{
    public enum TurnPhase
    { Upkeep, Draw, Plan, Execution, End };

    [Serializable]
    public struct GameState
    {
        public int TurnOwnerIndex;
        public TurnPhase phase;
        public int turnCount;
    }

    [Serializable]
    public struct CardReference
    {
        public int collection;
        public int number;
        public bool revealed;
    }

    public struct Utils
    {
        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
        public static T DeserializeObject<T>(string toDeserialize)
        {            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (TextReader textReaderr = new StringReader(toDeserialize))
            {
                return (T)xmlSerializer.Deserialize(textReaderr);                
            }
        }
    }
}


