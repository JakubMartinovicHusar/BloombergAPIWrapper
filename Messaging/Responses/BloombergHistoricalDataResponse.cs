using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using System.Reflection;
using BloombergAPIWrapper.Data.Attributes;

namespace BloombergAPIWrapper.Messaging.Responses
{
    /// <summary>
    /// Bloomberg historical data response type, processes messages with description content, cracked and parsed result is accessible direcly in class through indexer or properties.
    /// </summary>
    /// <typeparam name="T">Custom type with defined attributes that determine how is message parsed.</typeparam>
    public class BloombergHistoricalDataResponse<T> : BloombergResponse<T>
    {
        Type t = typeof(T);
        Dictionary<string, List<T>> data = new Dictionary<string, List<T>>();

        public BloombergHistoricalDataResponse(object RequestClass) : base(RequestClass) { }

        public List<T> this[string security]
        {
            get {
                if (data.ContainsKey(security))
                    return data[security];
                else return new List<T>();
            }
        }

        /// <summary>
        /// Processes the historical data messages.
        /// </summary>
        internal override void ProcessMessage(Message message) {
            
                Element securityData = message.GetElement("securityData");
                CheckErrors(securityData);
                for (int i = 0; i < securityData.NumValues; i++)
                {
                    Element sdElement = null;
                    if (securityData.IsArray)
                        sdElement = (Element)securityData[i];
                    else
                        sdElement = (Element)securityData;
                    CheckErrors(sdElement);
                    string securityName = sdElement.GetElementAsString("security");
                    Element fieldData = sdElement.GetElement("fieldData");
                    CheckErrors(fieldData);
                    List<T> ldata = new List<T>();
                    for (int j = 0; j < fieldData.NumValues; j++)
                    {
                        Element el = (Element)fieldData[j];
                        CheckErrors(el);
                        ldata.Add(ProcessFields(el));
                    }
                    data.Add(securityName, ldata);
                
            }
        }
    }
}
