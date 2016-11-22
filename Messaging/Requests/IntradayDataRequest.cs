using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using BloombergAPIWrapper.Messaging.Responses;

namespace BloombergAPIWrapper.Messaging.Requests
{
    public class IntradayDataRequest<T> : Request<T>
    {
       
        protected override string GetOperationName() { return "IntradayBarRequest"; }
        

        protected override void AssignParameters(Request BloombergRequest)
        {

            BloombergRequest.Set("security", Security);
            BloombergRequest.Set("interval", Interval);
            BloombergRequest.Set("eventType", EventType);
            BloombergRequest.Set("startDateTime", new Datetime(TimeFrom.Year,
                                          TimeFrom.Month,
                                          TimeFrom.Day,
                                          TimeFrom.Hour, TimeFrom.Minute, TimeFrom.Second, TimeFrom.Millisecond));
            BloombergRequest.Set("endDateTime", new Datetime(TimeTo.Year,
                                          TimeTo.Month,
                                          TimeTo.Day,
                                          TimeTo.Hour, TimeTo.Minute, TimeTo.Second, TimeTo.Millisecond));
        }

        internal override Type GetUnderlyingResponseType()
        {
            return typeof(IntradayDataResponse<T>);
        }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public int Interval { get; set; }
        
        public string EventType { get; set; }

        public string Security { get; set; }
    }
}
