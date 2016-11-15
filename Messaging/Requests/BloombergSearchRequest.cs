using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using BloombergAPIWrapper.Messaging.Responses;

namespace BloombergAPIWrapper.Messaging.Requests
{
    public class BloombergSearchRequest<BloombergSearchObject> : BloombergRequest<BloombergSearchObject>
    {
        public string Domain { get; set; }
        int limit = 5000;
        public int Limit { get { return limit; } set { limit = value; } }
        /*internal override SessionOptions GetSessionOptions() { 
            var sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = "localhost";
            sessionOptions.ServerPort = 8194;
            return sessionOptions;
        }*/
        internal override string GetService() { return "//blp/exrsvc"; }
        protected override string GetOperationName() { return "ExcelGetGridRequest"; }
        
        protected override void AssignParameters(Request BloombergRequest)
        {
            Element securities = BloombergRequest.GetElement("Domain");
            securities.SetValue(Domain);
            //Element option = BloombergRequest.GetElement("Option");
            //option.AppendValue(String.Format("Limit={0}", Limit));
        }

        internal override Type GetUnderlyingResponseType()
        {
            return typeof(BloombergSearchResponse<BloombergSearchObject>);
        }
    }
}
