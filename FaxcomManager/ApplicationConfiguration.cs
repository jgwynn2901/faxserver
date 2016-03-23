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
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ApplicationConfiguration Instance => _singletonInstance ?? (_singletonInstance = new ApplicationConfiguration());

        public string FaxQueue { get; private set; }
        public string FaxUser { get; private set; }
    }
}
