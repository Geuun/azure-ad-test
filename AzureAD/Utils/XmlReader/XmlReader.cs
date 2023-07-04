using System.Xml;

namespace AzureAD.Utils.XmlReader
{
    public class XmlReader
    {
        private static readonly string envPath = @"C:\dev\env\";

        public static string GetAppSettingConfig(string key)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(envPath + "AzureADAppSettings.xml");

            string value = doc.SelectSingleNode(@"//appSettings/add[@" + key + @"]")?.Attributes[key]?.Value;

            return value;
        }
    }
}