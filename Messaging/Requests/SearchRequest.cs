using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using BloombergAPIWrapper.Messaging.Responses;

namespace BloombergAPIWrapper.Messaging.Requests
{
    public class SearchRequest<SearchObject> : Request<SearchObject>
    {
        public string Domain { get; set; }
        int limit = 5000;
        public int Limit { get { return limit; } set { limit = value; } }

        internal override string GetService() { return "//blp/exrsvc"; }
        protected override string GetOperationName() { return "ExcelGetGridRequest"; }
        
        protected override void AssignParameters(Request BloombergRequest)
        {
            Element securities = BloombergRequest.GetElement("Domain");
            securities.SetValue(Domain);
        }

        internal override Type GetUnderlyingResponseType()
        {
            return typeof(SearchResponse<SearchObject>);
        }
    }
}
