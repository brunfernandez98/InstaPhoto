using System;
using Util.SettingsManagerInterface;
using System.Configuration;
namespace Util.SettingsManager
{
    public class SettingManager : ISettingManager
    {
        public string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading app settings");
                return string.Empty;
            }
        }
    }
}
