using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloomberglp.Blpapi;
using System.Threading;
using System.IO;
using System.Reflection;
using BloombergAPIWrapper.Data;
using BloombergAPIWrapper.Messaging.Requests;
using BloombergAPIWrapper.Messaging;

namespace BloombergAPIWrapper
{
    public class ConnectionHandler
    {

        public ConnectionHandler() {

            Initialize();
        }

        static Session sessionData = null;
        static bool connected;
        static DataEngine engine;
        /// <summary>
        /// Gets the engine.
        /// </summary>
        /// <value>
        /// The engine.
        /// </value>
        public DataEngine Engine{ 
            get {return engine;}
        }
        static SessionOptions sessionOptions = new SessionOptions();
        /// <summary>
        /// Gets the session data.
        /// </summary>
        /// <returns></returns>
        public Session GetSessionData()
        {
            return sessionData;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize() {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Connects to bloomberg API
        /// </summary>
        /// <exception cref="System.Exception">
        /// System can not connect to Bloomberg API
        /// or
        /// System can not connect to service of Bloomberg API
        /// </exception>
        public void Connect(bool throwException = false)
        {

            if (!connected)
            {
                try
                {
                    sessionOptions.ServerHost = "localhost"; // default value
                    sessionOptions.ServerPort = 8194; // default value
                    sessionOptions.ClientMode = SessionOptions.ClientModeType.DAPI;
                    sessionOptions.ConnectTimeout = 100;
                    sessionData = new Session(sessionOptions);
                    bool res = sessionData.Start();
                    if (!res)
                    {
                        throw new Exception("System can not connect to Bloomberg API");
                    }
                    res = sessionData.OpenService("//blp/exrsvc");
                    if (!res)//blp/refdata
                    {
                        throw new Exception("System can not connect to service of Bloomberg API");
                    }
                    res = sessionData.OpenService("//blp/refdata");
                    if (!res)//blp/refdata
                    {
                        throw new Exception("System can not connect to service of Bloomberg API");
                    }
                    engine = new DataEngine(sessionData);
                    connected = true;
                    if (BloombergConnectionChanged != null) BloombergConnectionChanged((object)this, new EventArgs());
                }
                catch (Exception ex)
                {
                    if (throwException)
                    throw ex;
                    

                }
            }
        }

        public event System.EventHandler BloombergConnectionChanged;

        /// <summary>
        /// Gets a value indicating whether this instance is connected to API service.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get { return connected; } }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect(bool throwException = false)
        {
            try
            {

                if (IsConnected)
                {
                    if (sessionData != null)
                    {
                        sessionData.Stop();
                        sessionData = null;
                        connected = false;
                    }
                    if (BloombergConnectionChanged != null) BloombergConnectionChanged((object)this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                if(throwException)
                throw ex;


            }
            
        }




    }
}
