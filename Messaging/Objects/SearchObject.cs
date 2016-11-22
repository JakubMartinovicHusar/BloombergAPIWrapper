using BloombergAPIWrapper.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloombergAPIWrapper.Messaging.Objects
{
    [DataClassAttribute]
    public class SearchObject
    {
        [DataAttribute(FieldName = "StringValue", Fillable = false)]
        public string SecurityCode { get; set; }
    }
}
