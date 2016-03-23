using System;

namespace FaxcomManager
{
    public interface IOpmComponent : IDisposable
    {
        /// <summary>
        /// Processes the call.
        /// </summary>
        /// <param name="call">The call.</param>
        /// <returns></returns>
        long ProcessCall(CALLOBJECTLib.Call call);

        /// <summary>
        /// Processes the end of OPM job with cleanup here.
        /// </summary>
        /// <returns></returns>
        long ProcessEnd();
    }
}
