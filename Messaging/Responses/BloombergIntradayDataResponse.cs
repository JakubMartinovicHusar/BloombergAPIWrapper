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
    /// Bloomberg intraday data response type, processes messages with description content, cracked and parsed result is accessible direcly in class through indexer or properties.
    /// </summary>
    /// <typeparam name="T">Custom type with defined attributes that determine how is message parsed.</typeparam>
    public class BloombergIntradayDataResponse<T> : BloombergResponse<T>
    {

        public BloombergIntradayDataResponse(object RequestClass) : base(RequestClass) { }

        Type t = typeof(T);
        List<T> data = new List<T>();

        public T this[int index]
        {
            get { return data[index]; }
        }

        /// <summary>
        /// Processes the Intraday data messages.
        /// </summary>
        internal override void ProcessMessage(Message message)
        {
           
                Element barData = message.GetElement("barData");
                CheckErrors(barData);
                Element barTickData = barData.GetElement("barTickData");
                CheckErrors(barTickData);
                for (int j = 0; j < barTickData.NumValues; j++)
                    {
                        Element el = (Element)barTickData[j];
                        CheckErrors(el);
                        data.Add(ProcessFields(el));
                    }
                    
            
        
        }

        /// <summary>
        /// Response data in form of list.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public List<T> Data { get { return data; } }
    }
}
