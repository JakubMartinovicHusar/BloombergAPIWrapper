using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using BloombergAPIWrapper.Messaging.Responses;

namespace BloombergAPIWrapper.Messaging.Requests
{
    public class BloombergDescriptionRequest<T> : BloombergRequest<T>
    {
        List<string> listOfSecurities = new List<string>();
        
        protected override string GetOperationName() { return "ReferenceDataRequest"; }
        
        public void AddSecurity(string Security) {
            listOfSecurities.Add(Security);
        }

        protected override void AssignParameters(Request BloombergRequest)
        {
            if (listOfSecurities != null)
                foreach (string security in listOfSecurities)
                {
                    BloombergRequest.Append("securities", security);
                }
        }

        internal override Type GetUnderlyingResponseType()
        {
            return typeof(BloombergDescriptionResponse<T>);
        }
    }
}
