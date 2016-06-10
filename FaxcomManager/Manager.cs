using System;
using System.Runtime.InteropServices;
using CALLOBJECTLib;
using FaxServer;
using FaxServer.Model;
using FnsUtility;
using Constants = FnsInterop.Constants;

namespace FaxcomManager
{
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("EA5DE6DB-3DDA-4262-8FD9-1DDC495E02FA")]
    public class Manager : OpmComponentBase
    {
        public override long ProcessCall(Call call)
        {
            try
            {
                const string faxStatusText = "FAX_STATUS_TEXT";
                var jobId = call.GetValue(Server.FaxJobId);

                call.SetValue(Server.ErrorStringAttribute, "");
                Assert.IsTrue(!string.IsNullOrEmpty(jobId));
                var queue = new FaxQueue
                {
                    Queue = ApplicationConfiguration.Instance.FaxQueue,
                    Username = ApplicationConfiguration.Instance.FaxUser
                };
                for (var wait = 0; wait < 3; ++wait)
                {
                    System.Threading.Thread.Sleep(ApplicationConfiguration.Instance.FaxManagerTimeout);
                    var response = FaxComponent.GetFaxResponse(queue, jobId);
                    Assert.IsNotNull(response, "response is null.");
                    Assert.IsNotNull(response.Body, "response.Body is null.");

                    if (response.Body.GetMessageStatusByUniqueIDResult == null) continue;
                    call.SetValue("FAX_STATUS_NAME", Value: response.Body.GetMessageStatusByUniqueIDResult.StatusName);
                    call.SetValue(faxStatusText, Value: response.Body.GetMessageStatusByUniqueIDResult.StatusText);
                    break;
                }
                if (call.GetValue("FAX_STATUS_NAME").ToLower() != "ok")
                {
                    throw new ApplicationException(
                        $"Result is null or not ok for {jobId} {call.GetValue(faxStatusText)}");
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, new CallingMethod());
                call.SetValue(Server.ErrorStringAttribute, ex.Message);
            }
            return Constants.S_OK;
        }
    }
}
