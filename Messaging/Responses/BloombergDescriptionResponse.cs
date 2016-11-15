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
    /// Bloomberg description response type, processes messages with description content, cracked and parsed result is accessible direcly in class through indexer or properties.
    /// </summary>
    /// <typeparam name="T">Custom type with defined attributes that determine how is message parsed.</typeparam>
    public class BloombergDescriptionResponse<T> : BloombergResponse<T>
    {
        Type t = typeof(T);
        Dictionary<string, T> data = new Dictionary<string, T>();
        public BloombergDescriptionResponse(object RequestClass) : base(RequestClass) { }
        public T GetDataObject(string security) {
            if (data.ContainsKey(security))
                return data[security];
            else return default(T);
        }
        public T this[string security]{
           
            get { 
                if(data.ContainsKey(security))
                  return data[security];
                else return default(T);
            }
        }


        /// <summary>
        /// Processes the messages of type description.
        /// </summary>
        internal override void ProcessMessage(Message message) {
            #region Resolving custom attribute setup
            // Root element
         
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
            #endregion
                    data.Add(securityName,
                     ProcessFields(fieldData));
                }
            
        }
    }
}
