using BloombergAPIWrapper.Data.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloombergAPIWrapper.Messaging.Objects
{
    [BloombergDataClassAttribute]
    public class BloombergSearchObject
    {
        [BloombergDataAttribute(FieldName = "StringValue", Fillable = false)]
        public string SecurityCode { get; set; }
    }
}
