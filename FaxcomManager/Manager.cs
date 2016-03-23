using System;
using System.Runtime.InteropServices;
using CALLOBJECTLib;
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

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, new CallingMethod());
                call.SetValue(Server.ErrorStringAttribute, ex.Message);
                return Constants.E_FAIL;
            }
            return Constants.S_OK;
        }
    }
}
