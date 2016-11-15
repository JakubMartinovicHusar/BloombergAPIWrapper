using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using System.Reflection;
using BloombergAPIWrapper.Data.Attributes;
using System.ComponentModel;
using BloombergAPIWrapper.Messaging;

namespace BloombergAPIWrapper.Data
{
    /// <summary>
    /// Bloomberg data engine serves for cracking of messages predefined in namespace .Messaging.Responses
    /// </summary>
    public class BloombergDataEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BloombergDataEngine"/> class. This provides basic functions for cracking of various bloomberg responses, all of various types which are present under namespace Messaging.Responses
        /// </summary>
        /// <param name="session">The session.</param>
        public BloombergDataEngine(Session session) {
            this.session = session;
        }

        Session session;

        private long GenerateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Retrieves the data from message processing custom request just to return custom response.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request of concrete type.</param>
        /// <param name="service">The service is default parameter with default value null. If argument is not supplied service is created by the method.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Request was not specified.</exception>
        public BloombergResponse<T> RetrieveData<T>(BloombergRequest<T> request, Service service=null, bool Async = true)
        {
            //Session session;
            BloombergResponse<T> result = (BloombergResponse<T>)Activator.CreateInstance(request.GetUnderlyingResponseType(), new object[]{request.RequestClass});
            Type t = typeof(T);

            Request blRequest;

            #region Initialize Service and Request if not provided
            /*if (request.GetSessionOptions() != null) { session = new Session(request.GetSessionOptions()); session.Start(); } // special session
            else session = this.session;*/
            if (service == null) service = session.GetService(request.GetService());
            if (request != null)
                blRequest = request.GetBloombergRequest(service);
            else throw new Exception("Request was not specified.");
            #endregion

            CorrelationID requestID = new CorrelationID(GenerateId());
            EventQueue eventQueue = new EventQueue();
            // Generate request from object definition
            
            session.SendRequest(blRequest, eventQueue, requestID);
            
            bool continueToLoop = true;
            if (Async)
            {
                result.StartMessageProcessing();
                while (continueToLoop)
                {
                    Event eventObj = eventQueue.NextEvent();
                    if (eventObj.Type == Event.EventType.RESPONSE)
                        continueToLoop = false;
                    // Cracking messages

                    foreach (Message message in eventObj.GetMessages())
                    {
                        result.AddMessage(message);
                        //result.ProcessMessage(message);
                    }
                    if (result.ProcessingError != null) { break; throw result.ProcessingError; }
                }
                result.StopMessageProcessing();
            }
            else {
                while (continueToLoop)
                {
                    Event eventObj = eventQueue.NextEvent();
                    if (eventObj.Type == Event.EventType.RESPONSE)
                        continueToLoop = false;
                    // Cracking messages

                    foreach (Message message in eventObj.GetMessages())
                    {
                       result.ProcessMessage(message);
                    }
                    if (result.ProcessingError != null) { break; throw result.ProcessingError; }
                }
            }
            return result;
        }


    }
}
