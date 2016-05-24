using DbClassLibrary;
using FnsUtility;

namespace FaxcomManager
{
    public class ApplicationConfiguration : ApConfigBase
    {
        private static ApplicationConfiguration _singletonInstance;

        protected ApplicationConfiguration()
        {
            FaxQueue = ConfigurationReader.GetAppConfigValue("FaxQueue");
            FaxUser = ConfigurationReader.GetAppConfigValue("FaxUser");
            var timeout = ConfigurationReader.GetAppConfigValue("FaxManagerTimeout");
            FaxManagerTimeout = string.IsNullOrEmpty(timeout) ? 22000 : int.Parse(timeout);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ApplicationConfiguration Instance => _singletonInstance ?? (_singletonInstance = new ApplicationConfiguration());

        public string FaxQueue { get; private set; }
        public string FaxUser { get; private set; }
        public int FaxManagerTimeout { get; private set; }
    }
}
