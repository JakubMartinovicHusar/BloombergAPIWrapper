using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BloombergAPIWrapper.Data.Attributes
{
    /// <summary>
    /// Bloomberg data attribute only assignable to property defines the data that should be serving as parameters, fields or overrides
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BloombergDataAttribute : Attribute
    {
        /// <summary>
        /// The fillable option specifies if this should be filled to the request
        /// </summary>
        public bool Fillable = true;
        /// <summary>
        /// The retrievable option specifies if this should be retrieved from the response, in some cases respond supplies unfilled fields in request so it's not dependet on fillable option
        /// </summary>
        public bool Retrievable = true;
        /// <summary>
        /// The name of bloomberg field
        /// </summary>
        public string FieldName = null;
        /// <summary>
        /// The name if field that is retrurned in response (in some cases is not equal to request so FieldName != ReturnName)
        /// </summary>
        public string ReturnName = null;

        /// <summary>
        /// The type of field which is in use
        /// </summary>
        public BloombergDataAttributeType Type = BloombergDataAttributeType.Field;

        /// <summary>
        /// Specifies if type is an array.
        /// </summary>
        public bool IsArray = false;

        /// <summary>
        /// The array underlying type
        /// </summary>
        public Type ArrayUnderlyingType;

        /// <summary>
        /// The array field names, should only be used if array is not a class.
        /// </summary>
        public string ArrayFieldName;


    }

    /// <summary>
    /// General definitions for cracking of the message on class level
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BloombergDataClassAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies type of fields, which could be field or override
    /// </summary>
    public enum BloombergDataAttributeType
    {
        [Description("fields")]
        Field,
        [Description("override")]
        Override,
        [Description("parameter")]
        Parameter
    }


}
