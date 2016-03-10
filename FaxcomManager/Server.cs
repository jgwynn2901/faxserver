using System;
using System.Runtime.InteropServices;
using FnsUtility;
using Constants = FnsInterop.Constants;

namespace FaxcomManager
{

    /// <summary>
    /// 
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("5C8EE9A3-39C9-457A-BB14-EC117D8A465B")]
    public class Server : IDisposable
    {
        // Call Object will have filename to process here:
        public const string ImageFileAttribute = "IMAGE_FILENAME";
        private const string ErrorStringAttribute = "ERRORSTRING";
        private const string DestinationStringAttribute = "DESTINATION_STRING";
        // N.B. these should be in the config file and NOT hard coded.
        private const string Queue = @"\\ltr1fx03\FaxcomQ_SMTPOPMEDIFaxQ";
        private const string Username = "Administrator";

        /// <summary>
        /// Processes the call.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        public long ProcessCall( CALLOBJECTLib.Call call)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, new CallingMethod());
                call.SetValue(ErrorStringAttribute, ex.Message);
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
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
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
