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
     public class Server : OpmComponentBase
    {
        // Call Object will have filename to process here:
        public const string ImageFileAttribute = "IMAGE_FILENAME";
        public const string ErrorStringAttribute = "ERRORSTRING";
        public const string DestinationStringAttribute = "DESTINATION_STRING";
        public const string FaxJobId = "FAX_JOB_ID";
        // N.B. these should be in the config file and NOT hard coded.

        /// <summary>
        /// Processes the call.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        public override long ProcessCall(CALLOBJECTLib.Call call)
        {
            try
            {
                var faxNumber = call.GetValue(DestinationStringAttribute);
                Assert.Test(!string.IsNullOrEmpty(faxNumber), "Missing Destination: (Fax #)");
                var filename = call.GetValue(ImageFileAttribute);
                Assert.Test(!string.IsNullOrEmpty(filename),"Missing File!");
                call.SetValue(ErrorStringAttribute, "");
                var sender = new Sender
                {
                    Email = "test@sedgwickcms.com",
                    Company = call.GetValue("CLAIM:ACCOUNT:NAME"),
                    FaxNumber = "6178862045",
                    Name = call.GetValue("CLAIM:INSURED:NAME")
                };
                Assert.Test(sender.Validate().IsValid, sender.LastError);
                var recipient = new Recipient
                {
                    Account = call.GetValue("CALL_ID"),
                    FaxNumber = faxNumber,
                    Company = call.GetValue("CLAIM:INSURED:NAME"),
                    Name = call.GetValue("CLAIM:RISK_LOCATION:NAME")
                };
                Assert.Test(recipient.Validate().IsValid, recipient.LastError);
                var queue = new FaxQueue
                {
                    Queue = ApplicationConfiguration.Instance.FaxQueue,
                    Username = ApplicationConfiguration.Instance.FaxUser
                };
                Assert.Test(queue.Validate().IsValid, queue.LastError);
                IFaxComponent worker = new FaxComponent(filename);
                var results = worker.SendFax(queue, sender, recipient).Body.SendFaxResult;
                call.SetValue(FaxJobId, results.Data);
                call.SetValue("FAX_SUBMISSION_DETAIL", results.Detail);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, new CallingMethod());
                call.SetValue(ErrorStringAttribute, ex.Message);
            }
            return Constants.S_OK;
        }
    }
}
