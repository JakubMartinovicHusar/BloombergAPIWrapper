using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using BloombergAPIWrapper.Data.Attributes;
using Bloomberglp.Blpapi;
using System.Threading;

namespace BloombergAPIWrapper.Messaging
{
    public abstract class BloombergResponse<T>
    {
        protected static readonly Name EXCEPTIONS = Name.GetName("exceptions");
        protected static readonly Name FIELD_ID = Name.GetName("fieldId");
        protected static readonly Name REASON = Name.GetName("reason");
        protected static readonly Name CATEGORY = Name.GetName("category");
        protected static readonly Name DESCRIPTION = Name.GetName("description");

        public List<Bloomberglp.Blpapi.Message> Messages = new List<Bloomberglp.Blpapi.Message>();

        object RequestClass;
        public BloombergResponse(object RequestClass)
        {
            this.RequestClass = RequestClass;
        }

        /// <summary>
        /// Appends message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void AddMessage(Bloomberglp.Blpapi.Message message)
        {
            if (message != null)
            {
                CheckExceptions(message);
                Messages.Add(message);
            }
            
        }

        /// <summary>
        /// Checks for exceptions in bloomberg message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception">
        /// </exception>
        protected void CheckExceptions(Message message){
            if (message.HasElement(REASON))
            {
                // This can occur on SubscriptionFailure.
                Element reason = message.GetElement(REASON);
               throw new Exception(String.Format("Bloomberg Exception : {0} {1}",
                   reason.GetElement(CATEGORY).GetValueAsString(),
                   reason.GetElement(DESCRIPTION).GetValueAsString()));
            }
            if(message.HasElement(EXCEPTIONS)){
                Element exceptions = message.GetElement(EXCEPTIONS);
                for (int i = 0; i < exceptions.NumValues; ++i )
                {
                    Element exInfo = exceptions.GetValueAsElement(i);
                    Element fieldId = exInfo.GetElement(FIELD_ID);
                    Element reason = exInfo.GetElement(REASON);
                    throw new Exception(String.Format("Bloomberg Exception : {0} {1}", fieldId.GetValueAsString(), reason.GetElement(CATEGORY).GetValueAsString()));
                }
            }
        }

        /// <summary>
        /// Checks for the errors in element of bloomberg response.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="System.Exception"></exception>
        protected void CheckErrors(Element element) { 
        /*securityError = {
                source = 244::bbdbd9
                code = 15
                category = BAD_SEC
                message = Unknown/Invalid security [nid:244] 
                subcategory = INVALID_SECURITY*/
            if(!element.IsNull)
            for(int i = 0; i<element.NumElements; i++){
                Element subElement = element.GetElement(i);
                string elementName = subElement.Name.ToString();
                if (elementName.Contains("error") || elementName.Contains("Error")) {
                    throw new Exception(String.Format("Bloomberg Exception : ({0}, {1}) {2} - {3}({4})",
                        subElement.GetElement("source").GetValueAsString(),
                        subElement.GetElement("code").GetValueAsString(),
                        subElement.GetElement("category").GetValueAsString(),
                        subElement.GetElement("message").GetValueAsString(),
                        subElement.GetElement("subcategory").GetValueAsString()
                        )
                        );
                }
            }

        }


        /// <summary>
        /// Processes the messages in queue.
        /// </summary>
        internal abstract void ProcessMessage(Message message);

        /// <summary>
        /// Processes the fields of concrete element.
        /// </summary>
        /// <param name="fieldData">The field data.</param>
        /// <returns></returns>
        protected virtual T ProcessFields(Element fieldData) {
            Type t = typeof(T);
            T result = (RequestClass is T && RequestClass is ICloneable ? ((T)((ICloneable)RequestClass).Clone()) : (T)Activator.CreateInstance(t));
            foreach (PropertyInfo pi in t.GetProperties())
            {
                foreach (Attribute a in pi.GetCustomAttributes(true))
                {
                    if (a is BloombergDataAttribute)
                    {
                        BloombergDataAttribute da = (BloombergDataAttribute)a;
                        if (da.Retrievable == true)
                        {
                            if (da.IsArray == true)
                            {
                                string field = (da.ReturnName != null ? da.ReturnName : (da.FieldName != null ? da.FieldName : pi.Name));
                                Element fieldDataCollection  = fieldData.GetElement(field);
                                if (!fieldDataCollection.IsArray) throw new Exception("Response class marked field as array type, however bloomberg response message type is not an array");
                                for (int i = 0; i < fieldDataCollection.NumValues; i++)
                                {
                                    Element subElement = (Element)fieldDataCollection.GetValue(i);
                                    Type propType = pi.PropertyType;
                                    object objProp = pi.GetValue(result, null);
                                    MethodInfo mi = propType.GetMethod("Add");
                                    mi.Invoke(objProp, new object[] { GetDataOfCorrectType(subElement, da.ArrayFieldName, da.ArrayUnderlyingType) });
                                }
                            }
                            else
                            {
                                string field = (da.ReturnName != null ? da.ReturnName : (da.FieldName != null ? da.FieldName : pi.Name));
                                pi.SetValue(result, GetDataOfCorrectType(fieldData, field, pi.PropertyType), null);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the content of element (cracking specific field), type must be specified explicitly.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="field">The field name.</param>
        /// <param name="t">Explicitly specified type.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        protected object GetDataOfCorrectType(Element element, string field, Type t){
            object result = null;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = Nullable.GetUnderlyingType(t);
            if (element.HasElement(field))
            {
                if (t == typeof(string)) result = element.GetElementAsString(field);
                else if (t == typeof(int)) result = element.GetElementAsInt32(field);
                else if (t == typeof(long)) result = element.GetElementAsInt64(field);
                else if (t == typeof(float)) result = float.Parse(element.GetElementAsString(field), System.Globalization.CultureInfo.CurrentCulture);
                else if (t == typeof(double)) result = double.Parse(element.GetElementAsString(field), System.Globalization.CultureInfo.CurrentCulture);
                else if (t == typeof(DateTime)) result = element.GetElementAsDatetime(field).ToSystemDateTime();
                else throw new Exception(String.Format("Return type {0} has no conversion rule.", t.FullName));
            }
            return result;
        
        }
        Thread msgProcessingThread;
        internal void StartMessageProcessing()
        {
            msgProcessingThread = new Thread(ProcessMessages);
            msgProcessingThread.Start();
        }
        public Exception ProcessingError;

        void ProcessMessages() {
            while (true) {
                try
                {
                    if (Messages.Count != 0)
                    {
                        if (Messages[0] != null)
                        {
                            ProcessMessage(Messages[0]);
                            Messages.RemoveAt(0);
                        }

                    }
                }
                catch (Exception exc) {
                    ProcessingError = exc;
                    break;
                }
            }
        }

        internal void StopMessageProcessing()
        {
            while (Messages.Count > 0 && ProcessingError==null) Thread.Sleep(10);
            if (msgProcessingThread != null && msgProcessingThread.ThreadState == ThreadState.Running) msgProcessingThread.Abort(); 
        }
    }
}
