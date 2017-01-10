using System;
using System.Configuration;
using System.Web.Configuration;

namespace redfoodie
{
    public interface IAppHarborConfig
    {
        string Get(string key);
    }

    public class AppHarborConfig: IAppHarborConfig
    {
        public string Get(string key)
        {
            var fromConfig = WebConfigurationManager.AppSettings[key];
            return string.Equals(fromConfig, "{ENV}", StringComparison.InvariantCultureIgnoreCase) ? Environment.GetEnvironmentVariable(key) : fromConfig;
        }
    }
}