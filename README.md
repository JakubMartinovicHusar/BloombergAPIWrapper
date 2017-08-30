# BloombergAPIWrapper
Bloomberg API Wrapper, dynamic class parsing with reflection makes communication of your programs and BL terminal easy as never before 


Bloomberg API Wrapper

How to use it

Initialize request with as a object HistoricalDataRequest. Add request details and call Engine.RetrieveData. Generic class should be class you want to fill with the data. The response will be of corresponding format. List or class filled. Overrides are possible. with an attribute.

Requests available for use: 
DescriptionRequest
HistoricalDataRequest
IntradayDataRequest
IntradayTickDataRequest

Every request has it's response 

HistoricalDataRequest req = new HistoricalDataRequest();
            req.Periodicity = "DAILY";
            req.TimeFrom = From;
            req.TimeTo = To;
            req.AddSecurity(Symbol);
            var res = (HistoricalDataResponse)conn.Engine.RetrieveData(req);
            if (res[Symbol] != null)
                foreach (Data d in res[Symbol])
                {
                    result1.Add(d.Time.ToShortDateString());
                    result2.Add(d.Price);
                }
        resv[0] = result1.ToArray();
        resv[1] = result2.ToArray();

Generic class - data definition

It's pretty simple. Mark properties with attributes specify field name if other than property name and specify if override.


  public class Data : DataClassAttribute
    {
        [DataAttribute(FieldName = "date")]
        public DateTime Time { get; set;}
        [DataAttribute(FieldName = "PX_LAST")]
        public double Price { get; set; }
}

public class BondInfo : DataClassAttribute {
    [DataAttribute(FieldName = "ISSUER")]
    public string Issuer { get; set; }
    [DataAttribute(FieldName = "GUARANTOR")]
    public string Guarantor { get; set; }
    [DataAttribute(FieldName = "COLLAT_TYP")]
    public string Collateral { get; set; }
    [DataAttribute(FieldName = "YRS_TO_MTY_ISSUE")]
    public double Maturity { get; set; }

    [DataAttribute(FieldName = "MATURITY")]
    public DateTime MaturityDate { get; set; }

    public string Bucket { get {
        if (Maturity <= 1) return "1Y";
        else if (Maturity <= 3) return "3Y";
        else if (Maturity <= 3) return "3Y";
        else if (Maturity <= 5) return "5Y";
        else if (Maturity <= 7) return "7Y";
        else if (Maturity <= 9) return "9Y";
        else if (Maturity <= 12) return "12Y";
        else if (Maturity <= 15) return "15Y";
        else if (Maturity <= 20) return "20Y";
        else if (Maturity <= 25) return "25Y";
        else if (Maturity <= 30) return "30Y";
        else return "50Y";
    } }

Short definition of attribute DataAttribute


     /// 

    /// Bloomberg data attribute only assignable to property defines the data that should be serving as parameters, fields or overrides
    /// 

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class DataAttribute : Attribute
    {
        /// 

        /// The fillable option specifies if this should be filled to the request
        /// 

        public bool Fillable = true;
        /// 

        /// The retrievable option specifies if this should be retrieved from the response, in some cases respond supplies unfilled fields in request so it's not dependet on fillable option
        /// 

        public bool Retrievable = true;
        /// 

        /// The name of bloomberg field
        /// 

        public string FieldName = null;
        /// 

        /// The name if field that is retrurned in response (in some cases is not equal to request so FieldName != ReturnName)
        /// 

        public string ReturnName = null;
    /// <summary>
    /// The type of field which is in use
    /// </summary>
    public DataAttributeType Type = DataAttributeType.Field;

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
public class DataClassAttribute : Attribute
{
}

/// <summary>
/// Specifies type of fields, which could be field or override
/// </summary>
public enum DataAttributeType
{
    [Description("fields")]
    Field,
    [Description("override")]
    Override,
    [Description("parameter")]
    Parameter
}
