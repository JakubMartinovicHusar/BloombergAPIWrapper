using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using BloombergAPIWrapper.Messaging.Responses;

namespace BloombergAPIWrapper.Messaging.Requests
{
    public class BloombergHistoricalDataRequest<T> : BloombergRequest<T>
    {
        List<string> listOfSecurities = new List<string>();
        
        protected override string GetOperationName() { return "HistoricalDataRequest"; }
        
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
            BloombergRequest.Set("periodicitySelection", Periodicity);
            BloombergRequest.Set("startDate", TimeFrom.ToString("yyyyMMdd"));
            BloombergRequest.Set("endDate", TimeTo.ToString("yyyyMMdd"));
        }

        internal override Type GetUnderlyingResponseType()
        {
            return typeof(BloombergHistoricalDataResponse<T>);
        }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public string Periodicity { get; set; }
    }
}
