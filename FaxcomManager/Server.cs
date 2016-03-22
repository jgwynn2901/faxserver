using System;
using System.Runtime.InteropServices;
using FaxServer;
using FaxServer.Model;
using FnsUtility;
using Constants = FnsInterop.Constants;

namespace FaxcomManager
{
    /// <summary>
    /// 
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("5C8EE9A3-39C9-457A-BB14-EC117D8A465B")]
     public class Server : IDisposable, IServer
    {
        public Server() { }

        // Call Object will have filename to process here:
        public const string ImageFileAttribute = "IMAGE_FILENAME";
        private const string ErrorStringAttribute = "ERRORSTRING";
        private const string DestinationStringAttribute = "DESTINATION_STRING";
        // N.B. these should be in the config file and NOT hard coded.

        /// <summary>
        /// Processes the call.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        public long ProcessCall(CALLOBJECTLib.Call call)
        {
            try
            {
                var faxNumber = call.GetValue(DestinationStringAttribute);
                Assert.Test(!string.IsNullOrEmpty(faxNumber), "Missing Destination: (Fax #)");
                var filename = call.GetValue(ImageFileAttribute);
                Assert.Test(!string.IsNullOrEmpty(filename),"Missing File!");
                var sender = new Sender
                {
                    Email = "test@firstnotice.com",
                    Company = call.GetValue("CLAIM:ACCOUNT:NAME"),
                    FaxNumber = call.GetValue("CLAIM:ACCOUNT:PHONE_FAX"),
                    Name = call.GetValue("CLAIM:INSURED:NAME")
                };
                Assert.Test(sender.IsValid, sender.LastError);
                var recipient = new Recipient
                {
                    Account = call.GetValue("CALL_ID"),
                    FaxNumber = faxNumber,
                    Company = call.GetValue("CLAIM:INSURED:NAME"),
                    Name = call.GetValue("CLAIM:RISK_LOCATION:NAME")
                };
                Assert.Test(recipient.IsValid, recipient.LastError);
                var queue = new FaxQueue
                {
                    Queue = ApplicationConfiguration.Instance.FaxQueue,
                    Username = ApplicationConfiguration.Instance.FaxUser
                };
                Assert.Test(queue.IsValid, queue.LastError);
                IFaxComponent worker = new FaxComponentProxy(filename);
                var results = worker.SendFax(queue, sender, recipient).Body.LoginAndSendNewFaxMessageResult.Data;
                call.SetValue(ErrorStringAttribute, results);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, new CallingMethod());
                call.SetValue(ErrorStringAttribute, ex.Message);
                return Constants.E_FAIL;
            }
            return Constants.S_OK;
        }

        /// <summary>
        /// Processes the end of OPM job with cleanup here.
        /// </summary>
        /// <returns></returns>
        public long ProcessEnd()
        {
            return Constants.S_OK;
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Server() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
