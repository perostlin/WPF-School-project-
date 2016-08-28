using System;
using System.Windows;
using System.Xml;

namespace WpfProject.Classes
{
    public static class AppConfigChanger
    {
        public static void UpdateConfigKey(string strKey, string newValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");

            if (!ConfigKeyExists(strKey))
            {
                MessageBox.Show("Nyckel:n <" + strKey + "> hittades inte i configuration.", "Ingen nyckel hittad",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

                foreach (XmlNode childNode in appSettingsNode)
                {
                    if (childNode.Attributes["key"].Value == strKey)
                        childNode.Attributes["value"].Value = newValue;
                }
                xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");
                xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                MessageBox.Show("Adressen uppdaterades!");
            }
        }

        private static bool ConfigKeyExists(string strKey)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\App.config");

            XmlNode appSettingsNode = xmlDoc.SelectSingleNode("configuration/appSettings");

            foreach (XmlNode childNode in appSettingsNode)
            {
                if (childNode.Attributes["key"].Value == strKey)
                    return true;
            }
            return false;
        }
    }
}
