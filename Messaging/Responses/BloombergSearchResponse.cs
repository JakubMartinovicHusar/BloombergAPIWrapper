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
    public class BloombergSearchResponse<BloombergSearchObject> : BloombergResponse<BloombergSearchObject>, IEnumerable<BloombergSearchObject>
    {
        Type t = typeof(BloombergSearchObject);
        List<BloombergSearchObject> data = new List<BloombergSearchObject>();
        public BloombergSearchResponse(object RequestClass) : base(RequestClass) { }
       
        public BloombergSearchObject this[int index]
        {
           
            get { 
                if(data.Count<index)
                  return data[index];
                else return default(BloombergSearchObject);
            }
        }

        public int Count { get { return data.Count; } }


        /// <summary>
        /// Processes the messages of type description.
        /// </summary>
        internal override void ProcessMessage(Message message) {
            #region Resolving custom attribute setup
            // Root element

            Element dataList = message.GetElement("DataRecords");
                CheckErrors(dataList);
                for (int i = 0; i < dataList.NumValues; i++)
                {
                    Element sdElement = null;
                    if (dataList.IsArray)
                        sdElement = (Element)dataList[i];
                    else
                        sdElement = (Element)dataList;
                    CheckErrors(sdElement);
                    Element fieldData = sdElement.GetElement("DataFields");
                    if (fieldData.IsArray) {
                        for (int j = 0; j < fieldData.NumValues; j++)
                        {
                            data.Add(ProcessFields((Element)fieldData[j])); 
                        }
                    }
                    else data.Add(ProcessFields(fieldData));
            #endregion

                    
                }

            
        }

        public IEnumerator<BloombergSearchObject> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
