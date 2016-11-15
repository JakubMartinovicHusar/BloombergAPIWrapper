using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using System.Reflection;
using BloombergAPIWrapper.Data.Attributes;

namespace BloombergAPIWrapper.Messaging
{
    public abstract class BloombergRequest<T>
    {

        public BloombergRequest() {
            requestClass = (T)Activator.CreateInstance(typeof(T));
        }
        internal virtual SessionOptions GetSessionOptions() { return null; }
        internal virtual string GetService() { return "//blp/refdata"; }
        protected abstract string GetOperationName();
        Type t = typeof(T);
        T requestClass;
        public T RequestClass { get { return requestClass; } }
        Request blRequest;
        public Request GetBloombergRequest(Service service) {
            if (blRequest == null)
            {
                blRequest = service.CreateRequest(GetOperationName());
                AssignFields(blRequest);
                AssignOverrides(blRequest);
                AssignParameters(blRequest);
            }
            return blRequest;
        }

        public Request RecreateBloombergRequest(Service service) {
            blRequest = null;
            GetBloombergRequest(service);
            return blRequest;
        }

        protected virtual void AssignFields(Request BloombergRequest){
            foreach (PropertyInfo i in t.GetProperties())
            {

                foreach (Attribute a in i.GetCustomAttributes(true))
                {
                    if (a is BloombergDataAttribute)
                    {
                        BloombergDataAttribute da = (BloombergDataAttribute)a;
                        if (da.Fillable == true && da.Type == BloombergDataAttributeType.Field)
                            BloombergRequest.Append(Global.GetEnumDescription(da.Type), (da.FieldName != null ? da.FieldName : i.Name));
                    }
                }
            }
        }

        protected virtual void AssignOverrides(Request BloombergRequest)
        {
            Element overrides = null;
            foreach (FieldInfo i in t.GetFields())
            {
                foreach (Attribute a in i.GetCustomAttributes(true))
                {
                    if (a is BloombergDataAttribute)
                    {
                       
                        BloombergDataAttribute da = (BloombergDataAttribute)a;
                        if (da.Fillable == true && da.Type == BloombergDataAttributeType.Override){
                            if (overrides == null)
                               overrides = BloombergRequest["overrides"]; 

                            Element overrideCust = overrides.AppendElement();
                            string fieldName = (da.FieldName != null ? da.FieldName : i.Name);
                            overrideCust.SetElement("fieldId", fieldName);
                            MethodInfo mi = typeof(Element).GetMethod("SetElement", new Type[] { typeof(string), (i.FieldType == typeof(DateTime) ? typeof(Datetime) : i.FieldType) });
                            if(i.FieldType == typeof(DateTime)) // datetime is done in special way
                                mi.Invoke(overrideCust, new object[] { "value", new Datetime(Convert.ToDateTime(i.GetValue(requestClass)))});
                            else
                                mi.Invoke(overrideCust, new object[] { "value", Convert.ChangeType(i.GetValue(requestClass), i.FieldType) });
                        }
                    }
                }
            }
        }
        protected virtual void AssignParameters(Request BloombergRequest)
        {

        }

        internal abstract Type GetUnderlyingResponseType();

    }
}
